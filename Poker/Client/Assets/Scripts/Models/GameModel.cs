using Protocol.DTO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 游戏数据
/// </summary>
public class GameModel
{
    public UserDTO userDTO { get; set; }

    /// <summary>
    /// minimun bet
    /// </summary>
    public int minBet { get; set; }

    /// <summary>
    /// room type
    /// </summary>
    public RoomType RoomType { get; set; }

    /// <summary>
    /// 匹配房间传输模型
    /// </summary>
    public MatchRoomDTO MatchRoomDTO { get; set; }
}
