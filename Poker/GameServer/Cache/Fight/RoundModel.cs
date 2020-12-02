using Protocol.Constant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Cache.Fight
{
    /// <summary>
    /// 回合管理类
    /// </summary>
    public class RoundModel
    {
        /// <summary>
        /// 当前下注的玩家ID
        /// </summary>
        public int CurrentBetUserId { get; set; }

        public RoundType roundType { get; set; }

        public RoundModel()
        {
            CurrentBetUserId = -1;
            roundType = RoundType.None;
        }

        public void Init()
        {
            CurrentBetUserId = -1;
            roundType = RoundType.None;
        }

        /// <summary>
        /// player start bet
        /// </summary>
        /// <param name="userId"></param>
        public void StartBet(int userId)
        {
            CurrentBetUserId = userId;
        }

        /// <summary>
        /// turn to userId to bet
        /// </summary>
        /// <param name="userId"></param>
        public void Turn(int userId)
        {
            CurrentBetUserId = userId;
        }

    }
}
