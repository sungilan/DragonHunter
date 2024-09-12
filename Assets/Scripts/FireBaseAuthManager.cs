using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;
using System;

public class FireBaseAuthManager : Singleton<FireBaseAuthManager>
{
    private FirebaseAuth auth; // �α��� / ȸ������ � ���
    private FirebaseUser user; // ������ �Ϸ�� ���� ����
    public string UserId => user.UserId;

    public Action<bool> LoginState;
    public void Init()
    {
        auth = FirebaseAuth.DefaultInstance;
        if(auth.CurrentUser != null)
        {
            LogOut();
        }
        auth.StateChanged += OnChanged; // ���� ���°� �ٲ� ������ ȣ��, OnChanged�� ���� ����
    }
    private void OnChanged(object sender, EventArgs e)
    {
        if(auth.CurrentUser != user)
        {
            bool signed = (auth.CurrentUser != user && auth.CurrentUser != null);
            if (!signed && user != null)
            {
                Debug.Log("�α׾ƿ�");
                LoginState?.Invoke(false);
            }
            user = auth.CurrentUser;
            if(signed) 
            {
                Debug.Log("�α���");
                LoginState?.Invoke(true);
            }
        }
    }

    public void Create(string email, string password)
    {
        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("ȸ������ ���");
                return;
            }
            if(task.IsFaulted)
            {
                Debug.LogError("ȸ������ ����");
                return;
            }
            AuthResult authResult = task.Result;
            FirebaseUser user = authResult.User;
            Debug.Log("ȸ������ �Ϸ�");
        });
    }
    public void LogIn(string email, string password)
    {
        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("�α��� ���");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("�α��� ����");
                return;
            }
            AuthResult authResult = task.Result;
            FirebaseUser user = authResult.User;
            Debug.Log("�α��� �Ϸ�");
        });
    }
    public void LogOut()
    {
        auth.SignOut();
    }
}
