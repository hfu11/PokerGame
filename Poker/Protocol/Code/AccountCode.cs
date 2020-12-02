using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol.Code
{
    /// <summary>
    /// 账户的子操作码
    /// </summary>
    public class AccountCode
    {
        /// <summary>
        /// Client Request
        /// </summary>
        public const int Register_CREQ = 0;
        /// <summary>
        /// Server Response
        /// </summary>
        public const int Register_SRES = 1;
        /// <summary>
        /// Login Request
        /// </summary>
        public const int Login_CREQ = 2;
        /// <summary>
        /// Server Response
        /// </summary>
        public const int Login_SRES = 3;
        /// <summary>
        /// Client Request
        /// </summary>
        public const int GetUserInfo_CREQ = 4;
        /// <summary>
        /// Server Response
        /// </summary>
        public const int GetUserInfo_SRES = 5;
        /// <summary>
        /// Client Request
        /// </summary>
        public const int GetRankList_CREQ = 6;
        /// <summary>
        /// Server Response
        /// </summary>
        public const int GetRankList_SRES = 7;
        /// <summary>
        /// Client Request
        /// </summary>
        public const int UpdateCoin_CREQ = 8;
        /// <summary>
        /// Server Response
        /// </summary>
        public const int UpdateCoin_SRES = 9;
    }
}
