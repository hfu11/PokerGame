using DG.Tweening;
using Protocol.Constant;
using Protocol.DTO;
using Protocol.DTO.Fight;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseManager : MonoBehaviour
{
    /// <summary>
    /// 是否开始下注，启动倒计时
    /// </summary>
    protected bool isStartBet = false;
    protected float _Time = 60f;
    protected float _Timer = 0f;

    public GameObject goCardPrefab;
    protected List<GameObject> goCardList = new List<GameObject>();
    protected float cardPointX;

    public PlayerDTO playerDTO;

    protected Transform cardPoint;
    protected GameObject goTimer;
    protected Text txtTimer;
    protected BetInfoTip betInfoTip;
    protected Text txtBet;
    protected Text txtReadyFold;
    protected Text txtName;
    protected Text txtCoin;
    protected Image imgCoin;
    protected GameObject imgBet;

    protected GameObject imgDealer;
    protected GameObject imgBig;
    protected GameObject imgSmall;

    public bool isLeave = false;
    public bool isFold = false;

    protected TableManager table;

    protected virtual void Awake()
    {
        EventCenter.AddListener(EventType.StartGame, StartGame);
        EventCenter.AddListener<int>(EventType.StartBet, StartBet);
        EventCenter.AddListener<BetDto>(EventType.CallBro, CallBro);
        EventCenter.AddListener<BetDto>(EventType.RaiseBro, RaiseBro);
        EventCenter.AddListener<int>(EventType.FoldBro, FoldBro);
        EventCenter.AddListener<GameOverDTO>(EventType.GameOverBro, GameOverBro);
        EventCenter.AddListener<int>(EventType.LeaveFightRoom, LeaveFightRoom);

    }

    protected virtual void OnDestroy()
    {
        EventCenter.RemoveListener(EventType.StartGame, StartGame);
        EventCenter.RemoveListener<int>(EventType.StartBet, StartBet);
        EventCenter.RemoveListener<BetDto>(EventType.CallBro, CallBro);
        EventCenter.RemoveListener<BetDto>(EventType.RaiseBro, RaiseBro);
        EventCenter.RemoveListener<int>(EventType.FoldBro, FoldBro);
        EventCenter.RemoveListener<GameOverDTO>(EventType.GameOverBro, GameOverBro);
        EventCenter.RemoveListener<int>(EventType.LeaveFightRoom, LeaveFightRoom);
    }

    protected virtual void Init()
    {
        table = GetComponentInParent<TableManager>();

        betInfoTip = transform.Find("txtBetInfoTip").GetComponent<BetInfoTip>();

        txtName = transform.Find("txtName").GetComponent<Text>();
        txtBet = transform.Find("imgBet/txtBet").GetComponent<Text>();
        txtReadyFold = transform.Find("txtReadyFold").GetComponent<Text>();
        cardPoint = transform.Find("cardPoint");
        goTimer = transform.Find("imgTimer").gameObject;
        txtTimer = goTimer.GetComponentInChildren<Text>();
        txtCoin = transform.Find("txtCoin").GetComponent<Text>();
        imgCoin = transform.Find("imgCoin").GetComponent<Image>();
        imgBet = transform.Find("imgBet").gameObject;

        imgSmall = transform.Find("imgSmall").gameObject;
        imgBig = transform.Find("imgBig").gameObject;
        imgDealer = transform.Find("imgDealer").gameObject;
        imgDealer.SetActive(false);
        imgBig.SetActive(false);
        imgSmall.SetActive(false);

        goTimer.SetActive(false);
        txtReadyFold.gameObject.SetActive(false);

        txtBet.text = "0";
        cardPointX = -20f;
    }

    protected virtual void StartGame()
    {
        txtReadyFold.gameObject.SetActive(false);
    }

    protected virtual void StartBet(int userId)
    {
        if (playerDTO.UserId == userId)
        {
            _Time = 60;
            goTimer.SetActive(true);
            txtTimer.text = "60";
            isStartBet = true;
        }
        else
        {
            goTimer.SetActive(false);
            isStartBet = false;
        }
    }
    protected virtual void CallBro(BetDto betDto)
    {
        if(betDto.userId == playerDTO.UserId)
        {
            txtCoin.text = betDto.remainCoin.ToString();
            txtBet.text = betDto.playerBetSum.ToString();
            betInfoTip.Show("Call");
        }
        goTimer.SetActive(false);
        isStartBet = false;
    }
    protected virtual void RaiseBro(BetDto betDto)
    {
        if(betDto.userId == playerDTO.UserId)
        {
            txtCoin.text = betDto.remainCoin.ToString();
            txtBet.text = betDto.playerBetSum.ToString();
            betInfoTip.Show("Raise to " + betDto.playerBetSum);
        }
        goTimer.SetActive(false);
        isStartBet = false;
    }

    protected virtual void FoldBro(int userId)
    {
        if (playerDTO.UserId == userId)
        {
            goTimer.SetActive(false);
            isStartBet = false;
            txtReadyFold.text = "Fold";
            txtReadyFold.gameObject.SetActive(true);
            foreach (var item in goCardList)
            {
                Destroy(item);
            }
            isFold = true;
        }
    }

    protected virtual void LeaveFightRoom(int userId)
    {
        if (playerDTO.UserId == userId)
        {
            goTimer.SetActive(false);
            isStartBet = false;
            txtReadyFold.text = "Leave";
            txtReadyFold.gameObject.SetActive(true);
            foreach (var item in goCardList)
            {
                Destroy(item);
            }
            goCardList.Clear();
            isLeave = true;
        }
    }

    protected virtual void GameOverBro(GameOverDTO gameOverDTO)
    {
        foreach (var item in gameOverDTO.winnerCoinDict)
        {
            if(playerDTO.UserId == item.Key.UserId)
            {
                Models.gameModel.userDTO.coin += item.Value;
                txtCoin.text = Models.gameModel.userDTO.coin.ToString();
            }
        }

        cardPointX = -20f;
        foreach (var item in goCardList)
        {
            Destroy(item);
        }
        goCardList.Clear();
        isFold = false;

        imgBig.SetActive(false);
        imgSmall.SetActive(false);
        imgDealer.SetActive(false);

    }

    protected virtual void FixedUpdate()
    {
        if (isStartBet)
        {
            if (_Time <= 0)
            {
                isStartBet = false;
                _Time = 60;
                _Timer = 0;
                return;
            }
            _Timer += Time.deltaTime;
            if (_Timer >= 1)
            {
                _Timer = 0;
                _Time--;
                txtTimer.text = _Time.ToString();
            }
        }
    }



}
