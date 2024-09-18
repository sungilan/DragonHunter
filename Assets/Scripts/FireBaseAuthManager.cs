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
    private FirebaseAuth auth; // �α��� / ȸ������ � ���
    private FirebaseUser user; // ������ �Ϸ�� ���� ����
    private DatabaseReference databaseReference; // Firebase Database ����
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

            // FirebaseApp�� �ùٸ��� �ʱ�ȭ�� ���
            FirebaseApp app = FirebaseApp.Create(new AppOptions()
            {
                DatabaseUrl = new Uri("https://test-93975-default-rtdb.firebaseio.com/"),
            });

            // Firebase Auth�� Realtime Database �ʱ�ȭ
            auth = FirebaseAuth.DefaultInstance;
            databaseReference = FirebaseDatabase.DefaultInstance.RootReference;

            if (auth.CurrentUser != null)
            {
                LogOut();
            }

            // ���� ���°� �ٲ� ������ ȣ��Ǵ� �̺�Ʈ �ڵ鷯
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

    public void Create(string email, string password, string nickname)
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
            Debug.Log(user.UserId + "ȸ������ �Ϸ�");
            Debug.Log("�г��� : " + nickname);
            // �г����� Realtime Database�� ����
            SaveUserProfile(nickname);
        });
    }
    private void SaveUserProfile(string nickname)
    {
        // CharacterData ��ü ����
        CharacterData newCharacterData = new CharacterData
        {
            nickName = nickname,
            minAtk = 10,  // �⺻ ��
            maxAtk = 20,  // �⺻ ��
            criticalHitChance = 0.1f,  // �⺻ ��
            criticalHitMultiplier = 1.5f,  // �⺻ ��
            skillList = new List<string> { "Fireball", "IceBlast" },  // �⺻ ��ų
            equipSlot = new Dictionary<string, Item>() // �⺻ ��� ����
        };

        // CharacterData ��ü�� JSON���� ��ȯ (Newtonsoft.Json ���)
        string jsonData = JsonConvert.SerializeObject(newCharacterData, Formatting.Indented);

        // Firebase Database�� ���� (���� ID�� ��������)
        databaseReference.Child("Users").Child(user.UserId).SetRawJsonValueAsync(jsonData)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("ĳ���� ������ ���� ���");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("ĳ���� ������ ���� ����");
                    return;
                }
                Debug.Log("ĳ���� ������ ���� �Ϸ�");
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
            LoginState?.Invoke(true);
        });
    }
    public void LogOut()
    {
        auth.SignOut();
    }
}
