using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Database;

public static class Authentication
{
    private static readonly string sessionIdTag = "sessionId";

    private static DatabaseReference databaseReference = null;

    public static void Initialize()
    {
        DependencyStatus dependencyStatus=DependencyStatus.UnavailableInvalid;
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
            }
        });
    }

    public static void Sign(string email, string password, bool creation)
    {
        FirebaseAuth.DefaultInstance.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                Debug.LogError("회원가입 실패: " + task.Exception?.Flatten().Message);
                return;
            }
        });
    }

    public static void SignOut()
    {
        if(databaseReference != null)
        {

        }
    }
}