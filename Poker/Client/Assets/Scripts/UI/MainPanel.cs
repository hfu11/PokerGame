using Protocol.Code;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainPanel : MonoBehaviour
{
    private Text txtName;
    private Text txtCoin;
    private Button btnRank;
    private Button btnCharge;
    private Button btnStartOnline;

    private void Awake()
    {
        EventCenter.AddListener<int>(EventType.UpdateCoin, UpdateCoin);
        Init();
    }

    private void OnDestroy()
    {
        EventCenter.RemoveListener<int>(EventType.UpdateCoin, UpdateCoin);
    }

    private void Init()
    {
        txtName = transform.Find("txtName").GetComponent<Text>();
        txtCoin = transform.Find("txtCoin").GetComponent<Text>();
        btnRank = transform.Find("btnRank").GetComponent<Button>();
        btnRank.onClick.AddListener(()=> {
            //向服务器发送获取排行榜的请求
            NetMsgCenter.Instance.SendMsg(OpCode.Account, AccountCode.GetRankList_CREQ, null);
            EventCenter.Broadcast(EventType.ShowRankPanel);
        });
        btnCharge = transform.Find("btnCharge").GetComponent<Button>();
        btnCharge.onClick.AddListener(() =>
        {
            EventCenter.Broadcast(EventType.ShowChargePanel);
        });
        btnStartOnline = transform.Find("btnStartOnline").GetComponent<Button>();
        btnStartOnline.onClick.AddListener(() =>
        {
            EventCenter.Broadcast(EventType.ShowTablePanel);
        });

        txtCoin.text = Models.gameModel.userDTO.coin.ToString();
        txtName.text = Models.gameModel.userDTO.username;

    }

    private void UpdateCoin(int coin)
    {
        txtCoin.text = coin.ToString();
    }

}
