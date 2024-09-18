using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;
using Firebase.Database;
using System;
using Firebase;
using Firebase.Extensions;
using Newtonsoft.Json;
using static UnityEditor.Progress;

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
            Debug.Log(user.UserId + "회원가입 완료");
            Debug.Log("닉네임 : " + nickname);
            // 닉네임을 Realtime Database에 저장
            SaveUserProfile(nickname);
        });
    }
    private void SaveUserProfile(string nickname)
    {
        // CharacterData 객체 생성
        CharacterData newCharacterData = new CharacterData
        {
            nickName = nickname,
            minAtk = 10,  // 기본 값
            maxAtk = 20,  // 기본 값
            criticalHitChance = 0.1f,  // 기본 값
            criticalHitMultiplier = 1.5f,  // 기본 값
            skillList = new List<string> { "Fireball", "IceBlast" },  // 기본 스킬
            equipSlot = new Dictionary<string, Item>() // 기본 장비 슬롯
        };

        // CharacterData 객체를 JSON으로 변환 (Newtonsoft.Json 사용)
        string jsonData = JsonConvert.SerializeObject(newCharacterData, Formatting.Indented);

        // Firebase Database에 저장 (유저 ID를 기준으로)
        databaseReference.Child("Users").Child(user.UserId).SetRawJsonValueAsync(jsonData)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("캐릭터 데이터 저장 취소");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("캐릭터 데이터 저장 실패");
                    return;
                }
                Debug.Log("캐릭터 데이터 저장 완료");
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
