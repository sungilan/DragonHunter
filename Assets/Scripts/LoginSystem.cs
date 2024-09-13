using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginSystem : MonoBehaviour
{
    public TMP_InputField email;
    public TMP_InputField password;
    public TMP_InputField nickname;
    public TextMeshProUGUI outputText;

    void Start()
    {
        FireBaseAuthManager.Instance.LoginState += OnChangedState;
        FireBaseAuthManager.Instance.Init();
    }
    private void OnChangedState(bool sign)
    {
        if (sign)
        {
            // 로그인 성공 시
            outputText.text = "LogIn : " + FireBaseAuthManager.Instance.UserId;
            SceneManager.LoadScene("Main"); // 로그인 성공 후 'Main' 씬으로 이동
        }
        else
        {
            // 로그아웃 시
            outputText.text = "LogOut : " + FireBaseAuthManager.Instance.UserId;
        }
    }

    public void Create()
    {
        string e = email.text;
        string p = password.text;
        string n = nickname.text;
        FireBaseAuthManager.Instance.Create(e, p, n);
    }
    public void LogIn()
    {
        FireBaseAuthManager.Instance.LogIn(email.text, password.text);
    }
    public void LogOut()
    {
        FireBaseAuthManager.Instance.LogOut();
    }
    private void Update()
    {
        if (email.isFocused == true)
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                password.Select();
            }
        }
    }
}
