using MySql.Data.MySqlClient.Memcached;
using Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Cache.Fight
{
    public class FightCache
    {
        /// <summary>
        /// 用户ID与fightroomID映射
        /// </summary>
        public Dictionary<int, int> userRoomDict = new Dictionary<int, int>();

        /// <summary>
        /// 房间ID与模型映射
        /// </summary>
        public Dictionary<int, FightRoom> roomModelDict = new Dictionary<int, FightRoom>();

        /// <summary>
        /// 房间队列
        /// </summary>
        public Queue<FightRoom> roomQueue = new Queue<FightRoom>();

        /// <summary>
        /// 房间ID
        /// </summary>
        public ThreadSafeInt roomId = new ThreadSafeInt(-1);

        public FightRoom CreateRoom(List<ClientPeer> clientList)
        {
            FightRoom room = null;
            if (roomQueue.Count > 0)
            {
                room = roomQueue.Dequeue();
                room.Init(clientList);
            }
            else
            {
                room = new FightRoom(roomId.AddGetValue(), clientList);
            }
            foreach (var item in clientList)
            {
                userRoomDict.Add(item.Id, room.RoomId);
            }
            roomModelDict.Add(room.RoomId, room);
            return room;
        }

        /// <summary>
        /// 销毁房间
        /// </summary>
        /// <param name="room"></param>
        public void DestroyRoom(FightRoom room)
        {
            roomModelDict.Remove(room.RoomId);
            foreach (var item in room.playerList)
            {
                userRoomDict.Remove(item.UserId);
            }
            //初始化房间数据
            room.Destroy();
            roomQueue.Enqueue(room);
        }

        /// <summary>
        /// 用户是否在战斗房间
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool IsFighting(int userId)
        {
            return userRoomDict.ContainsKey(userId);
        }

        /// <summary>
        /// 利用UserId获取当前所在的房间
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public FightRoom GetFightRoomByUserId(int userId)
        {
            int roomId = userRoomDict[userId];
            return roomModelDict[roomId];
        }


    }
}
