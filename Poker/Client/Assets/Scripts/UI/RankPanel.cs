using DG.Tweening;
using Protocol.DTO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RankPanel : MonoBehaviour
{
    public GameObject goItem;
    private Button btnBack;
    private Transform mParent;

    private void Awake()
    {
        EventCenter.AddListener(EventType.ShowRankPanel, Show);
        EventCenter.AddListener<RankListDTO>(EventType.SendRankListDTO, GetRankListDTO);
        btnBack = transform.Find("btnBack").GetComponent<Button>();
        mParent = transform.Find("List/ScrollRect/Parent");

        btnBack.onClick.AddListener(OnBackButtonClick);
    }

    private void OnDestroy()
    {
        EventCenter.RemoveListener(EventType.ShowRankPanel, Show);
    }

    private void OnBackButtonClick()
    {
        for (int i = 0; i < mParent.childCount; i++)
        {
            Destroy(mParent.GetChild(i).gameObject);
        }
        transform.DOScale(Vector3.zero, 0.3f);
    }

    /// <summary>
    /// 得到排行榜传输模型
    /// </summary>
    private void GetRankListDTO(RankListDTO dto)
    {
        if(dto == null)
        {
            return;
        }

        for (int i = 0; i < dto.rankList.Count; i++)
        {
            GameObject go = Instantiate(goItem, mParent);
            go.transform.Find("Image/txtIndex").GetComponent<Text>().text = (i + 1).ToString();
            go.transform.Find("Image/txtName").GetComponent<Text>().text = dto.rankList[i].username;
            go.transform.Find("Image/txtCoin").GetComponent<Text>().text = dto.rankList[i].coin.ToString();

            if (dto.rankList[i].username == Models.gameModel.userDTO.username)
            {
                go.transform.Find("Image/txtName").GetComponent<Text>().color = Color.red;
                go.transform.Find("Image/txtCoin").GetComponent<Text>().color = Color.red;
                go.transform.Find("Image/txtIndex").GetComponent<Text>().color = Color.red;
            }
        }
        //Show();
    }
    private void Show()
    {
        transform.DOScale(Vector3.one, 0.3f);
    }
}
