using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol.DTO.Fight
{
    /// <summary>
    /// 下注的传输模型
    /// </summary>
    [Serializable]
    public class BetDto
    {
        public int userId;
        public int remainCoin;
        /// <summary>
        /// 本次下注数量
        /// </summary>
        public int betCount;
        public int playerBetSum;

        public void Change(int userId, int remainCoin, int betCount, int playerBetSum)
        {
            this.userId = userId;
            this.remainCoin = remainCoin;
            this.betCount = betCount;
            this.playerBetSum = playerBetSum;
        }
    }
}
