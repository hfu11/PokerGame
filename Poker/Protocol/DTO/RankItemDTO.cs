using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol.DTO
{
    /// <summary>
    /// 排行榜
    /// </summary>
    [Serializable]
    public class RankItemDTO
    {
        public string username;
        public int coin;

        public RankItemDTO(string username, int coin)
        {
            this.username = username;
            this.coin = coin;
        }

        public void Change(string username, int coin)
        {
            this.username = username;
            this.coin = coin;
        }
    }
}
