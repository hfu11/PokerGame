using DG.Tweening;
using Protocol.Code;
using Protocol.Constant;
using Protocol.DTO;
using Protocol.DTO.Fight;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelfManager : BaseManager
{
    private GameObject goBottom;
    private Button btnCall;
    private Button btnRaise;
    private Button btnFold;
    private Toggle tog_2;
    private Toggle tog_5;
    private Toggle tog_10;

    private Button btnReady;
    private Button btnUnReady;

    protected List<GameObject> goCommunityList = new List<GameObject>();
    protected float communityPointX = -200f;
    private Transform communityPoint;


    protected override void Awake()
    {
        base.Awake();

        EventCenter.AddListener<PlayerDTO>(EventType.SelfDealCard, SelfDealCard);

        EventCenter.AddListener(EventType.FlopBro, FlopBro);
        EventCenter.AddListener(EventType.TurnBro, TurnBro);
        EventCenter.AddListener(EventType.RiverBro, RiverBro);

        Init();

    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        EventCenter.RemoveListener<PlayerDTO>(EventType.SelfDealCard, SelfDealCard);

        EventCenter.RemoveListener(EventType.FlopBro, FlopBro);
        EventCenter.RemoveListener(EventType.TurnBro, TurnBro);
        EventCenter.RemoveListener(EventType.RiverBro, RiverBro);

    }


    protected override void Init()
    {
        base.Init();

        communityPoint = transform.Find("communityPoint");

        goBottom = transform.Find("Bottom").gameObject;
        btnReady = transform.Find("btnReady").GetComponent<Button>();
        btnUnReady = transform.Find("btnUnReady").GetComponent<Button>();
        btnReady.onClick.AddListener(OnReadyButtonClick);
        btnUnReady.onClick.AddListener(OnUnReadyButtonClick);


        btnCall = goBottom.transform.Find("btnCall").GetComponent<Button>();
        btnCall.onClick.AddListener(OnCallButtonClick);

        btnRaise = goBottom.transform.Find("btnRaise").GetComponent<Button>();
        btnRaise.onClick.AddListener(OnRaiseButtonClick);


        btnFold = goBottom.transform.Find("btnFold").GetComponent<Button>();
        btnFold.onClick.AddListener(OnFoldButtonClick);

        tog_2 = goBottom.transform.Find("tog_2").GetComponent<Toggle>();
        tog_5 = goBottom.transform.Find("tog_5").GetComponent<Toggle>();
        tog_10 = goBottom.transform.Find("tog_10").GetComponent<Toggle>();

        btnUnReady.gameObject.SetActive(false);

        goBottom.SetActive(false);

        txtName.text = Models.gameModel.userDTO.username;
        txtCoin.text = Models.gameModel.userDTO.coin.ToString();
    }

    protected override void GameOverBro(GameOverDTO gameOverDTO)
    {
        base.GameOverBro(gameOverDTO);

        communityPointX = -200f;
        foreach (var item in goCommunityList)
        {
            Destroy(item);
        }
    }

    private void FlopBro()
    {
        goCommunityList[0].GetComponent<Image>().sprite = ResourcesManager.LoadCardSprite(playerDTO.communityList[0].cardName);
        goCommunityList[1].GetComponent<Image>().sprite = ResourcesManager.LoadCardSprite(playerDTO.communityList[1].cardName);
        goCommunityList[2].GetComponent<Image>().sprite = ResourcesManager.LoadCardSprite(playerDTO.communityList[2].cardName);
    }
    private void TurnBro()
    {
        goCommunityList[3].GetComponent<Image>().sprite = ResourcesManager.LoadCardSprite(playerDTO.communityList[3].cardName);
    }
    private void RiverBro()
    {
        goCommunityList[4].GetComponent<Image>().sprite = ResourcesManager.LoadCardSprite(playerDTO.communityList[4].cardName);
    }

    private void SelfDealCard(PlayerDTO dto)
    {
        playerDTO = dto;

        txtBet.text = playerDTO.BetSum.ToString();

        switch (dto.identity)
        {
            case Identity.Small:
                imgSmall.SetActive(true);
                break;
            case Identity.Big:
                imgBig.SetActive(true);
                break;
            case Identity.Dealer:
                imgDealer.SetActive(true);
                break;
            default:
                break;
        }
        goCardList.Clear();
        foreach (var item in dto.cardList)
        {
            GameObject go = Instantiate(goCardPrefab, cardPoint);
            go.GetComponent<Image>().sprite = ResourcesManager.LoadCardSprite(item.cardName);
            go.GetComponent<RectTransform>().localPosition = new Vector3(cardPointX, 0, 0);
            cardPointX += 40;
            goCardList.Add(go);
        }

        goCommunityList.Clear();
        //create community
        foreach (var item in dto.communityList)
        {
            GameObject go = Instantiate(goCardPrefab, communityPoint);
            go.GetComponent<RectTransform>().localPosition = new Vector3(communityPointX, 0, 0);
            communityPointX += 100;
            goCommunityList.Add(go);
        }

        goBottom.SetActive(true);
        SetBottomButtonInteractable(false);

    }

    protected override void FoldBro(int userId)
    {
        base.FoldBro(userId);

        if(playerDTO.UserId == userId)
        {
            goBottom.SetActive(false);
        }
    }

    protected override void CallBro(BetDto betDto)
    {
        base.CallBro(betDto);

        SetBottomButtonInteractable(false);
    }

    protected override void RaiseBro(BetDto betDto)
    {
        base.RaiseBro(betDto);

        SetBottomButtonInteractable(false);
    }

    protected override void StartGame()
    {
        base.StartGame();

        btnUnReady.gameObject.SetActive(false);
    }

    protected override void StartBet(int userId)
    {
        base.StartBet(userId);

        if (Models.gameModel.userDTO.userId == userId)
        {
            SetBottomButtonInteractable(true);
        }
        else
        {
            SetBottomButtonInteractable(false);
        }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (tog_2.isOn)
        {
            tog_2.transform.Find("Background").GetComponent<Image>().color = Color.gray;
            tog_5.transform.Find("Background").GetComponent<Image>().color = Color.white;
            tog_10.transform.Find("Background").GetComponent<Image>().color = Color.white;
        }
        if (tog_5.isOn)
        {
            tog_5.transform.Find("Background").GetComponent<Image>().color = Color.gray;
            tog_2.transform.Find("Background").GetComponent<Image>().color = Color.white;
            tog_10.transform.Find("Background").GetComponent<Image>().color = Color.white;
        }
        if (tog_10.isOn)
        {
            tog_10.transform.Find("Background").GetComponent<Image>().color = Color.gray;
            tog_5.transform.Find("Background").GetComponent<Image>().color = Color.white;
            tog_2.transform.Find("Background").GetComponent<Image>().color = Color.white;
        }
    }

    /// <summary>
    /// 取消准备按钮点击
    /// </summary>
    private void OnUnReadyButtonClick()
    {
        btnUnReady.gameObject.SetActive(false);
        btnReady.gameObject.SetActive(true);
        txtReadyFold.gameObject.SetActive(false);
        NetMsgCenter.Instance.SendMsg(OpCode.Match, MatchCode.UnReady_CREQ, (int)Models.gameModel.RoomType);

    }

    /// <summary>
    /// 跟牌的处理
    /// </summary>
    private void OnCallButtonClick()
    {
        NetMsgCenter.Instance.SendMsg(OpCode.Fight, FightCode.Call_CREQ, null);
    }
    /// <summary>
    /// 点击加注
    /// </summary>
    private void OnRaiseButtonClick()
    {
        if (tog_2.isOn)
        {
            NetMsgCenter.Instance.SendMsg(OpCode.Fight, FightCode.RAISE_CREQ, 2);
        }
        if (tog_5.isOn)
        {
            NetMsgCenter.Instance.SendMsg(OpCode.Fight, FightCode.RAISE_CREQ, 5);
        }
        if (tog_10.isOn)
        {
            NetMsgCenter.Instance.SendMsg(OpCode.Fight, FightCode.RAISE_CREQ, 10);
        }
    }


    /// <summary>
    /// 点击弃牌
    /// </summary>
    private void OnFoldButtonClick()
    {
        NetMsgCenter.Instance.SendMsg(OpCode.Fight, FightCode.FOLD_CREQ, null);
    }


    /// <summary>
    /// 准备按钮点击
    /// </summary>
    private void OnReadyButtonClick()
    {
        btnUnReady.gameObject.SetActive(true);

        btnReady.gameObject.SetActive(false);
        txtReadyFold.gameObject.SetActive(true);
        txtReadyFold.text = "Ready";
        NetMsgCenter.Instance.SendMsg(OpCode.Match, MatchCode.Ready_CREQ, (int)Models.gameModel.RoomType);
    }


    /// <summary>
    /// 设置底部按钮是否可以交互
    /// </summary>
    /// <param name="value"></param>
    private void SetBottomButtonInteractable(bool value)
    {
        btnCall.interactable = value;
        btnRaise.interactable = value;
        tog_2.interactable = value;
        tog_5.interactable = value;
        tog_10.interactable = value;
    }

}
