using GameServer.Cache;
using GameServer.Database;
using Org.BouncyCastle.Asn1.Pkcs;
using Protocol.Code;
using Protocol.DTO;
using Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GameServer.Logic
{
    /// <summary>
    /// delegate to start a game
    /// </summary>
    /// <param name="clientList"></param>
    /// <param name="roomType"></param>
    public delegate void StartFight(List<ClientPeer> clientList, int roomType);

    public class MatchHandler : Ihandler
    {
        /// <summary>
        /// 匹配房间缓存集合
        /// </summary>
        private List<MatchCache> matchCacheList = Caches.matchCacheList;
        public StartFight startFight;
        public void Disconnect(ClientPeer client)
        {
            for (int i = 0; i < matchCacheList.Count; i++)
            {
                LeaveRoom(client, i);
            }
        }

        public void Receive(ClientPeer client, int subCode, object value)
        {
            switch (subCode)
            {
                case MatchCode.Enter_CREQ:
                    EnterRoom(client, (int)value);
                    break;
                case MatchCode.Leave_CREQ:
                    LeaveRoom(client, (int)value);
                    break;
                case MatchCode.Ready_CREQ:
                    Ready(client, (int)value);
                    break;
                case MatchCode.UnReady_CREQ:
                    UnReady(client, (int)value);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 客户端取消准备的请求
        /// </summary>
        private void UnReady(ClientPeer client, int roomType)
        {
            SingleExecute.Instance.Execute(() =>
            {
                if (matchCacheList[roomType].IsMatching(client.Id) == false) return;

                MatchRoom room = matchCacheList[roomType].GetRoomOfUser(client.Id);

                room.UnReady(client.Id);
                room.Broadcast(OpCode.Match, MatchCode.UnReady_BRO, client.Id, client);
            });
        }


        /// <summary>
        /// 客户端准备的请求
        /// </summary>
        /// <param name="client"></param>
        /// <param name="roomType"></param>
        private void Ready(ClientPeer client, int roomType)
        {
            SingleExecute.Instance.Execute(() =>
            {
                //safety check
                if (matchCacheList[roomType].IsMatching(client.Id) == false) return;

                MatchRoom room = matchCacheList[roomType].GetRoomOfUser(client.Id);
                room.Ready(client.Id);
                room.Broadcast(OpCode.Match, MatchCode.Ready_BRO, client.Id, client);

                //if everyone is ready, the game shall start
                if (room.IsAllReady())
                {
                    startFight(room.ClientList, roomType);
                    room.Broadcast(OpCode.Match, MatchCode.StartGame_BRO, null);
                    matchCacheList[roomType].DestroyRoom(room);
                }
            });
        }

        /// <summary>
        /// 客户端进入房间的请求
        /// </summary>
        /// <param name="client"></param>
        /// <param name="roomType"></param>
        private void EnterRoom(ClientPeer client, int roomType)
        {
            SingleExecute.Instance.Execute(() =>
            {
                if (matchCacheList[roomType].IsMatching(client.Id))
                {
                    return;
                }

                var room = matchCacheList[roomType].Enter(client);

                UserDTO userDTO = DatabaseManager.CreateUserDTO(client.Id);

                //broadcast that a user entered
                room.Broadcast(OpCode.Match, MatchCode.Enter_BRO, userDTO, client);

                //transimit MatchroomDTO to client
                client.SendMsg(OpCode.Match, MatchCode.Enter_SRES, CreateMatchRoomDTO(room));

                //debug
                if(roomType == 0)
                {
                    Console.WriteLine(userDTO.username + " enter room min10");
                }
                if(roomType == 1)
                {
                    Console.WriteLine(userDTO.username + " enter room min40");
                }
                if(roomType == 2)
                {
                    Console.WriteLine(userDTO.username + " enter room min100");
                }
            });
        }

        /// <summary>
        /// return a MatchRoomDTO
        /// </summary>
        /// <param name="room"></param>
        /// <returns></returns>
        private MatchRoomDTO CreateMatchRoomDTO(MatchRoom room)
        {
            MatchRoomDTO matchRoomDTO = new MatchRoomDTO();
            for (int i = 0; i < room.ClientList.Count; i++)
            {
                matchRoomDTO.Enter(DatabaseManager.CreateUserDTO(room.ClientList[i].Id));
            }
            matchRoomDTO.ReadyUserList = room.ReadyUserIDList;
            return matchRoomDTO;
        }

        /// <summary>
        /// 客户端离开的请求
        /// </summary>
        /// <param name="client"></param>
        /// <param name="roomType"></param>
        private void LeaveRoom(ClientPeer client, int roomType)
        {
            SingleExecute.Instance.Execute(() =>{
                //玩家不在匹配房间，忽略
                if (matchCacheList[roomType].IsMatching(client.Id) == false) return;

                var room = matchCacheList[roomType].Leave(client.Id);
                room.Broadcast(OpCode.Match, MatchCode.Leave_BRO, client.Id, client);
                
            });
        }
    }
}
