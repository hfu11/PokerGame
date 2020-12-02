using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol.DTO
{
    /// <summary>
    /// 用户信息传输模型
    /// </summary>
    [Serializable]
    public class UserDTO
    {
        public int userId;
        public string username;
        public int coin;

        public UserDTO (int userId, string username, int coin)
        {
            this.userId = userId;
            this.username = username;
            this.coin = coin;
        }

        public void Change(int userId, string username, int coin)
        {
            this.userId = userId;
            this.username = username;
            this.coin = coin;
        }
    } 
}
