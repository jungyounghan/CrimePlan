using System;
using System.Collections.Generic;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;

public static class Authentication
{
    public enum State: byte
    {
        EmptyAccount,
        SignUpFailure,
        SignUpAlready,
        SignUpSuccess,
        SignInFailure,
        SignInAlready,
        SignInInvalidEmail,
        SignInSuccess,
    }

    [Serializable]
    public class LoginInfo
    {
        public string uid;
        public long timestamp;
    }

    private static readonly string UsersTag = "Users";

    private static FirebaseAuth firebaseAuth = null;
    private static DatabaseReference databaseReference = null;

    public static void Initialize(Action<DependencyStatus> action = null)
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            DependencyStatus dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                firebaseAuth = FirebaseAuth.DefaultInstance;
            }
            action?.Invoke(dependencyStatus);
        });
    }

    public static void Sign(string identification, string password, bool creation, Action<State> action = null)
    {
        if (firebaseAuth != null)
        {
            if (creation == true)
            {
                firebaseAuth.CreateUserWithEmailAndPasswordAsync(identification, password).ContinueWithOnMainThread(task =>
                {
                    if (task.IsFaulted == true || task.IsCanceled == true)
                    {
                        foreach (Exception exception in task.Exception.Flatten().InnerExceptions)
                        {
                            if (exception is FirebaseException firebaseException)
                            {
                                AuthError authError = (AuthError)firebaseException.ErrorCode;
                                switch (authError)
                                {
                                    case AuthError.EmailAlreadyInUse:
                                        action?.Invoke(State.SignUpAlready);
                                        return;
                                }
                            }
                        }
                        action?.Invoke(State.SignUpFailure);
                    }
                    else
                    {
                        action?.Invoke(State.SignUpSuccess);
                    }
                });
            }
            else
            {
                firebaseAuth.SignInWithEmailAndPasswordAsync(identification, password).ContinueWithOnMainThread(task =>
                {
                    if (task.IsFaulted == true || task.IsCanceled == true)
                    {
                        foreach (Exception exception in task.Exception.Flatten().InnerExceptions)
                        {
                            if (exception is FirebaseException firebaseEx)
                            {
                                AuthError authError = (AuthError)firebaseEx.ErrorCode;
                                switch (authError)
                                {
                                    case AuthError.InvalidEmail:
                                        action?.Invoke(State.SignInInvalidEmail);
                                        return;
                                }
                            }
                        }
                        action?.Invoke(State.SignInFailure);
                    }
                    else
                    {
                        FirebaseUser firebaseUser = task.Result.User;
                        if (firebaseUser == null)
                        {
                            action?.Invoke(State.SignInFailure);
                        }
                        else
                        {
                            databaseReference = FirebaseDatabase.DefaultInstance.RootReference.Child(UsersTag).Child(firebaseUser.UserId);

                            long now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

                            databaseReference.RunTransaction(mutableData =>
                            {
                                if (mutableData.Value == null)
                                {
                                    mutableData.Value = new Dictionary<string, object> {
            { "uid", firebaseUser.UserId },
            { "timestamp", now }
        };
                                    return TransactionResult.Success(mutableData);
                                }
                                else
                                {
                                    return TransactionResult.Abort(); // 이미 로그인 중
                                }
                            }).ContinueWithOnMainThread(task =>
                            {
                                if (task.IsCanceled || task.IsFaulted)
                                {
                                    UnityEngine.Debug.LogError("트랜잭션 실패");
                                    firebaseAuth.SignOut();
                                    firebaseUser = null;
                                    action?.Invoke(State.SignInAlready);
                                    return;
                                }

                                DataSnapshot result = task.Result;

                                if (result.Value == null)
                                {
                                    firebaseAuth.SignOut();
                                    firebaseUser = null;
                                    action?.Invoke(State.SignInAlready);
                                    return;
                                }

                                UnityEngine.Debug.Log("로그인 성공");
                                databaseReference.OnDisconnect().RemoveValue();
                                action?.Invoke(State.SignInSuccess);
                            });
                        }
                    }
                });
            }
        }
        else
        {
            action?.Invoke(State.EmptyAccount);
        }
    }

    public static void SignOut()
    {
        if (firebaseAuth != null)
        {
            firebaseAuth.SignOut();
            firebaseAuth = null;
        }
    }
}