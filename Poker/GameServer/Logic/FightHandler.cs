using GameServer.Cache;
using GameServer.Cache.Fight;
using GameServer.Database;
using MySql.Data.MySqlClient.Memcached;
using Org.BouncyCastle.Cms;
using Protocol.Code;
using Protocol.Constant;
using Protocol.DTO;
using Protocol.DTO.Fight;
using Server;
using Server.TimerTool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Protocol.DTO.Fight.GameOverDTO;

namespace GameServer.Logic
{
    public class FightHandler : Ihandler
    {
        private FightCache fightCache = Caches.fightCache;

        /// <summary>
        /// bet DTO
        /// </summary>
        private BetDto betDto = new BetDto();
        /// <summary>
        /// game over DTO
        /// </summary>
        private GameOverDTO gameOverDTO = new GameOverDTO();
        public void Disconnect(ClientPeer client)
        {
            //throw new NotImplementedException();
            LeaveRoom(client);
        }

        public void Receive(ClientPeer client, int subCode, object value)
        {
            //throw new NotImplementedException();
            switch (subCode)
            {
                case FightCode.Leave_CREQ:
                    LeaveRoom(client);
                    break;
                case FightCode.Call_CREQ:
                    Call(client);
                    break;
                case FightCode.RAISE_CREQ:
                    Raise(client, (int)value);
                    break;
                case FightCode.FOLD_CREQ:
                    Fold(client);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// game is over, go for next game defaultly. or if everybody left, destroy the room
        /// </summary>
        private void GameOver(FightRoom room)
        {
            Dictionary<PlayerDTO, int> winnerCoinDict = new Dictionary<PlayerDTO, int>();
            List<PlayerDTO> loserList = new List<PlayerDTO>();

            //find winners and losers

            //players who still in game
            List<PlayerDTO> playersInGame = new List<PlayerDTO>();

            foreach (var item in room.playerList)
            {
                if (room.IsFold(item.UserId) || room.IsLeaveRoom(item.UserId))
                {
                    loserList.Add(item);
                }
                else
                {
                    playersInGame.Add(item);
                }
            }
            #region get winners and losers

            if (playersInGame.Count == 1)
            {
                winnerCoinDict.Add(playersInGame[0], room.pot);
            }
            else if(playersInGame.Count == 2)
            {
                var a = playersInGame[0];
                var b = playersInGame[1];

                if(a.handRank > b.handRank)
                {
                    //a wins
                    winnerCoinDict.Add(a, room.pot);
                    loserList.Add(b);
                }
                else if(a.handRank == b.handRank)
                {
                    //tie
                    winnerCoinDict.Add(a, room.pot / 2);
                    winnerCoinDict.Add(b, room.pot / 2);
                }
                else
                {
                    //b wins
                    winnerCoinDict.Add(b, room.pot);
                    loserList.Add(a);
                }
            }
            else if(playersInGame.Count == 3)
            {
                //sort by hand rank
                playersInGame.Sort((x, y) => -x.handRank.CompareTo(y.handRank));

                var a = playersInGame[0];
                var b = playersInGame[1];
                var c = playersInGame[2];

                if (a.handRank != b.handRank && b.handRank != c.handRank)
                {
                    //only a(with highest rank) wins
                    winnerCoinDict.Add(a, room.pot);
                    loserList.Add(b);
                    loserList.Add(c);
                }
                else if(a.handRank == b.handRank && b.handRank != c.handRank)
                {
                    //a and b tied
                    winnerCoinDict.Add(a, room.pot / 2);
                    winnerCoinDict.Add(b, room.pot / 2);
                    loserList.Add(c);
                }
                else if(a.handRank == b.handRank && b.handRank == c.handRank)
                {
                    //a, b and c tied
                    winnerCoinDict.Add(a, room.pot / 3);
                    winnerCoinDict.Add(b, room.pot / 3);
                    winnerCoinDict.Add(c, room.pot / 3);
                }

            }

            #endregion

            foreach (var item in winnerCoinDict)
            {
                DatabaseManager.UpdateCoin(item.Key.UserId, item.Value);
            }

            //debug
            foreach (var item in winnerCoinDict)
            {
                Console.WriteLine(item.Key.Username + " wins: " + item.Value);
            }

            //check where to go
            if(room.leaveUserIdList.Count > 0)
            {
                gameOverDTO.Change(winnerCoinDict, loserList, ActionType.Over);
                room.Broadcast(OpCode.Fight, FightCode.GameOver_BRO, gameOverDTO);

                fightCache.DestroyRoom(room);
            }
            else
            {
                gameOverDTO.Change(winnerCoinDict, loserList, ActionType.Continue);
                room.Broadcast(OpCode.Fight, FightCode.GameOver_BRO, gameOverDTO);

                //same as StartFight, except that we flush instead create a room

                room.Flush();

                //set dealer, small and big. get big's client
                ClientPeer big = room.SetDSB();

                //debug
                for (int i = 0; i < room.playerList.Count; i++)
                {
                    var item = room.playerList[i];
                    Console.WriteLine(item.Username + " is " + item.identity + " Index: " + i);
                }

                //place blind bets
                room.UpdatePlayerBet(room.GetUserIdByIdentity(Identity.Big), room.minBet);
                room.UpdatePlayerBet(room.GetUserIdByIdentity(Identity.Small), room.minBet / 2);
                room.betCounter = 0;

                //deal
                room.DealCards();

                //roundtype = first round
                room.StartRound();

                //sort
                room.SortAllPlayerCard();

                //get hand rank
                room.GetAllPlayerHandRank();

                room.Broadcast(OpCode.Fight, FightCode.StartFight_BRO, room.playerList);

                //take turn to place bet, start from the player next to big(which will be dealer)
                Turn(big);
            }

            TimerManager.Instance.Clear();
        }

        /// <summary>
        /// 客户端弃牌的请求处理
        /// </summary>
        /// <param name="client"></param>
        private void Fold(ClientPeer client)
        {
            SingleExecute.Instance.Execute(() =>
            {
                if (fightCache.IsFighting(client.Id) == false) return;

                var room = fightCache.GetFightRoomByUserId(client.Id);

                room.foldUserIdList.Add(client.Id);
                room.Broadcast(OpCode.Fight, FightCode.FOLD_BRO, client.Id);

                //player who folds is placing a bet
                if (room.roundModel.CurrentBetUserId == client.Id)
                {
                    Turn(client);
                }

                if(room.foldUserIdList.Count + room.leaveUserIdList.Count == 2)
                {
                    //only one player playing
                    GameOver(room);
                }

                if (room.foldUserIdList.Count == 2)
                {
                    //only one player playing
                    GameOver(room);
                    return;
                }

            });
        }

        private void Raise(ClientPeer client, int multiple)
        {
            SingleExecute.Instance.Execute(() =>
            {
                if (fightCache.IsFighting(client.Id) == false) return;

                var room = fightCache.GetFightRoomByUserId(client.Id);

                var player = room.GetPlayerDtoByUserId(client.Id);

                int betToPlace = room.lastPlayerBetSum * multiple - player.BetSum;

                var playerCoin = DatabaseManager.GetCoinByUserId(client.Id);

                //if the player does not have that mush coin, lastPlayerBetSum should be as mush the player has
                if (betToPlace > playerCoin)
                {
                    betToPlace = playerCoin;
                }

                int playerBetSum = room.UpdatePlayerBet(client.Id, betToPlace);
                //update lastPlayerBetSum
                room.lastPlayerBetSum = playerBetSum;

                int remainCoin = DatabaseManager.UpdateCoin(client.Id, -betToPlace);

                betDto.Change(client.Id, remainCoin, betToPlace, playerBetSum);
                room.Broadcast(OpCode.Fight, FightCode.Raise_BRO, betDto);

                Turn(client);

            });
        }

        /// <summary>
        /// client calls, this will make their bet sum equal to last player bet sum
        /// </summary>
        /// <param name="client"></param>
        private void Call(ClientPeer client)
        {
            SingleExecute.Instance.Execute(() =>
            {
                if (fightCache.IsFighting(client.Id) == false) return;

                var room = fightCache.GetFightRoomByUserId(client.Id);

                var player = room.GetPlayerDtoByUserId(client.Id);

                //bet to place for this time is the difference between last player's betsum and player's own betsum
                int betToPlace = room.lastPlayerBetSum - player.BetSum;

                //if the player does not have that mush coin, betToPlace should be as mush the player have 
                var playerCoin = DatabaseManager.GetCoinByUserId(client.Id);
                if (betToPlace > playerCoin)
                {
                    betToPlace = playerCoin;
                }

                int playerBetSum = room.UpdatePlayerBet(client.Id, betToPlace);
                int remainCoin = DatabaseManager.UpdateCoin(client.Id, -betToPlace);
                betDto.Change(client.Id, remainCoin, betToPlace, playerBetSum);
                room.Broadcast(OpCode.Fight, FightCode.Call_BRO, betDto);



                //if everyone's bet sum is the same

                if (room.IsAllBetEqual() && room.betCounter >= room.playerList.Count)
                {
                    //go to the next round
                    var nextAction = room.NextActionInRound();
                    switch (nextAction)
                    {
                        case RoundType.Flop:
                            room.Broadcast(OpCode.Fight, FightCode.Flop_BRO, null);
                            //small blind to bet
                            SmallBlindBet(room, client);
                            break;
                        case RoundType.Turn:
                            room.Broadcast(OpCode.Fight, FightCode.Turn_BRO, null);
                            //small blind to bet
                            SmallBlindBet(room, client);
                            break;
                        case RoundType.River:
                            room.Broadcast(OpCode.Fight, FightCode.River_BRO, null);
                            //small blind to bet
                            SmallBlindBet(room, client);
                            break;
                        case RoundType.Show:
                            GameOver(room);
                            break;
                        default:
                            break;
                    }
                }

                //turn to next player to bet
                Turn(client);
            });
        }

        private void SmallBlindBet(FightRoom room, ClientPeer client)
        {
            foreach (var item in room.playerList)
            {
                if(item.identity == Identity.Dealer)
                {
                    Turn(client, item.UserId);
                }
            }
        }

        private ClientPeer timerClient;
        /// <summary>
        /// turn to the next player to bet by current betting client
        /// </summary>
        /// <param name="client"></param>
        private void Turn(ClientPeer client, int toId = -1)
        {
            //clear timer tasks firstly
            TimerManager.Instance.Clear();


            if (fightCache.IsFighting(client.Id) == false) return;

            var room = fightCache.GetFightRoomByUserId(client.Id);
            int nextId = room.Turn();

            if(room.IsFold(nextId) || room.IsLeaveRoom(nextId) || (toId != -1 && nextId != toId))
            {
                //turn to available player or certain player
                Turn(client, toId);
            }
            else
            {
                timerClient = DatabaseManager.GetClientPeerByUserId(nextId);
                
                TimerManager.Instance.AddTimerEvent(60, TimerDelegate);
                Console.WriteLine("current betting player: " + client.Username);
                room.Broadcast(OpCode.Fight, FightCode.StartBet_BRO, nextId);
            }
        }

        /// <summary>
        /// 计时器结束时执行的任务
        /// </summary>
        private void TimerDelegate()
        {
            //automatically call when time runs out
            Call(timerClient);
        }

        /// <summary>
        /// player leave fightroom
        /// </summary>
        /// <param name="client"></param>
        private void LeaveRoom(ClientPeer client)
        {
            SingleExecute.Instance.Execute(() =>
            {
                //safety check
                if (fightCache.IsFighting(client.Id) == false) return;

                var room = fightCache.GetFightRoomByUserId(client.Id);
                room.leaveUserIdList.Add(client.Id);

                if (room.foldUserIdList.Contains(client.Id))
                {
                    //player fold before leave
                    room.foldUserIdList.Remove(client.Id);
                }

                room.Broadcast(OpCode.Fight, FightCode.Leave_BRO, client.Id);

                //player who left is placing a bet
                if (room.roundModel.CurrentBetUserId == client.Id)
                {
                    Turn(client);
                }

                if (room.leaveUserIdList.Count == 2)
                {
                    //only one player is playing
                    GameOver(room);
                    return;
                }
                if(room.leaveUserIdList.Count == 3)
                {
                    fightCache.DestroyRoom(room);
                }
            });
        }

        /// <summary>
        /// 开始战斗
        /// </summary>
        /// <param name="clientList"></param>
        /// <param name="roomType"></param>
        public void StartFight(List<ClientPeer> clientList, int roomType)
        {
            SingleExecute.Instance.Execute(() =>
            {
                var room = fightCache.CreateRoom(clientList);
                switch (roomType)
                {
                    case 0:
                        room.minBet = 10;
                        room.lastPlayerBetSum = room.minBet;
                        break;
                    case 1:
                        room.minBet = 40;
                        room.lastPlayerBetSum = room.minBet;
                        break;
                    case 2:
                        room.minBet = 100;
                        room.lastPlayerBetSum = room.minBet;
                        break;
                    default:
                        break;
                }

                //set dealer, small and big. get big's client
                ClientPeer big = room.SetDSB();

                //debug
                for (int i = 0; i < room.playerList.Count; i++)
                {
                    var item = room.playerList[i];
                    Console.WriteLine(item.Username + " is " + item.identity + " Index: " + i);
                }

                //place blind bets
                room.UpdatePlayerBet(room.GetUserIdByIdentity(Identity.Big), room.minBet);
                room.UpdatePlayerBet(room.GetUserIdByIdentity(Identity.Small), room.minBet/2);
                room.betCounter = 0;
                
                //deal
                room.DealCards();

                //roundtype = first round
                room.StartRound();

                //sort
                room.SortAllPlayerCard();
                //get hand rank
                room.GetAllPlayerHandRank();

                room.Broadcast(OpCode.Fight, FightCode.StartFight_BRO, room.playerList);

                //take turn to place bet, start from the player next to big(which will be dealer)
                Turn(big);
            });
        }
    }
}
