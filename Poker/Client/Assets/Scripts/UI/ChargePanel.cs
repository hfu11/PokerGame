using DG.Tweening;
using Protocol.Code;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChargePanel : MonoBehaviour
{
    private GameObject goods;
    private Button[] btnBuyArr;
    private Button btnBack;

    private void Awake()
    {
        EventCenter.AddListener(EventType.ShowChargePanel, Show);
        Init();   
    }

    private void OnDestroy()
    {
        EventCenter.RemoveListener(EventType.ShowChargePanel, Show);
    }

    private void Init()
    {
        goods = transform.Find("goods").gameObject;
        btnBuyArr = new Button[goods.transform.childCount];
        for (int i = 0; i < btnBuyArr.Length; i++)
        {
            btnBuyArr[i] = goods.transform.GetChild(i).GetComponentInChildren<Button>();
        }
        btnBuyArr[0].onClick.AddListener(delegate { Charge(10); });
        btnBuyArr[1].onClick.AddListener(delegate { Charge(20); });
        btnBuyArr[2].onClick.AddListener(delegate { Charge(50); });
        btnBuyArr[3].onClick.AddListener(delegate { Charge(100); });
        btnBuyArr[4].onClick.AddListener(delegate { Charge(500); });
        btnBuyArr[5].onClick.AddListener(delegate { Charge(1000); });

        btnBack = transform.Find("btnBack").GetComponent<Button>();
        btnBack.onClick.AddListener(OnBackButtonClick);
    }

    private void Charge(int count)
    {
        NetMsgCenter.Instance.SendMsg(OpCode.Account, AccountCode.UpdateCoin_CREQ, count);
    }

    private void OnBackButtonClick()
    {
        transform.DOScale(Vector3.zero, 0.3f);
    }
    private void Show()
    {
        transform.DOScale(Vector3.one, 0.3f);
    }
}
