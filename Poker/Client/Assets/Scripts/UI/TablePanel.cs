using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class TablePanel : MonoBehaviour
{
    private Button btnTable_1;
    private Button btnTable_2;
    private Button btnTable_3;
    private Button btnBack;

    private void Awake()
    {
        EventCenter.AddListener(EventType.ShowTablePanel, Show);
        Init();
    }

    private void OnDestroy()
    {
        EventCenter.RemoveListener(EventType.ShowTablePanel, Show);
    }

    private void Init()
    {
        btnTable_1 = transform.Find("btnTable_1").GetComponent<Button>();
        btnTable_1.onClick.AddListener(delegate { EnterRoom(10); });
        btnTable_2 = transform.Find("btnTable_2").GetComponent<Button>();
        btnTable_2.onClick.AddListener(delegate { EnterRoom(40); });
        btnTable_3 = transform.Find("btnTable_3").GetComponent<Button>();
        btnTable_3.onClick.AddListener(delegate { EnterRoom(100); });
        btnBack = transform.Find("btnBack").GetComponent<Button>();
        btnBack.onClick.AddListener(OnBackButtonClick);
    } 

    private void Show()
    {
        transform.DOScale(Vector3.one, 0.3f);
    }

    private void OnBackButtonClick()
    {
        transform.DOScale(Vector3.zero, 0.3f);
    }

    private void EnterRoom(int minBet)
    {
        Models.gameModel.minBet = minBet;

        switch (minBet)
        {
            case 10:
                Models.gameModel.RoomType = RoomType.Room_10;
                break;
            case 40:
                Models.gameModel.RoomType = RoomType.Room_40;
                break;
            case 100:
                Models.gameModel.RoomType = RoomType.Room_100;
                break;
            default:
                break;
        }

        SceneManager.LoadScene("3.Online");
    }
}
