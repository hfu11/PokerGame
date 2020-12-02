using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BetInfoTip : MonoBehaviour
{
    public float tipShowTime = 1.5f;

    private Text txtTip;
    private Color color;


    private void Awake()
    {
        txtTip = GetComponent<Text>();
        color = txtTip.color;
    }
    public void Show(string str)
    {
        txtTip.text = str;
        txtTip.DOColor(new Color(color.r, color.g, color.b, 1), 0.3f).OnComplete(() =>
        {
            StartCoroutine(Delay());
        });
    }

    IEnumerator Delay()
    {
        yield return new WaitForSeconds(tipShowTime);

        txtTip.DOColor(new Color(color.r, color.g, color.b, 0), 0.3f);

    }
}
