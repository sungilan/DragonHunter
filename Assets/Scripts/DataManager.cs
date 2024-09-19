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
    public float criticalHitChance; // 20% Ȯ���� ġ��Ÿ �߻�
    public float criticalHitMultiplier;
    private int _gold;
    public delegate void GoldUpdated(int newGold);
    public static event GoldUpdated OnGoldUpdated;

    // gold�� get/set ������Ƽ
    public int gold
    {
        get { return _gold; }
        set
        {
            _gold = value;
            // gold�� ����� �� OnGoldUpdated �̺�Ʈ�� �߻���ŵ�ϴ�.
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

            LoadUserProfile(); // �ʱ�ȭ �� ������ �ε�
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
                            // ������ �����͵� �ε��Ͽ� ����
                            criticalHitChance = loadedPlayerData.criticalHitChance;
                            criticalHitMultiplier = loadedPlayerData.criticalHitMultiplier;
                            Debug.Log($"�г���: {loadedPlayerData.nickName}");
                            Debug.Log($"�ּ� ���ݷ�: {loadedPlayerData.minAtk}");
                            Debug.Log($"�ִ� ���ݷ�: {loadedPlayerData.maxAtk}");
                        }
                        else
                        {
                            Debug.LogError("PlayerData ������ȭ ����");
                        }
                    }
                    else
                    {
                        Debug.LogError("�����Ͱ� �������� �ʽ��ϴ�.");
                    }
                }
                else
                {
                    Debug.LogError("PlayerData �ε� ����");
                }
            });
        }
        else
        {
            Debug.LogError("���� �α��ε� ������ �����ϴ�.");
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
