using Protocol.Code;
using Protocol.DTO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AccountHandler : BaseHandler
{
    public override void OnReceive(int subCode, object value)
    {
        switch (subCode)
        {
            case AccountCode.Register_SRES:
                Register_SRES((int)value);
                break;
            case AccountCode.Login_SRES:
                Login_SRES((int)value);
                break;
            case AccountCode.GetUserInfo_SRES:
                GetUserInfo_SRES(value as UserDTO);
                break;
            case AccountCode.GetRankList_SRES:
                EventCenter.Broadcast(EventType.SendRankListDTO, value as RankListDTO);
                break;
            case AccountCode.UpdateCoin_SRES:
                UpdateCoin_SRES((int)value);
                break;
            default:
                break;
        }
    }

    private void UpdateCoin_SRES(int coin)
    {
        if(coin == -1)
        {
            return;
        }

        Models.gameModel.userDTO.coin = coin;
        EventCenter.Broadcast<int>(EventType.UpdateCoin, coin);
        EventCenter.Broadcast<string>(EventType.ShowTip, "Coin: "+ coin);

    }

    private void GetUserInfo_SRES(UserDTO userDTO)
    {
        if(userDTO == null)
        {
            //EventCenter.Broadcast<string>(EventType.ShowTip, "Try Later");
            Debug.Log("userDTO Not Found");
        }

        Models.gameModel.userDTO = userDTO;
        //跳转至主场景
        SceneManager.LoadScene("2.Main");


    }

    private void Login_SRES(int value)
    {
        if(value == -1)
        {
            EventCenter.Broadcast<string>(EventType.ShowTip, "User Name Not Exist");
        }
        else if(value == -2)
        {
            EventCenter.Broadcast<string>(EventType.ShowTip, "Incorrect Password");
        }
        else if(value == -3)
        {
            EventCenter.Broadcast<string>(EventType.ShowTip, "Account Is Online");
        }
        else if(value == 0)
        {
            NetMsgCenter.Instance.SendMsg(OpCode.Account, AccountCode.GetUserInfo_CREQ, null);
            EventCenter.Broadcast<string>(EventType.ShowTip, "Login Success");
        }
    }

    private void Register_SRES(int value)
    {
        if(value == -1)
        {
            EventCenter.Broadcast<string>(EventType.ShowTip, "User Name Exist");
        }
        else if(value == 0)
        {
            EventCenter.Broadcast<string>(EventType.ShowTip, "Success");
            EventCenter.Broadcast(EventType.ShowLoginPanel);
        }
    }
}

