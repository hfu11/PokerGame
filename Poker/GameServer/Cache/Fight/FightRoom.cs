using GameServer.Database;
using K4os.Compression.LZ4.Encoders;
using MySql.Data.MySqlClient.Memcached;
using Protocol.Constant;
using Protocol.DTO;
using Protocol.DTO.Fight;
using Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Cache.Fight
{
    /// <summary>
    /// Logic of fight room
    /// </summary>
    public class FightRoom
    {
        public int RoomId { get; set; }

        /// <summary>
        /// players of the game
        /// </summary>
        public List<PlayerDTO> playerList;

        /// <summary>
        /// A deck of 52 cards
        /// </summary>
        public CardLibrary cardLibrary;

        /// <summary>
        /// round model keeps track of 1.current bet user; 2.
        /// </summary>
        public RoundModel roundModel;

        /// <summary>
        /// players who left
        /// </summary>
        public List<int> leaveUserIdList;

        /// <summary>
        /// players who fold
        /// </summary>
        public List<int> foldUserIdList;
        /// <summary>
        /// minBet of the game
        /// </summary>
        public int minBet;

        /// <summary>
        /// last player's betSum
        /// </summary>
        public int lastPlayerBetSum;

        /// <summary>
        /// pot of the room
        /// </summary>k
        public int pot;

        /// <summary>
        /// index of the dealer in playerList
        /// </summary>
        public int dealerIndex = -1;


        /// <summary>
        /// how many times bets have been made in a single round
        /// </summary>
        public int betCounter = -1;

        public FightRoom(int roomId, List<ClientPeer> clientList)
        {
            this.RoomId = roomId;
            playerList = new List<PlayerDTO>();
            foreach (var item in clientList)
            {
                PlayerDTO dto = new PlayerDTO(item.Id, item.Username);
                playerList.Add(dto);
            }
            cardLibrary = new CardLibrary();
            roundModel = new RoundModel();
            leaveUserIdList = new List<int>();
            foldUserIdList = new List<int>();
            pot = 0;

        }

        public void Init(List<ClientPeer> clientList)
        {
            playerList.Clear();
            foreach (var item in clientList)
            {
                PlayerDTO dto = new PlayerDTO(item.Id, item.Username);
                playerList.Add(dto);
            }
            pot = 0;
            betCounter = 0;
        }

        /// <summary>
        /// set up some data for the next game, keep the playerList and dealerIndex
        /// </summary>
        public void Flush()
        {
            pot = 0;
            betCounter = 0;
            foldUserIdList.Clear();
            leaveUserIdList.Clear();
            cardLibrary.Init();
            roundModel.Init();
            lastPlayerBetSum = minBet;

            foreach (var item in playerList)
            {
                item.handList.Clear();
                item.cardList.Clear();
                item.communityList.Clear();
                item.BetSum = 0;
            }
        }

        /// <summary>
        /// destroy the room(actually reset everything instead literally destroy)
        /// </summary>
        public void Destroy()
        {
            playerList.Clear();
            cardLibrary.Init();
            roundModel.Init();
            leaveUserIdList.Clear();
            foldUserIdList.Clear();
            pot = 0;
            dealerIndex = -1;
            betCounter = 0;

        }

        /// <summary>
        /// At the beginning of a game,set Dealer, Small and Big. return the big blind's client
        /// </summary>
        public ClientPeer SetDSB()
        {
            int playerCount = playerList.Count;

            if(dealerIndex != -1)
            {
                dealerIndex++;
                dealerIndex %= playerCount;
            }
            else
            {
                Random ran = new Random();
                int ranv = ran.Next(0, playerCount);
                dealerIndex = ranv;
            }
            //set dealer
            playerList[dealerIndex].identity = Identity.Dealer;

            //set small 
            int smallIndex = dealerIndex + 1;
            smallIndex %= playerCount;
            playerList[smallIndex].identity = Identity.Small;

            //set big 
            int bigIndex = dealerIndex + 2;
            bigIndex %= playerCount;
            playerList[bigIndex].identity = Identity.Big;

            int bigId = playerList[bigIndex].UserId;
            //set the current bet user id to bigId
            roundModel.StartBet(bigId);
            ClientPeer bigClient = DatabaseManager.GetClientPeerByUserId(bigId);

            return bigClient;
        }

        /// <summary>
        /// 发牌
        /// </summary>
        public void DealCards()
        {
            int dealIndex = dealerIndex;
            for (int i = 0; i < 6; i++)
            {
                playerList[dealIndex].AddCard(cardLibrary.DealCard());
                dealIndex++;
                if (dealIndex > playerList.Count - 1)
                {
                    dealIndex = 0;
                }
            }

            //community card
            var communityList = new List<CardDTO>();
            for(int i = 0; i < 5; i++)
            {
                communityList.Add(cardLibrary.DealCard());
            }

            foreach (var player in playerList)
            {
                player.handList.AddRange(player.cardList);
                player.handList.AddRange(communityList);
                player.communityList.AddRange(communityList);
            }
        }

        /// <summary>
        /// 排序牌组
        /// </summary>
        private void SortCard(ref List<CardDTO> cardList)
        {
            for (int i = 0; i < cardList.Count; i++)
            {
                for (int j = i + 1; j < cardList.Count; j++)
                {
                    if (cardList[j].Value < cardList[i].Value)
                    {
                        var temp = cardList[j];
                        cardList[j] = cardList[i];
                        cardList[i] = temp;
                    }
                }
            }
        }

        /// <summary>
        /// sort all player's handcards
        /// </summary>
        public void SortAllPlayerCard()
        {
            foreach (var item in playerList)
            {
                //sort own cards
                SortCard(ref item.cardList);
                //sort hand cards
                SortCard(ref item.handList);
            }
        }

        /// <summary>
        /// get all players handranks
        /// </summary>
        public void GetAllPlayerHandRank()
        {
            foreach (var item in playerList)
            {
                item.handRank = GetHandRank(item.handList);
            }
        }
        
        /// <summary>
        /// helper function to GetHandRank to caculate handrank within range
        /// </summary>
        /// <param name="cards"></param>
        /// <param name="excludeCardValue"></param>
        /// <param name="excludeCardValue2"></param>
        /// <param name="limitCheck"></param>
        /// <param name="normalize"></param>
        /// <returns></returns>
        private double EvaluateRankByHighestCards(List<CardDTO> cards, int excludeCardValue = -1, int excludeCardValue2 = -1, int limitCheck = 7, double normalize = 433175)
        {
            int i = 0;
            double sum = 0;
            int fixedSize = cards.Count() - 1;

            for (int j = fixedSize; j >= 0; j--)
            {
                int cardValue = cards[j].Value;
                if (cardValue == excludeCardValue || cardValue == excludeCardValue2)
                    continue;

                int normalizedValue = cardValue - 2; // since CardValue is an integer between [2,14]

                sum += normalizedValue * Math.Pow(13, fixedSize - i);

                if (i == limitCheck - 1)
                    break;

                i++;
            }

            return (double)sum / normalize;
        }

        /// <summary>
        /// get hand rank of a player
        /// </summary>
        /// <param name="cardList">7 cards from community and player's cards</param>
        /// <returns></returns>
        private double GetHandRank(List<CardDTO> cardList)
        {

            if (cardList.Count != 7) return -1;

            int dupCount = 1, seqCount = 1, seqCountMax = 1;
            int maxCardValue = -1, dupValue = -1, seqMaxValue = -1;

            int currCardValue = -1, nextCardValue = -1;
            int currCardSuit = -1, nextCardSuit = -1;

            // Since the cards are sorted, we can find in O(1) the max value.
            maxCardValue = cardList[6].Value;

            /* 
             * There's no more than 3 series of dpulicates possible in 7 cards.
             * Struct: [Value: Count], for example:
             * [4, 3] : There are 3 cards of the value 4.
             */
            List<double[]> duplicates = new List<double[]>();

            for(int i = 0; i < 6; i++)
            {
                currCardValue = cardList[i].Value;
                currCardSuit = cardList[i].Suit;

                nextCardValue = cardList[i + 1].Value;
                nextCardSuit = cardList[i + 1].Suit;

                //check for duplicates
                if(currCardValue == nextCardValue)
                {
                    dupCount++;
                    dupValue = currCardValue;
                }
                else if(dupCount > 1)
                {
                    duplicates.Add(new double[] { dupCount, currCardValue });
                }

                //check for sequence
                if(currCardValue + 1 == nextCardValue)
                {
                    seqCount++;
                }
                /*
                 * Another edge case:
                 * the reason why we are checking if curCardValue != nextCardValue is to ensure that
                 * cases like these: 7,8,8,8,9,10,11 will also consider as a straight of seqCount = 5,
                 * therefore we will reset the seqCount if and only if curValue != nextValue completly,
                 * but if it has the same number, we will just simply skip seqCount++, but won't reset the counter.
                 * */
                else if(currCardValue != nextCardValue)
                {
                    if(seqCount > seqCountMax)
                    {
                        seqCountMax = seqCount;
                        seqMaxValue = currCardValue;
                    }

                    seqCount = 1;
                }
            }
            //check the 7th card right after the loop
            if(seqCount > seqCountMax)
            {
                seqCountMax = seqCount;
                seqMaxValue = nextCardValue;
            }
            if(dupCount > 1)
            {
                duplicates.Add(new double[] { dupCount, nextCardValue });
            }

            /*
             * if we got this far it means we finished to calculate everything needed and we
             * are ready to start checks for the player's hand rank
             */

            double rank = 0;


            if (cardList[6].Value == 14
                && cardList[5].Value == 13
                && cardList[4].Value == 12
                && cardList[3].Value == 11
                && cardList[2].Value == 10
                && cardList[6].Suit == cardList[5].Suit
                && cardList[5].Suit == cardList[4].Suit
                && cardList[4].Suit == cardList[3].Suit
                && cardList[3].Suit == cardList[2].Suit)
            {
                rank = 900;
            }
            else
            {
                //check for straight flush, rank: [800, 900)
                for (int suit = 0; suit < 4; suit++)
                {
                    //4 suits
                    var suitCards = cardList.Where(x => x.Suit == suit).ToList();
                    if(suitCards.Count >= 5)
                    {
                        // There's no way for duplicates, since every card in the same suit is unique.
                        int counter = 1, lastValue = -1;
                        for (int i = 0; i < suitCards.Count - 1; i++)
                        {
                            if(suitCards[i].Value + 1 == suitCards[i + 1].Value)
                            {
                                counter++;
                                lastValue = suitCards[i + 1].Value;
                            }
                            else
                            {
                                counter = 1;
                            }
                        }
                        if(counter >= 5)
                        {
                            //straight flush
                            rank = 800 + (double)lastValue / 14 * 99;
                        }
                        //edge case 2 3 4 5 A
                        else if(counter == 4 && lastValue == 5 && suitCards[suitCards.Count - 1].Value == 14)
                        {
                            rank = 835.3571; // 800 + 5/14 * 99
                        }
                    }
                }

                if (rank == 0)
                {
                    //sort the duplicates cards according by the amount(descend)
                    duplicates.Sort((x, y) => (int)y[0] - (int)x[0]);

                    //check for four of a kind. rank:[700,800)
                    if (duplicates.Count > 0 && duplicates[0][0] == 4)
                    {
                        //card value of the four of a kind
                        var temp = duplicates[0][1];
                        rank = 700 + temp / 14 * 50 + EvaluateRankByHighestCards(cardList, (int)temp, -1, 1);

                    }
                    //check for a full house, rank: [600,700)
                    //edge case: there are 2 pairs of the 2, for example: 333 22 66
                    else if (duplicates.Count > 2 && duplicates[0][0] == 3 && duplicates[1][0] == 2 && duplicates[2][0] == 2)
                    {
                        //in this edge case, we will check which one of the two pairs is greater
                        double maxTmpValue = Math.Max(duplicates[1][1], duplicates[2][1]);

                        rank = 600 + (duplicates[0][1]) + maxTmpValue / 14;
                    }
                    //normal case full house
                    else if(duplicates.Count > 1 && duplicates[0][0] == 3 && duplicates[1][0] == 2)
                    {
                        rank = 600 + (duplicates[0][1]) + duplicates[1][1] / 14;
                    }
                    //edge case: there are 2 pairs of three of a kind: 333 222
                    else if (duplicates.Count > 1 && duplicates[0][0] == 3 && duplicates[1][0] == 3)
                    {
                        double rank1, rank2;
                        rank1 = 600 + (duplicates[0][1]) + duplicates[1][1] / 14;
                        rank2 = 600 + (duplicates[1][1]) + duplicates[0][1] / 14;
                        rank = Math.Max(rank1, rank2);
                    }

                    else
                    {
                        //check for flush, rank:[500,600)
                        for (int suit = 0; suit < 4; suit++)
                        {
                            var suitCards = cardList.Where(x => x.Suit == suit).ToList();
                            if(suitCards.Count >= 5)
                            {
                                //we only need 5 cards. For example 2 3 5 7 9 11 14 all in same suit. Then only 5 7 9 11 14 is needed(actually only 14 is needed)
                                var suitCardResult = suitCards.Skip(suitCards.Count - 5).ToList();

                                rank = 500 + EvaluateRankByHighestCards(suitCardResult);
                                break;
                            }
                        }

                        if(rank == 0)
                        {
                            //check for straight, rank: [400,500)
                            if(seqCount >= 5)
                            {
                                rank = 400 + (double)seqMaxValue / 14 * 99;
                            }

                            //edge case: 2 3 4 5 A
                            else if(seqCount == 4 && seqMaxValue == 5 && maxCardValue == 14)
                            {
                                rank = 435.3571;// 400 + 5/14 * 99
                            }

                            //check for three of a kind
                            else if (duplicates.Count >0 && duplicates[0][0] == 3)
                            {
                                rank = 300 + duplicates[0][1] / 14 * 50 + EvaluateRankByHighestCards(cardList, (int)duplicates[0][1]);
                            }

                            //check for two pairs, rank:[200,300)
                            else if (duplicates.Count > 1 && duplicates[0][0] == 2 && duplicates[1][0] == 2)
                            {
                                if (duplicates.Count > 2 && duplicates[0][0] == 2 && duplicates[1][0] == 2 && duplicates[2][0] == 2)
                                {
                                    double[] threePairsValues = new double[] { duplicates[0][1], duplicates[1][1], duplicates[2][1] };
                                    Array.Sort(threePairsValues, (x, y) => (int)(y - x));
                                    rank = 200 + (Math.Pow(threePairsValues[0], 2) / 392 + Math.Pow(threePairsValues[1], 2) / 392) * 50 + EvaluateRankByHighestCards(cardList, (int)threePairsValues[0], (int)threePairsValues[1]);
                                }
                                else
                                {
                                    rank = 200 + (Math.Pow(duplicates[0][1], 2) / 392 + Math.Pow(duplicates[1][1], 2) / 392) * 50 + EvaluateRankByHighestCards(cardList, (int)duplicates[0][1], (int)duplicates[1][1]);
                                }
                            }

                            //check for one pair, rank: [100,200)
                            else if(duplicates.Count > 0 && duplicates[0][0] == 2)
                            {
                                rank = 100 + duplicates[0][1] / 14 * 50 + EvaluateRankByHighestCards(cardList, (int)duplicates[0][1], -1, 3);
                            }

                            //otherwise it will only be high card, rank:[0,100)
                            else
                            {
                                rank = EvaluateRankByHighestCards(cardList, -1, -1, 5);
                            }
                        }
                    }
                }
            }
            return rank;
        }

        /// <summary>
        /// is player leave already
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool IsLeaveRoom(int userId)
        {
            return leaveUserIdList.Contains(userId);
        }

        /// <summary>
        /// is player fold already
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool IsFold(int userId)
        {
            return foldUserIdList.Contains(userId);
        }

        /// <summary>
        /// turn to the next player to place bet
        /// </summary>
        /// <returns>下一次下注的玩家ID</returns>
        public int Turn()
        {
            int currentUserId = roundModel.CurrentBetUserId;
            int nextId =  GetNextUserId(currentUserId);
            roundModel.Turn(nextId);
            return nextId;
        }

        /// <summary>
        /// update player's betsum by betCount this time and return his betsum
        /// </summary>
        public int UpdatePlayerBet(int userId, int betCount)
        {
            foreach (var item in playerList)
            {
                if(item.UserId == userId)
                {
                    //this player made a bet, increase betCounter
                    betCounter++;

                    item.BetSum += betCount;
                    pot += betCount;
                    return item.BetSum;
                }
            }
            return -1;
        }

        /// <summary>
        /// get the next player who need to place bet
        /// </summary>
        /// <param name="currentId"></param>
        /// <returns></returns>
        private int GetNextUserId(int currentId)
        {
            for (int i = 0; i < playerList.Count; i++)
            {
                if(playerList[i].UserId == currentId)
                {
                    if(i == playerList.Count - 1)
                    {
                        return playerList[0].UserId;
                    }
                    else
                    {
                        return playerList[i + 1].UserId;
                    }
                }
            }
            //循环到这里，说明有问题
            return -1;
        }

        /// <summary>
        ///  broadcast message to players in room
        /// </summary>
        /// <param name="opCode"></param>
        /// <param name="subCode"></param>
        /// <param name="obj"></param>
        /// <param name="excepteClient">需要剔除的玩家</param>
        public void Broadcast(int opCode, int subCode, object value, ClientPeer excepteClient = null)
        {
            foreach (var player in playerList)
            {
                var client = DatabaseManager.GetClientPeerByUserId(player.UserId);

                if (client == excepteClient)
                {
                    continue;
                }
                client.SendMsg(opCode, subCode, value);
            }
        }

        /// <summary>
        /// get user id(big, small or dealer)
        /// </summary>
        /// <returns></returns>
        public int GetUserIdByIdentity(Identity identity)
        {
            foreach (var item in playerList)
            {
                if(item.identity == identity)
                {
                    return item.UserId;
                }
            }
            return -1;
        }

        public PlayerDTO GetPlayerDtoByUserId(int userId)
        {
            foreach (var item in playerList)
            {
                if(userId == item.UserId)
                {
                    return item;
                }
            }
            return null;
        }

        /// <summary>
        /// check if everybody's bet sum is the same, if so we shall move to the next round
        /// </summary>
        /// <returns></returns>
        public bool IsAllBetEqual()
        {
            bool res = true;
            for (int i = 1; i < playerList.Count; i++)
            {
                if(playerList[i].BetSum != playerList[i - 1].BetSum)
                {
                    res = false;
                    break;
                }
            }
            return res;
        }

        public void StartRound()
        {
            roundModel.roundType = RoundType.First;
        }

        public RoundType NextActionInRound()
        {
            //safety check
            if(roundModel.roundType != RoundType.Show && roundModel.roundType != RoundType.None)
            {
                //reset betCounter
                betCounter = 0;

                roundModel.roundType++;
            }

            return roundModel.roundType;
        }


    }
}
