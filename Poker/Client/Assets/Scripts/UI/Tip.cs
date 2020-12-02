using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tip : MonoBehaviour
{
    public float tipShowTime = 2f;

    private Text txtTip;
    private Color color;


    private void Awake()
    {
        EventCenter.AddListener<string>(EventType.ShowTip, Show);
        txtTip = transform.Find("txtTip").GetComponent<Text>();
        //txtTip.CrossFadeAlpha(0f, 0f, true);
        color = txtTip.color;
    }

    private void OnDestroy()
    {
        EventCenter.RemoveListener<string>(EventType.ShowTip, Show);
    }

    private void Show(string str)
    {
        txtTip.text = str;
        //txtTip.CrossFadeAlpha(1f, 1f, true);
        txtTip.DOColor(new Color(color.r, color.g, color.b, 1), 0.3f).OnComplete(() =>
        {
            StartCoroutine(Delay());
        });
        //Invoke("Disappear", 3f);
    }

    IEnumerator Delay()
    {
        yield return new WaitForSeconds(tipShowTime);

        txtTip.DOColor(new Color(color.r, color.g, color.b, 0), 0.3f);

    }

    //private void Disappear()
    //{
    //    txtTip.CrossFadeAlpha(0f, 1f, true);
    //}
}
