using Protocol.Code;
using Protocol.DTO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RegisterPanel : MonoBehaviour
{
    private InputField inputUsername;
    private InputField inputPassword;
    private Button btnBack;
    private Button btnRegister;
    private Button btnShowPwd;

    private void Awake()
    {
        EventCenter.AddListener(EventType.ShowRegisterPanel, Show);
        Init();
        gameObject.SetActive(false);
    }

    private void Init()
    {
        inputUsername = transform.Find("inputUsername").GetComponent<InputField>();
        inputPassword = transform.Find("inputPassword").GetComponent<InputField>();
        btnBack = transform.Find("btnBack").GetComponent<Button>();
        btnRegister = transform.Find("btnRegister").GetComponent<Button>();
        btnShowPwd = transform.Find("btnShowPwd").GetComponent<Button>();

        btnBack.onClick.AddListener(OnBackButtonClick);
        btnRegister.onClick.AddListener(OnRegisterButtonClick);
        btnShowPwd.onClick.AddListener(OnShowPwdButtonClick);
    }

    private void OnDestroy()
    {
        EventCenter.RemoveListener(EventType.ShowRegisterPanel, Show);
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    /// <summary>
    /// 返回按钮点击
    /// </summary>
    private void OnBackButtonClick()
    {
        gameObject.SetActive(false);
        EventCenter.Broadcast(EventType.ShowLoginPanel);
    }

    /// <summary>
    /// 注册按钮点击
    /// </summary>
    private void OnRegisterButtonClick()
    {
        if (inputUsername.text == null || inputUsername.text == "")
        {
            EventCenter.Broadcast<string>(EventType.ShowTip, "Please Enter User Name");
            return;
        }

        if (inputPassword.text == null || inputPassword.text == "")
        {
            EventCenter.Broadcast<string>(EventType.ShowTip, "Please Enter Password");
            return;
        }
        AccountDTO accountDTO = new AccountDTO(inputUsername.text, inputPassword.text);

        //TODO向服务器发送数据，注册用户
        NetMsgCenter.Instance.SendMsg(OpCode.Account, AccountCode.Register_CREQ, accountDTO);
    }

    #region 显示/隐藏密码
    private bool isShowPwd = false;
    /// <summary>
    /// 显示密码
    /// </summary>
    private void OnShowPwdButtonClick()
    {
        isShowPwd = !isShowPwd;
        if (isShowPwd)
        {
            inputPassword.contentType = InputField.ContentType.Standard;
            btnShowPwd.GetComponentInChildren<Text>().text = "Hide";
        }
        else
        {
            inputPassword.contentType = InputField.ContentType.Password;
            btnShowPwd.GetComponentInChildren<Text>().text = "Show";
        }

        EventSystem.current.SetSelectedGameObject(inputPassword.gameObject);
    }
    #endregion

}
