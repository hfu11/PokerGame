using Protocol.DTO.Fight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Cache.Fight
{
    public class CardLibrary
    {
        private Queue<CardDTO> cardQueue = new Queue<CardDTO>();

        public CardLibrary()
        {
            //初始化牌
            InitCard();
            //洗牌
            Shuffle();
        }

        public CardDTO DealCard()
        {
            if(cardQueue.Count < 9)
            {
                Init();
            }
            return cardQueue.Dequeue();
        }

        public void Init()
        {
            //初始化牌
            InitCard();
            //洗牌
            Shuffle();
        }

        private void InitCard()
        {
            cardQueue.Clear();
            for (int suit = 0; suit < 4; suit++)
            {
                for (int rank = 2; rank <= 14; rank++)
                {
                    CardDTO cardDTO = new CardDTO("card_" + suit + "_" + rank, rank, suit);
                    cardQueue.Enqueue(cardDTO);
                }
            }
        }

        private void Shuffle()
        {
            Random rand = new Random();

            var cardList = cardQueue.ToList<CardDTO>();

            for (int i = 0; i < cardList.Count; i++)
            {
                int ran = rand.Next(0,cardList.Count);
                var temp = cardList[i];
                cardList[i] = cardList[ran];
                cardList[ran] = temp;
            }

            cardQueue.Clear();
            foreach (var card in cardList)
            {
                cardQueue.Enqueue(card);
            }
        }
    }
}
