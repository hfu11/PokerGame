using Protocol.Code;
using Protocol.DTO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchHandler : BaseHandler
{
    public override void OnReceive(int subCode, object value)
    {
        switch (subCode)
        {
            case MatchCode.Enter_SRES:
                Enter_SRES(value as MatchRoomDTO);
                break;
            case MatchCode.Enter_BRO:
                Enter_BRO(value as UserDTO);
                break;
            case MatchCode.Leave_BRO:
                Leave_BRO((int)value);
                break;
            case MatchCode.Ready_BRO:
                Ready_BRO((int)value);
                break;
            case MatchCode.UnReady_BRO:
                UnReady_BRO((int)value);
                break;
            case MatchCode.StartGame_BRO:
                StartGame_BRO();
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 开始游戏的广播
    /// </summary>
    private void StartGame_BRO()
    {
        EventCenter.Broadcast<string>(EventType.ShowTip, "Game Start");
        EventCenter.Broadcast(EventType.StartGame);
    }

    /// <summary>
    /// 有玩家取消准备的广播
    /// </summary>
    /// <param name="userId"></param>
    private void UnReady_BRO(int userId)
    {
        Models.gameModel.MatchRoomDTO.UnReady(userId);
        EventCenter.Broadcast(EventType.RefreshUI);
    }

    /// <summary>
    /// 有玩家准备的广播
    /// </summary>
    private void Ready_BRO(int userId)
    {
        Models.gameModel.MatchRoomDTO.Ready(userId);
        EventCenter.Broadcast(EventType.RefreshUI);
    }

    /// <summary>
    /// 有玩家离开服务器的广播
    /// </summary>
    /// <param name="userId"></param>
    private void Leave_BRO(int userId)
    {
        Models.gameModel.MatchRoomDTO.Leave(userId);
        ResetPosition();
        EventCenter.Broadcast(EventType.RefreshUI);
    }

    /// <summary>
    /// 客户端请求进入房间服务器的响应
    /// </summary>
    /// <param name="matchRoomDTO"></param>
    private void Enter_SRES(MatchRoomDTO roomDTO)
    {
        Models.gameModel.MatchRoomDTO = roomDTO;
        ResetPosition();
        //刷新UI
        EventCenter.Broadcast(EventType.RefreshUI);
    }


    /// <summary>
    /// 客户端进入服务器的广播
    /// </summary>
    /// <param name="userDTO"></param>
    private void Enter_BRO(UserDTO userDTO)
    {
        Models.gameModel.MatchRoomDTO.Enter(userDTO);
        ResetPosition();
        //todo
        EventCenter.Broadcast(EventType.RefreshUI);
    }    
    /// <summary>
    /// 给房间内的玩家重排序
    /// </summary>
    private void ResetPosition()
    {
        MatchRoomDTO dto = Models.gameModel.MatchRoomDTO;
        dto.ResetPosition(Models.gameModel.userDTO.userId);
    }

}
