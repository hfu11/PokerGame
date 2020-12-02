using Protocol.DTO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class PlayerManager : BaseManager
{

    protected override void Init()
    {
        base.Init();

        SetPlayer(false);
    }

    /// <summary>
    /// 当玩家进来或离开，设置可见性
    /// </summary>
    /// <param name="value"></param>
    protected void SetPlayer(bool value = true)
    {
        txtName.gameObject.SetActive(value);
        txtCoin.gameObject.SetActive(value);
        imgCoin.gameObject.SetActive(value);
        imgBet.SetActive(value);
    }

    protected void LeaveFightRoom(int userId)
    {
        if (userId == playerDTO.UserId)
        {
            SetPlayer(false);
            txtReadyFold.gameObject.SetActive(true);
            txtReadyFold.text = "Left";

            foreach (var item in goCardList)
            {
                Destroy(item);
            }
            isLeave = true;
        }
    }
}
