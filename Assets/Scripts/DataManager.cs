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
    //public CharacterController player;
    public string playerNickname;
    private DatabaseReference databaseReference;
    private FirebaseAuth auth;
    public int minAtk = 10;
    public int maxAtk = 20;
    public float criticalHitChance = 0.2f; // 20% Ȯ���� ġ��Ÿ �߻�
    public float criticalHitMultiplier = 1.5f;
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

    private void LoadUserProfile()
    {
        databaseReference = FirebaseDatabase.DefaultInstance.GetReference("Users");
        databaseReference.GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                Debug.Log("�б� �Ϸ�");
                foreach (DataSnapshot data in snapshot.Children)
                {
                    IDictionary Userdata = (IDictionary)data.Value;
                    Debug.Log("�г��� : " + Userdata["NickName"]);
                    playerNickname = Userdata["NickName"].ToString();
                }
            }
        });
    }

    public void LoadData<T>(string path) where T : RawData
    {
        var textAsset = Resources.Load<TextAsset>(path);
        var json = textAsset.text;
        var arrData = JsonConvert.DeserializeObject<T[]>(json);
        foreach (var data in arrData) 
        {
            //this.dicData.Add(data.id, (T)data);
            this.dicData[data.id] = (T)data; //Ű�� �ߺ��Ǹ� �����.
        }
    }
    public T GetData<T>(int key) where T : RawData
    {
        var data = this.dicData[key];
        return (T)data;
    }
}
