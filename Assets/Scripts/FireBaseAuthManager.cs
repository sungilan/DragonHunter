using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;
using Firebase.Database;
using System;
using Firebase;
using Firebase.Extensions;
using static UnityEditor.Progress;
using Newtonsoft.Json;

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
            // 새로운 PlayerData 생성
            PlayerData newPlayerData = new PlayerData(
                nickname,          // 닉네임
                100,               // 기본 체력
                10,                // 기본 최소 공격력
                20,                // 기본 최대 공격력
                5,                 // 기본 방어력
                0.1f,              // 기본 치명타 확률 (10%)
                1.5f,              // 기본 치명타 배율
                new List<Skill>(),               // 스킬 목록 (빈 리스트)
                new Dictionary<string, Item>(),  // 장비 슬롯 (빈 딕셔너리)
                0,                 // 초기 장비 공격력
                0                  // 초기 Def
            );
            SaveUserProfile(newPlayerData);
        });
    }
    private void SaveUserProfile(PlayerData playerData)
    {
        Debug.Log("데이터 저장 시작");
        string jsonData = JsonConvert.SerializeObject(playerData, Formatting.Indented);

        databaseReference.Child("Users").Child(user.UserId).SetRawJsonValueAsync(jsonData)
        .ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("플레이어 데이터 저장 취소");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("플레이어 데이터 저장 실패");
                return;
            }

            Debug.Log("플레이어 데이터 저장 완료");
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

            DataManager.Instance.LoadUserProfile();
        });
    }
    public void LogOut()
    {
        auth.SignOut();
    }
}
public class PlayerData
{
    public string nickName;
    public int hp;
    public int minAtk;
    public int maxAtk;
    public int def;
    public float criticalHitChance;
    public float criticalHitMultiplier;
    public List<Skill> skillList;
    public Dictionary<string, Item> equipSlot = new Dictionary<string, Item>(); // 장비
    public int equipAtk;
    public int equipDef;
    public PlayerData(string nickName, int hp, int minAtk, int maxAtk, int def, float criticalHitChance, float criticalHitMultiplier, List<Skill> skillList, Dictionary<string, Item> equipSlot, int equipAtk, int equipDps)
    {
        this.nickName = nickName;
        this.hp = hp;
        this.minAtk = minAtk;
        this.maxAtk = maxAtk;
        this.def = def;
        this.criticalHitChance = criticalHitChance;
        this.criticalHitMultiplier = criticalHitMultiplier;
        this.skillList = skillList;
        this.equipSlot = equipSlot;
        this.equipAtk = equipAtk;
        this.equipDef = equipDps;
    }
}
