using Protocol.Constant;
using Protocol.DTO.Fight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol.DTO
{
    /// <summary>
    /// player in the fightroom
    /// </summary>
    [Serializable]
    public class PlayerDTO
    {
        public int UserId { get; set; }

        public string Username { get; set; }

        public int BetSum { get; set; }

        public Identity identity { get; set; }

        public List<CardDTO> cardList;

        public List<CardDTO> communityList;

        /// <summary>
        /// handlist = community + card
        /// </summary>
        public List<CardDTO> handList;


        //public CardType cardType;

        public double handRank;

        public PlayerDTO(int userId, string username)
        {
            UserId = userId;
            Username = username;
            BetSum = 0;
            identity = Identity.Small;
            cardList = new List<CardDTO>();
            handList = new List<CardDTO>();
            communityList = new List<CardDTO>();
            handRank = -1;
        }

        public void AddCard(CardDTO cardDTO)
        {
            cardList.Add(cardDTO);
        }

        public void RemoveCard(CardDTO cardDTO)
        {
            cardList.Remove(cardDTO);
        }
    }
}
