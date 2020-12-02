using Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Cache
{
    /// <summary>
    /// 匹配房间
    /// </summary>
    public class MatchRoom
    {
        /// <summary>
        /// RoomId,唯一标识
        /// </summary>
        public int RoomId { get; set; }

        /// <summary>
        /// 房间内的玩家
        /// </summary>
        public List<ClientPeer> ClientList { get; set; }

        /// <summary>
        /// 房间内准备的玩家ID列表
        /// </summary>
        public List<int> ReadyUserIDList { get;private set; }

        public MatchRoom(int ID)
        {
            RoomId = ID;
            ClientList = new List<ClientPeer>();
            ReadyUserIDList = new List<int>();
        }

        /// <summary>
        /// 房间是否满了
        /// </summary>
        /// <returns></returns>
        public bool IsFull()
        {
            return ClientList.Count == 3;
        }
        
        /// <summary>
        /// 房间是否为空
        /// </summary>
        /// <returns></returns>
        public bool IsEmpty()
        {
            return ClientList.Count == 0;
        }

        /// <summary>
        /// 是否全部玩家准备
        /// </summary>
        /// <returns></returns>
        public bool IsAllReady()
        {
            return ReadyUserIDList.Count == 3;
        }

        /// <summary>
        /// 使客户端进入房间
        /// </summary>
        /// <param name="client"></param>
        public void Enter(ClientPeer client)
        {
            ClientList.Add(client);
        }
        /// <summary>
        /// 使客户端离开房间
        /// </summary>
        /// <param name="client"></param>
        public void Leave(ClientPeer client)
        {
            if (ReadyUserIDList.Contains(client.Id))
            {
                ReadyUserIDList.Remove(client.Id);
            }
            ClientList.Remove(client);
        }

        /// <summary>
        /// 准备
        /// </summary>
        /// <param name="userId"></param>
        public void Ready(int userId)
        {
            ReadyUserIDList.Add(userId);
        }

        /// <summary>
        /// 取消准备
        /// </summary>
        /// <param name="userId"></param>
        public void UnReady(int userId)
        {
            ReadyUserIDList.Remove(userId);
        }

        /// <summary>
        /// 广播发送消息
        /// </summary>
        /// <param name="opCode"></param>
        /// <param name="subCode"></param>
        /// <param name="obj"></param>
        /// <param name="excepteClient">需要剔除的玩家</param>
        public void Broadcast(int opCode, int subCode, object value, ClientPeer excepteClient = null)
        {
            foreach (var client in ClientList)
            {
                if(client == excepteClient)
                {
                    continue;
                }
                client.SendMsg(opCode, subCode, value);
            }
        }

    }
}
