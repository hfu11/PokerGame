using GameServer.Database;
using Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GameServer.Cache
{
    /// <summary>
    /// 匹配缓存层
    /// </summary>
    public class MatchCache
    {
        /// <summary>
        /// userId to roomId he is in
        /// </summary>
        public Dictionary<int, int> userRoomDict = new Dictionary<int, int>();

        /// <summary>
        /// roomId to room Model
        /// </summary>
        public Dictionary<int, MatchRoom> roomModelDict = new Dictionary<int, MatchRoom>();

        /// <summary>
        /// room objects pool
        /// </summary>
        public Queue<MatchRoom> roomQueue = new Queue<MatchRoom>();

        /// <summary>
        /// thread safe interger to gennerate roomId
        /// </summary>
        private ThreadSafeInt roomId = new ThreadSafeInt(-1);

        /// <summary>
        /// iterate through dictionary to join a room
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public MatchRoom Enter(ClientPeer client)
        {
            foreach (var matchRoom in roomModelDict.Values)
            {
                if (matchRoom.IsFull())
                {
                    continue;
                }
                else
                {
                    matchRoom.Enter(client);
                    userRoomDict.Add(client.Id, matchRoom.RoomId);
                    return matchRoom;
                }
            }
            //once we are here it means all rooms are full, we have to get a new room
            MatchRoom room = null;
            if(roomQueue.Count > 0)
            {
                room = roomQueue.Dequeue();
            }
            else
            {
                room = new MatchRoom(roomId.AddGetValue());
            }
            room.Enter(client);
            roomModelDict.Add(room.RoomId, room);
            userRoomDict.Add(client.Id, room.RoomId);
            return room;

        }

        /// <summary>
        /// user leaving room
        /// </summary>
        /// <param name="userId"></param>
        public MatchRoom Leave(int userId)
        {
            var rid = userRoomDict[userId];

            MatchRoom room = roomModelDict[rid];

            room.Leave(DatabaseManager.GetClientPeerByUserId(userId));

            userRoomDict.Remove(userId);

            //如果房间为空了，就将房间加入房间重用队列，还要从roomModelDict中移除掉
            if (room.IsEmpty())
            {
                roomQueue.Enqueue(room);
                roomModelDict.Remove(rid);
            }

            return room;
        }

        /// <summary>
        /// is user in any match room
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool IsMatching(int userId)
        {
            return userRoomDict.ContainsKey(userId);
        }

        /// <summary>
        /// get user's room, return the model
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public MatchRoom GetRoomOfUser(int userId)
        {
            return roomModelDict[userRoomDict[userId]];
        }

        /// <summary>
        /// destroy the room model(drop it to the pool) as game starts
        /// </summary>
        /// <param name="room"></param>
        public void DestroyRoom(MatchRoom room)
        {
            roomModelDict.Remove(room.RoomId);
            foreach (var client in room.ClientList)
            {
                userRoomDict.Remove(client.Id);
            }
            room.ClientList.Clear();
            room.ReadyUserIDList.Clear();
            roomQueue.Enqueue(room);
        }
    }
}
