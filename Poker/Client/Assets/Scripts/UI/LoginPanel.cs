using Protocol.Code;
using Protocol.DTO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginPanel : MonoBehaviour
{
    private InputField inputUsername;
    private InputField inputPassword;
    private Button btnLogin;
    private Button btnRegister;

    private void Awake()
    {
        EventCenter.AddListener(EventType.ShowLoginPanel, Show);
        Init();
    }

    private void Init()
    {
        inputUsername = transform.Find("inputUsername").GetComponent<InputField>();
        inputPassword = transform.Find("inputPassword").GetComponent<InputField>();
        btnLogin = transform.Find("btnLogin").GetComponent<Button>();
        btnRegister = transform.Find("btnRegister").GetComponent<Button>();

        btnLogin.onClick.AddListener(OnLoginButtonClick);
        btnRegister.onClick.AddListener(OnRegisterButtonClick);

    }

    private void OnDestroy()
    {
        EventCenter.RemoveListener(EventType.ShowLoginPanel, Show);
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    /// <summary>
    /// 注册按钮点击
    /// </summary>
    private void OnRegisterButtonClick()
    {
        gameObject.SetActive(false);
        EventCenter.Broadcast(EventType.ShowRegisterPanel);
    }

    private void OnLoginButtonClick()
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

        NetMsgCenter.Instance.SendMsg(OpCode.Account, AccountCode.Login_CREQ, accountDTO);
    }
}
