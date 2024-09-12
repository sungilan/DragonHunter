using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;
using System;

public class FireBaseAuthManager : Singleton<FireBaseAuthManager>
{
    private FirebaseAuth auth; // 로그인 / 회원가입 등에 사용
    private FirebaseUser user; // 인증이 완료된 유저 정보
    public string UserId => user.UserId;

    public Action<bool> LoginState;
    public void Init()
    {
        auth = FirebaseAuth.DefaultInstance;
        if(auth.CurrentUser != null)
        {
            LogOut();
        }
        auth.StateChanged += OnChanged; // 계정 상태가 바뀔 때마다 호출, OnChanged를 만들어서 연결
    }
    private void OnChanged(object sender, EventArgs e)
    {
        if(auth.CurrentUser != user)
        {
            bool signed = (auth.CurrentUser != user && auth.CurrentUser != null);
            if (!signed && user != null)
            {
                Debug.Log("로그아웃");
                LoginState?.Invoke(false);
            }
            user = auth.CurrentUser;
            if(signed) 
            {
                Debug.Log("로그인");
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
                Debug.LogError("회원가입 취소");
                return;
            }
            if(task.IsFaulted)
            {
                Debug.LogError("회원가입 실패");
                return;
            }
            AuthResult authResult = task.Result;
            FirebaseUser user = authResult.User;
            Debug.Log("회원가입 완료");
        });
    }
    public void LogIn(string email, string password)
    {
        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("로그인 취소");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("로그인 실패");
                return;
            }
            AuthResult authResult = task.Result;
            FirebaseUser user = authResult.User;
            Debug.Log("로그인 완료");
        });
    }
    public void LogOut()
    {
        auth.SignOut();
    }
}
