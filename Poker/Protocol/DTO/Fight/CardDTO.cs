using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol.DTO.Fight
{
    /// <summary>
    /// 卡牌传输模型
    /// </summary>
    [Serializable]
    public class CardDTO
    {
        public string cardName;
        public int Value;
        public int Suit;

        public CardDTO(string cardName, int value, int suit)
        {
            this.cardName = cardName;
            this.Value = value;
            this.Suit = suit;
        }
    }
}
