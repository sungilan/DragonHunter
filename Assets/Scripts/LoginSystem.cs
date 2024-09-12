using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginSystem : MonoBehaviour
{
    public TMP_InputField email;
    public TMP_InputField password;
    public TextMeshProUGUI outputText;

    void Start()
    {
        FireBaseAuthManager.Instance.LoginState += OnChangedState;
        FireBaseAuthManager.Instance.Init();
    }
    private void OnChangedState(bool sign)
    {
        outputText.text = sign ? "로그인 : " : "로그아웃 : ";
        outputText.text += FireBaseAuthManager.Instance.UserId;
    }

    public void Create()
    {
        string e = email.text;
        string p = password.text;
        FireBaseAuthManager.Instance.Create(e, p);
    }
    public void LogIn()
    {
        FireBaseAuthManager.Instance.LogIn(email.text, password.text);
    }
    public void LogOut()
    {
        FireBaseAuthManager.Instance.LogOut();
    }
}
