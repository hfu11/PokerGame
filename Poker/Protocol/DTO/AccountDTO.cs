using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol.DTO
{
    /// <summary>
    /// 账号数据传输模型
    /// </summary>
    [Serializable]
    public class AccountDTO
    {
        public string username;
        public string password;

        public AccountDTO(string username, string password)
        {
            this.username = username;
            this.password = password;
        }

        public void Change(string username, string password)
        {
            this.username = username;
            this.password = password;
        }
    }
}
