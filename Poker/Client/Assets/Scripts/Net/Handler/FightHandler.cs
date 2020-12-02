using Protocol.Code;
using Protocol.Constant;
using Protocol.DTO;
using Protocol.DTO.Fight;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightHandler : BaseHandler
{
    public override void OnReceive(int subCode, object value)
    {
        switch (subCode)
        {
            case FightCode.StartFight_BRO:
                StartFight_BRO(value as List<PlayerDTO>);
                break;
            case FightCode.Leave_BRO:
                EventCenter.Broadcast<int>(EventType.LeaveFightRoom, (int)value);
                break;
            case FightCode.StartBet_BRO:
                EventCenter.Broadcast<int>(EventType.StartBet, (int)value);
                break;
            case FightCode.Call_BRO:
                EventCenter.Broadcast<BetDto>(EventType.CallBro, value as BetDto);
                break;
            case FightCode.Raise_BRO:
                EventCenter.Broadcast<BetDto>(EventType.RaiseBro, value as BetDto);
                break;
            case FightCode.FOLD_BRO:
                EventCenter.Broadcast<int>(EventType.FoldBro, (int)value);
                break;
            case FightCode.Flop_BRO:
                EventCenter.Broadcast(EventType.FlopBro);
                break;
            case FightCode.Turn_BRO:
                EventCenter.Broadcast(EventType.TurnBro);
                break;
            case FightCode.River_BRO:
                EventCenter.Broadcast(EventType.RiverBro);
                break;
            case FightCode.GameOver_BRO:
                EventCenter.Broadcast<GameOverDTO>(EventType.GameOverBro, value as GameOverDTO);
                break;
            case FightCode.Flush_BRO:
                EventCenter.Broadcast(EventType.FlushBro);
                break;
            default:
                break;
        }
    }

    private void StartFight_BRO(List<PlayerDTO> playerList)
    {
        foreach (var item in playerList)
        {
            if(item.UserId == Models.gameModel.MatchRoomDTO.LeftPlayerId)
            {
                EventCenter.Broadcast<PlayerDTO>(EventType.LeftDealCard, item);
            }
            else if(item.UserId == Models.gameModel.MatchRoomDTO.RightPlayerId)
            {
                EventCenter.Broadcast<PlayerDTO>(EventType.RightDealCard, item);
            }
            else
            {
                EventCenter.Broadcast<PlayerDTO>(EventType.SelfDealCard, item);
            }
        }
    }
}
