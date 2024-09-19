using System.Collections;
using TMPro;
using UnityEngine;
using Firebase.Database;
using Firebase.Auth;
using System;
using Firebase;
using Newtonsoft.Json;
using System.Collections.Generic;

public class DataManager : Singleton<DataManager>
{
    public string nickname;
    private DatabaseReference databaseReference;
    private FirebaseAuth auth;
    public int hp;
    public int minAtk;
    public int maxAtk;
    public int def;
    public float criticalHitChance; // 20% 확률로 치명타 발생
    public float criticalHitMultiplier;
    private int _gold;
    public delegate void GoldUpdated(int newGold);
    public static event GoldUpdated OnGoldUpdated;

    // gold의 get/set 프로퍼티
    public int gold
    {
        get { return _gold; }
        set
        {
            _gold = value;
            // gold가 변경될 때 OnGoldUpdated 이벤트를 발생시킵니다.
            OnGoldUpdated?.Invoke(_gold);
        }
    }

    private Dictionary<int, RawData> dicData = new Dictionary<int, RawData>();

    private void Start()
    {
        _gold = 1000000;
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError("Firebase dependencies check failed.");
                return;
            }

            FirebaseApp app = FirebaseApp.Create(new AppOptions()
            {
                DatabaseUrl = new Uri("https://test-93975-default-rtdb.firebaseio.com/"),
            });

            auth = FirebaseAuth.DefaultInstance;
            //databaseReference = FirebaseDatabase.DefaultInstance.RootReference;

            LoadUserProfile(); // 초기화 후 데이터 로드
        });
    }

    public void LoadUserProfile()
    {
        var currentUser = auth.CurrentUser;
        if (currentUser != null)
        {
            databaseReference = FirebaseDatabase.DefaultInstance.GetReference("Users").Child(currentUser.UserId);
            databaseReference.GetValueAsync().ContinueWith(task =>
            {
                if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    if (snapshot.Exists)
                    {
                        string jsonData = snapshot.GetRawJsonValue();
                        PlayerData loadedPlayerData = JsonConvert.DeserializeObject<PlayerData>(jsonData);

                        if (loadedPlayerData != null)
                        {
                            nickname = loadedPlayerData.nickName;
                            hp = loadedPlayerData.hp;
                            minAtk = loadedPlayerData.minAtk;
                            maxAtk = loadedPlayerData.maxAtk;
                            def = loadedPlayerData.def;
                            // 나머지 데이터도 로드하여 적용
                            criticalHitChance = loadedPlayerData.criticalHitChance;
                            criticalHitMultiplier = loadedPlayerData.criticalHitMultiplier;
                            Debug.Log($"닉네임: {loadedPlayerData.nickName}");
                            Debug.Log($"최소 공격력: {loadedPlayerData.minAtk}");
                            Debug.Log($"최대 공격력: {loadedPlayerData.maxAtk}");
                        }
                        else
                        {
                            Debug.LogError("PlayerData 역직렬화 실패");
                        }
                    }
                    else
                    {
                        Debug.LogError("데이터가 존재하지 않습니다.");
                    }
                }
                else
                {
                    Debug.LogError("PlayerData 로드 실패");
                }
            });
        }
        else
        {
            Debug.LogError("현재 로그인된 유저가 없습니다.");
        }
    }



    public void LoadData<T>(string path) where T : RawData
    {
        var textAsset = Resources.Load<TextAsset>(path);
        var json = textAsset.text;
        var arrData = JsonConvert.DeserializeObject<T[]>(json);
        foreach (var data in arrData) 
        {
            this.dicData[data.id] = (T)data;
        }
    }
    public T GetData<T>(int key) where T : RawData
    {
        var data = this.dicData[key];
        return (T)data;
    }
}
