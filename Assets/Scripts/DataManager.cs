using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Firebase.Database;
using Firebase.Auth;
using System.Threading.Tasks;
using Firebase.Extensions;

public class DataManager : Singleton<DataManager>
{
    public TPSCharacterController player;
    [SerializeField] private TextMeshProUGUI playerNickname;
    private DatabaseReference databaseReference;
    private FirebaseAuth auth;

    void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
        LoadUserProfile();
    }

    private void LoadUserProfile()
    {
        FirebaseUser user = auth.CurrentUser;
        if (user != null)
        {
            string userId = user.UserId;
            databaseReference.Child("users").Child(userId).GetValueAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError("Failed to retrieve user profile.");
                    return;
                }

                DataSnapshot snapshot = task.Result;
                if (snapshot.Exists)
                {
                    UserProfile profile = JsonUtility.FromJson<UserProfile>(snapshot.GetRawJsonValue());
                    playerNickname.text = profile.Nickname;
                }
            });
        }
    }
}
