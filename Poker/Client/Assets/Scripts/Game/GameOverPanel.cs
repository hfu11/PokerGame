using DG.Tweening;
using Protocol.DTO.Fight;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverPanel : MonoBehaviour
{
    //[System.Serializable]
    //public class Player
    //{
    //    public Text txtName;
    //    public Text txtCoinCount;
    //}
    private GameObject[] winners = new GameObject[3];

    private GameObject winner1;
    private Text txtName1;
    private Text txtCoinCount1;

    private GameObject winner2;
    private Text txtName2;
    private Text txtCoinCount2;

    private GameObject winner3;
    private Text txtName3;
    private Text txtCoinCount3;

    public Button btnAgain;
    public Button btnMain;

    private void Awake()
    {
        EventCenter.AddListener<GameOverDTO>(EventType.GameOverBro, GameOverBro);
        Init();
    }

    private void Init()
    {
        btnAgain.onClick.AddListener(OnButtonAgainClick);
        btnMain.onClick.AddListener(OnButtonMainClick);

        btnAgain.gameObject.SetActive(false);
        btnMain.gameObject.SetActive(false);

        for (int i = 0; i < 3; i++)
        {
            string tmp = "winner" + i;
            winners[i] = transform.Find(tmp).gameObject;
        }

        for (int i = 0; i < 3; i++)
        {
            winners[i].SetActive(false);
        }

        //winner1 = transform.Find("winner1").gameObject;
        //txtName1 = winner1.transform.Find("txtName").GetComponent<Text>();
        //txtCoinCount1 = winner1.transform.Find("txtCoinCount").GetComponent<Text>();

        //winner2 = transform.Find("winner2").gameObject;
        //txtName2 = winner2.transform.Find("txtName").GetComponent<Text>();
        //txtCoinCount2 = winner2.transform.Find("txtCoinCount").GetComponent<Text>();

        //winner3 = transform.Find("winner3").gameObject;
        //txtName3 = winner3.transform.Find("txtName").GetComponent<Text>();
        //txtCoinCount3 = winner3.transform.Find("txtCoinCount").GetComponent<Text>();

        //winner1.SetActive(false);
        //winner2.SetActive(false);
        //winner3.SetActive(false);

    }

    private void OnDestroy()
    {
        EventCenter.RemoveListener<GameOverDTO>(EventType.GameOverBro, GameOverBro);
    }

    private void GameOverBro(GameOverDTO gameOverDTO)
    {
        switch (gameOverDTO.actionType)
        {
            case GameOverDTO.ActionType.Continue:
                transform.DOScale(Vector3.one, 0.3f).OnComplete(() =>
                {
                    StartCoroutine(Delay());
                });
                break;
            case GameOverDTO.ActionType.Over:
                transform.DOScale(Vector3.one, 0.3f);
                btnAgain.gameObject.SetActive(true);
                btnMain.gameObject.SetActive(true);
                break;
            default:
                break;
        }

        var winnerCoinDict = gameOverDTO.winnerCoinDict;

        int i = 0;
        foreach (var item in winnerCoinDict)
        {
            winners[i].SetActive(true);
            winners[i].transform.Find("txtName").GetComponent<Text>().text = item.Key.Username;
            winners[i].transform.Find("txtCoinCount").GetComponent<Text>().text = item.Value.ToString();
        }
    }

    private IEnumerator Delay()
    {
        yield return new WaitForSeconds(2f);
        transform.DOScale(Vector3.zero, 0.3f);
    }

    private void OnButtonAgainClick()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    private void OnButtonMainClick()
    {
        SceneManager.LoadScene("2.Main");
    }
}
