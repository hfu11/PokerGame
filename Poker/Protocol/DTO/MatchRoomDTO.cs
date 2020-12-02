using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol.DTO
{
    /// <summary>
    /// 匹配房间传输模型
    /// </summary>
    [Serializable]
    public class MatchRoomDTO
    {
        /// <summary>
        /// 用户ID与对应模型之间的映射字典
        /// </summary>
        public Dictionary<int,UserDTO> UserModelDict { get; private set; }

        /// <summary>
        /// 准备的用户ID
        /// </summary>
        public List<int> ReadyUserList { get; set; }

        /// <summary>
        /// 进入房间顺序的用户ID列表
        /// </summary>
        public List<int> EnterOrderList { get; private set; }

        /// <summary>
        /// 自身玩家的左边玩家ID
        /// </summary>
        public int LeftPlayerId { get; set; }

        /// <summary>
        /// 自身玩家的右边玩家ID
        /// </summary>
        public int RightPlayerId { get; set; }

        public MatchRoomDTO()
        {
            UserModelDict = new Dictionary<int, UserDTO>();
            ReadyUserList = new List<int>();
            EnterOrderList = new List<int>();
        }

        /// <summary>
        /// 进入房间
        /// </summary>
        /// <param name="userDTO"></param>
        public void Enter(UserDTO userDTO)
        {
            UserModelDict.Add(userDTO.userId, userDTO);
            EnterOrderList.Add(userDTO.userId);
        }
        /// <summary>
        /// 离开房间
        /// </summary>
        /// <param name="userDTO"></param>
        public void Leave(int userId)
        {
            UserModelDict.Remove(userId);
            if (ReadyUserList.Contains(userId))
            {
                ReadyUserList.Remove(userId);
            }
            EnterOrderList.Remove(userId);
        }

        /// <summary>
        /// 准备
        /// </summary>
        /// <param name="userId"></param>
        public void Ready(int userId)
        {
            ReadyUserList.Add(userId);
        }
        
        /// <summary>
        /// 取消准备
        /// </summary>
        /// <param name="userId"></param>
        public void UnReady(int userId)
        {
            ReadyUserList.Remove(userId);
        }

        /// <summary>
        /// 重置位置，给三个玩家排序
        /// </summary>
        /// <param name="myUserId"></param>
        public void ResetPosition(int myUserId)
        {
            RightPlayerId = -1;
            LeftPlayerId = -1;
            if (EnterOrderList.Count == 1) return;

            if(EnterOrderList.Count == 2)
            {
                if(EnterOrderList[0] == myUserId)
                {
                    LeftPlayerId = EnterOrderList[1];
                }
                else
                {
                    RightPlayerId = EnterOrderList[0];
                }
            }

            if(EnterOrderList.Count == 3)
            {
                if (EnterOrderList[0] == myUserId)
                {
                    // b x a b
                    LeftPlayerId = EnterOrderList[1];
                    RightPlayerId = EnterOrderList[2];
                }
                else if(EnterOrderList[1] == myUserId)
                {
                    // a x b
                    LeftPlayerId = EnterOrderList[2];
                    RightPlayerId = EnterOrderList[0];
                }
                else if(EnterOrderList[2] == myUserId)
                {
                    // a b x a
                    LeftPlayerId = EnterOrderList[0];
                    RightPlayerId = EnterOrderList[1];
                }
            }
            
        }

    }
}
