using Protocol.Code;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TableManager : MonoBehaviour
{
    private Text txtSmall;
    private Text txtBig;
    private Button btnBack;

    private SelfManager self;
    private LeftManager left;
    private RightManager right;

    public bool IsRightFold { get { return right.isFold; } }
    public bool IsRightRun { get { return right.isLeave; } }
    public bool IsLeftFold { get { return left.isFold; } }
    public bool IsLeftRun { get { return left.isLeave; } }


    private void Awake()
    {
        NetMsgCenter.Instance.SendMsg(OpCode.Match, MatchCode.Enter_CREQ, (int)Models.gameModel.RoomType);
        Init();
    }

    private void Init()
    {
        self = GetComponentInChildren<SelfManager>();
        left = GetComponentInChildren<LeftManager>();
        right = GetComponentInChildren<RightManager>();

        txtSmall = transform.Find("Main/txtSmall").GetComponent<Text>();
        txtBig = transform.Find("Main/txtBig").GetComponent<Text>();
        btnBack = transform.Find("Main/btnBack").GetComponent<Button>();

        txtBig.text = Models.gameModel.minBet.ToString();
        txtSmall.text = (Models.gameModel.minBet / 2).ToString();
        btnBack.onClick.AddListener(OnBackButtonClick);

    }

    private void OnBackButtonClick()
    {
        SceneManager.LoadScene("2.Main");

        NetMsgCenter.Instance.SendMsg(OpCode.Match, MatchCode.Leave_CREQ, (int)Models.gameModel.RoomType);
        NetMsgCenter.Instance.SendMsg(OpCode.Fight, FightCode.Leave_CREQ, null);
    }
}
