using GameServer.Database;
using MySql.Data.MySqlClient.Memcached;
using Protocol.Code;
using Protocol.DTO;
using Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Logic
{
    public class AccountHandler : Ihandler
    {
        public void Disconnect(ClientPeer client)
        {
            DatabaseManager.Logout(client);
        }

        public void Receive(ClientPeer client, int subCode, object value)
        {
            switch (subCode)
            {
                case AccountCode.Register_CREQ:
                    Register(client,value as AccountDTO);
                    break;
                case AccountCode.Login_CREQ:
                    Login(client, value as AccountDTO);
                    break;
                case AccountCode.GetUserInfo_CREQ:
                    GetUserInfo(client);
                    break;
                case AccountCode.GetRankList_CREQ:
                    GetRankList(client);
                    break;
                case AccountCode.UpdateCoin_CREQ:
                    UpdateCoin(client, (int)value);
                    break;
                default:
                    break;
            }
        }

        private void UpdateCoin(ClientPeer client, int count)
        {
            int coin = DatabaseManager.UpdateCoin(client.Id, count);
            client.SendMsg(OpCode.Account, AccountCode.UpdateCoin_SRES, coin);
        }

        /// <summary>
        /// 客户端获取排行榜请求的处理
        /// </summary>
        private void GetRankList(ClientPeer client)
        {
            SingleExecute.Instance.Execute(() =>
            {
                RankListDTO dto = DatabaseManager.GetRankListDTO();
                client.SendMsg(OpCode.Account, AccountCode.GetRankList_SRES, dto);
            });
        }

        private void GetUserInfo(ClientPeer client)
        {
            SingleExecute.Instance.Execute(() =>
            {
                UserDTO userDTO = DatabaseManager.CreateUserDTO(client.Id);
                client.SendMsg(OpCode.Account, AccountCode.GetUserInfo_SRES, userDTO);
            });
        }

        /// <summary>
        /// 客户端登陆的处理
        /// </summary>
        /// <param name="client"></param>
        /// <param name="accountDTO"></param>
        private void Login(ClientPeer client, AccountDTO accountDTO)
        {
            SingleExecute.Instance.Execute(() =>
            {
                if (DatabaseManager.IsUsernameExist(accountDTO.username)==false)
                {
                    client.SendMsg(OpCode.Account, AccountCode.Login_SRES, -1);
                    return;
                }

                if (DatabaseManager.IsMatch(accountDTO.username, accountDTO.password) == false)
                {
                    //password incorrect
                    client.SendMsg(OpCode.Account, AccountCode.Login_SRES, -2);
                    return;
                }

                if (DatabaseManager.IsOnline(accountDTO.username))
                {
                    //account is online
                    client.SendMsg(OpCode.Account, AccountCode.Login_SRES, -3);
                    return;
                }
                DatabaseManager.Login(accountDTO.username, client);
                client.SendMsg(OpCode.Account, AccountCode.Login_SRES, 0);
            });
        }


        /// <summary>
        /// 客户端注册的处理
        /// </summary>
        /// <param name="accountDTO"></param>
        private void Register(ClientPeer client ,AccountDTO accountDTO)
        {
            SingleExecute.Instance.Execute(() =>
            {
                if (DatabaseManager.IsUsernameExist(accountDTO.username))
                {
                    client.SendMsg(OpCode.Account, AccountCode.Register_SRES, -1);
                    return;
                }
                DatabaseManager.CreateUser(accountDTO.username, accountDTO.password);
                client.SendMsg(OpCode.Account, AccountCode.Register_SRES, 0);

            });

        }
    }
}
