using MySql.Data.MySqlClient;
using MySql.Data.MySqlClient.Memcached;
using Protocol.DTO;
using Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Database
{
    public class DatabaseManager
    {
        private static MySqlConnection sqlConnection;
        private static Dictionary<int, ClientPeer> idClientDict;
        private static List<string> onlineUserList;
        private static RankListDTO rankListDTO;

        public static void StartConnect()
        {
            try
            {
                idClientDict = new Dictionary<int, ClientPeer>();
                onlineUserList = new List<string>();
                rankListDTO = new RankListDTO();
                string conStr = "database=poker;data source=127.0.0.1;port=3306;user=root;pwd=root";
                sqlConnection = new MySqlConnection(conStr);
                sqlConnection.Open();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        #region Account

        public static bool IsUsernameExist(string username)
        {
            MySqlCommand cmd = new MySqlCommand("select username from userinfo where username = @user", sqlConnection);
            cmd.Parameters.AddWithValue("user", username);
            MySqlDataReader reader = cmd.ExecuteReader();
            bool res = reader.HasRows;
            reader.Close();
            return res;
        }

        public static void CreateUser(string username, string password)
        {
            MySqlCommand cmd = new MySqlCommand("insert into userinfo set username=@username,password=@pwd", sqlConnection);
            cmd.Parameters.AddWithValue("username", username);
            cmd.Parameters.AddWithValue("pwd", password);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// password match username
        /// </summary>
        /// <param name="username"></param>
        /// <param name="pwd"></param>
        public static bool IsMatch(string username, string pwd)
        {
            bool res = false;
            MySqlCommand cmd = new MySqlCommand("select * from userinfo where username = @user", sqlConnection);
            cmd.Parameters.AddWithValue("user", username);
            MySqlDataReader reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                reader.Read();
                res = (reader.GetString("password") == pwd);
            }
            reader.Close();
            return res;
        }

        public static bool IsOnline(string username)
        {
            //if (onlineUserList.Contains(username))
            //{
            //    return true;
            //}
            //else
            //{
            //    return false;
            //}
            return onlineUserList.Contains(username);

            //bool res = false;
            //MySqlCommand cmd = new MySqlCommand("select isonline from userinfo where username = @user", sqlConnection);
            //cmd.Parameters.AddWithValue("user", username);
            //MySqlDataReader reader = cmd.ExecuteReader();
            //if (reader.HasRows)
            //{
            //    reader.Read();
            //    res = reader.GetBoolean("isonline");
            //}
            //reader.Close();
            //return res;
        }

        public static void Login(string username, ClientPeer client)
        {
            //MySqlCommand cmd = new MySqlCommand("update userinfo set isonline=true where username=@user", sqlConnection);
            //cmd.Parameters.AddWithValue("user", username);
            //cmd.ExecuteNonQuery();

            MySqlCommand cmd1 = new MySqlCommand("select * from userinfo where username=@user", sqlConnection);
            cmd1.Parameters.AddWithValue("user", username);
            MySqlDataReader reader = cmd1.ExecuteReader();
            if (reader.HasRows)
            {
                reader.Read();
                int id = reader.GetInt32("id");
                client.Id = id;
                client.Username = username;
                if (!idClientDict.ContainsKey(id))
                {
                    idClientDict.Add(id, client);
                }
                if (!onlineUserList.Contains(username))
                {
                    onlineUserList.Add(username);
                }

                reader.Close();
            }
        }

        public static void Logout(ClientPeer client)
        {
            if (idClientDict.ContainsKey(client.Id))
            {
                idClientDict.Remove(client.Id);
            }
            if (onlineUserList.Contains(client.Username))
            {
                onlineUserList.Remove(client.Username);
            }
        }

        public static ClientPeer GetClientPeerByUserId(int userId)
        {
            if (idClientDict.ContainsKey(userId))
            {
                return idClientDict[userId];
            }
            return null;
        }

        /// <summary>
        /// create a userDto for account and match handlers
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static UserDTO CreateUserDTO(int id)
        {
            MySqlCommand cmd = new MySqlCommand("select * from userinfo where id=@id",sqlConnection);
            cmd.Parameters.AddWithValue("id", id);
            MySqlDataReader reader = cmd.ExecuteReader();
            UserDTO userDTO = null;
            if (reader.HasRows)
            {
                reader.Read();
                userDTO = new UserDTO(id, reader.GetString("username"), reader.GetInt32("coin"));
            }
            reader.Close();
            return userDTO;
        }
        /// <summary>
        /// for leaderboard data
        /// </summary>
        /// <returns></returns>
        public static RankListDTO GetRankListDTO()
        {
            MySqlCommand cmd = new MySqlCommand("select username,coin from userinfo order by coin desc",sqlConnection);
            MySqlDataReader reader = cmd.ExecuteReader();
            rankListDTO.Clear();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    RankItemDTO dto = new RankItemDTO(reader.GetString("username"), reader.GetInt32("coin"));
                    rankListDTO.Add(dto);
                }
                reader.Close();
                return rankListDTO;
            }
            reader.Close();
            return null;
        }

        /// <summary>
        /// update user's coin by count this time and return remain coin
        /// </summary>
        /// <param name="userId">userID</param>
        /// <param name="count">update count for this time</param>
        public static int UpdateCoin(int userId, int count)
        {
            MySqlCommand cmd = new MySqlCommand("select coin from userinfo where id=@id", sqlConnection);
            cmd.Parameters.AddWithValue("id", userId);
            MySqlDataReader reader = cmd.ExecuteReader();
            int currentCoin;
            if (reader.HasRows)
            {
                reader.Read();
                currentCoin = reader.GetInt32("coin");
                reader.Close();

                var value = currentCoin + count;
                if (value < 0) value = 0;

                MySqlCommand cmdUpdate = new MySqlCommand("update userinfo set coin=@coin where id=@id", sqlConnection);
                cmdUpdate.Parameters.AddWithValue("coin", value);
                cmdUpdate.Parameters.AddWithValue("id", userId);
                cmdUpdate.ExecuteNonQuery();

                return value;
            }
            reader.Close();
            return -1;
        }

        public static int GetCoinByUserId(int userId)
        {
            MySqlCommand cmd = new MySqlCommand("select coin from userinfo where id=@id", sqlConnection);
            cmd.Parameters.AddWithValue("id", userId);
            MySqlDataReader reader = cmd.ExecuteReader();
            int coin;
            if (reader.HasRows)
            {
                reader.Read();
                coin = reader.GetInt32("coin");
                reader.Close();

                return coin;
            }
            return -1;
        }

        #endregion

    }
}
