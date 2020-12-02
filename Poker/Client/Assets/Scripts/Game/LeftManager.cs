﻿using DG.Tweening;
using Protocol.Constant;
using Protocol.DTO;
using Protocol.DTO.Fight;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeftManager : PlayerManager
{

    protected override void Awake()
    {
        base.Awake();
        EventCenter.AddListener(EventType.RefreshUI, RefreshUI);
        EventCenter.AddListener<PlayerDTO>(EventType.LeftDealCard, LeftDealCard);

        Init();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        EventCenter.RemoveListener(EventType.RefreshUI, RefreshUI);
        EventCenter.RemoveListener<PlayerDTO>(EventType.LeftDealCard, LeftDealCard);
    }

    private void RefreshUI()
    {
        var room = Models.gameModel.MatchRoomDTO;

        if (room.LeftPlayerId != -1)
        {
            SetPlayer();
            UserDTO userDTO = room.UserModelDict[room.LeftPlayerId];
            txtName.text = userDTO.username;
            txtCoin.text = userDTO.coin.ToString();

            if (room.ReadyUserList.Contains(room.LeftPlayerId))
            {
                txtReadyFold.gameObject.SetActive(true);
            }
            else
            {
                txtReadyFold.gameObject.SetActive(false);
            }
        }
        else
        {
            SetPlayer(false);

            txtReadyFold.gameObject.SetActive(false);
        }
    }

    private void LeftDealCard(PlayerDTO dto)
    {
        goCardList.Clear();

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
            go.GetComponent<RectTransform>().localPosition = new Vector3(cardPointX, 0, 0);
            cardPointX += 40;
            goCardList.Add(go);
        }
    }
}
