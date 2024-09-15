using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Firebase.Database;
using Firebase.Auth;
using System;
using Firebase;

public class DataManager : Singleton<DataManager>
{
    public CharacterController player;
    [SerializeField] private string playerNickname;
    private DatabaseReference databaseReference;
    private FirebaseAuth auth;
    public int minAtk = 10;
    public int maxAtk = 20;
    public float criticalHitChance = 0.2f; // 20% 확률로 치명타 발생
    public float criticalHitMultiplier = 1.5f;

    [SerializeField] private TextMeshProUGUI nicknameText;

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

            LoadUserProfile(); // 초기화 후 데이터 로드
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
                Debug.Log("읽기 완료");
                foreach (DataSnapshot data in snapshot.Children)
                {
                    IDictionary Userdata = (IDictionary)data.Value;
                    Debug.Log("닉네임 : " + Userdata["NickName"]);
                    playerNickname = Userdata["NickName"].ToString();
                    nicknameText.text = playerNickname;
                }
            }
        });
    }
    private void Update()
    {
        //nicknameText.text = playerNickname;
    }
}
