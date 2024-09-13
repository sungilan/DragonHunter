using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;
using Firebase.Database;
using System;
using Firebase;

public class FireBaseAuthManager : Singleton<FireBaseAuthManager>
{
    private FirebaseAuth auth; // 로그인 / 회원가입 등에 사용
    private FirebaseUser user; // 인증이 완료된 유저 정보
    private DatabaseReference databaseReference; // Firebase Database 참조
    public string UserId => user.UserId;

    public Action<bool> LoginState;
    public void Init()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                // Firebase dependencies check failed, handle error
                Debug.LogError("Firebase dependencies check failed.");
                return;
            }

            // FirebaseApp이 올바르게 초기화된 경우
            FirebaseApp app = FirebaseApp.Create(new AppOptions()
            {
                DatabaseUrl = new Uri("https://test-93975-default-rtdb.firebaseio.com/"),
            });

            // Firebase Auth와 Realtime Database 초기화
            auth = FirebaseAuth.DefaultInstance;
            databaseReference = FirebaseDatabase.DefaultInstance.RootReference;

            if (auth.CurrentUser != null)
            {
                LogOut();
            }

            // 계정 상태가 바뀔 때마다 호출되는 이벤트 핸들러
            auth.StateChanged += OnChanged;
        });
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

    public void Create(string email, string password, string nickname)
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
            // 닉네임을 Realtime Database에 저장
            SaveUserProfile(user.UserId, nickname);
        });
    }
    private void SaveUserProfile(string userId, string nickname)
    {
        UserProfile profile = new UserProfile { Nickname = nickname };
        databaseReference.Child("users").Child(userId).SetValueAsync(profile).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("닉네임 저장 취소");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("닉네임 저장 실패");
                return;
            }
            Debug.Log("닉네임 저장 완료");
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
            LoginState?.Invoke(true);
        });
    }
    public void LogOut()
    {
        auth.SignOut();
    }
}
[System.Serializable]
public class UserProfile
{
    public string Nickname;
}
