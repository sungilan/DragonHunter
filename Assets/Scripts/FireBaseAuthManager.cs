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
            // ���ο� PlayerData ����
            PlayerData newPlayerData = new PlayerData(
                nickname,          // �г���
                100,               // �⺻ ü��
                10,                // �⺻ �ּ� ���ݷ�
                20,                // �⺻ �ִ� ���ݷ�
                5,                 // �⺻ ����
                0.1f,              // �⺻ ġ��Ÿ Ȯ�� (10%)
                1.5f,              // �⺻ ġ��Ÿ ����
                new List<Skill>(),               // ��ų ��� (�� ����Ʈ)
                new Dictionary<string, Item>(),  // ��� ���� (�� ��ųʸ�)
                0,                 // �ʱ� ��� ���ݷ�
                0                  // �ʱ� Def
            );
            SaveUserProfile(newPlayerData);
        });
    }
    private void SaveUserProfile(PlayerData playerData)
    {
        Debug.Log("������ ���� ����");
        string jsonData = JsonConvert.SerializeObject(playerData, Formatting.Indented);

        databaseReference.Child("Users").Child(user.UserId).SetRawJsonValueAsync(jsonData)
        .ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("�÷��̾� ������ ���� ���");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("�÷��̾� ������ ���� ����");
                return;
            }

            Debug.Log("�÷��̾� ������ ���� �Ϸ�");
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
    public Dictionary<string, Item> equipSlot = new Dictionary<string, Item>(); // ���
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
