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

    private static readonly string UsersTag = "Users";
    private static readonly string SessionTag = "Session";
    private static readonly string TokenTag = "Token";
    private static readonly string TimestampTag = "Timestamp";

    private static FirebaseAuth firebaseAuth = null;
    private static DatabaseReference databaseReference = null;
    private static EventHandler<ValueChangedEventArgs> sessionListener = null;

    private static void CleanupSessionListener()
    {
        if (databaseReference != null && sessionListener != null)
        {
            databaseReference.ValueChanged -= sessionListener;
            databaseReference = null;
            sessionListener = null;
        }
    }

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
                        FirebaseDatabase.DefaultInstance.RootReference.Child(UsersTag).Child(task.Result.User.UserId).Child(SessionTag).SetValueAsync("");
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
                            string userId = task.Result.User.UserId;
                            string sessionToken = Guid.NewGuid().ToString();
                            databaseReference = FirebaseDatabase.DefaultInstance.RootReference.Child(UsersTag).Child(userId);
                            databaseReference.Child(SessionTag).RunTransaction(mutableData =>
                            {
                                Dictionary<string, object> data = mutableData.Value as Dictionary<string, object>;
                                if (data == null || data.ContainsKey(TokenTag) == false) // 세션 없음: 내가 선점
                                {
                                    mutableData.Value = new Dictionary<string, object>
                                    {
                                        { TokenTag, sessionToken },
                                        { TimestampTag, ServerValue.Timestamp }
                                    };
                                    return TransactionResult.Success(mutableData);
                                }
                                return TransactionResult.Abort(); // 이미 세션 있음: 내가 새로 덮을 수 없음
                            }).ContinueWithOnMainThread(task =>
                            {
                                if (task.IsCanceled || task.IsFaulted)
                                {
                                    firebaseAuth.SignOut();
                                    action?.Invoke(State.SignInAlready);
                                }
                                else
                                {
                                    DataSnapshot snapshot = task.Result;
                                    if (snapshot == null || snapshot.Value == null)
                                    {
                                        firebaseAuth.SignOut();
                                        action?.Invoke(State.SignInAlready);
                                    }
                                    else
                                    {
                                        Dictionary<string, object> resultData = snapshot.Value as Dictionary<string, object>;
                                        if (resultData == null || resultData.ContainsKey(TokenTag) == false || resultData[TokenTag].ToString() != sessionToken)
                                        {
                                            firebaseAuth.SignOut();
                                            action?.Invoke(State.SignInAlready);
                                        }
                                        else //트랜잭션 성공 = 내가 첫 로그인 사용자
                                        {
                                            databaseReference.Child(SessionTag).OnDisconnect().SetValue(""); // 나만 이 세션 소유
                                            action?.Invoke(State.SignInSuccess); // 이후 주기적으로 내 세션 유지 확인 필요
                                            sessionListener = (object sender, ValueChangedEventArgs arguments) =>
                                            {
                                                if (arguments.DatabaseError == null)
                                                {
                                                    if (arguments.Snapshot.Exists == false) //세션 노드가 삭제됨 (로그아웃됨)
                                                    {
                                                        firebaseAuth.SignOut();
                                                        CleanupSessionListener();//action?.Invoke(State.SignInAlready);
                                                    }
                                                    else
                                                    {
                                                        Dictionary<string, object> data = arguments.Snapshot.Value as Dictionary<string, object>;
                                                        if (data != null && data.TryGetValue(TokenTag, out object serverTokenObject) && serverTokenObject.ToString() != sessionToken) //세션 탈취 감지: 다른 기기에서 로그인됨
                                                        {
                                                            firebaseAuth.SignOut();
                                                            CleanupSessionListener();//action?.Invoke(State.SignInAlready);
                                                        }
                                                    }
                                                }
                                            };
                                            databaseReference.Child(SessionTag).ValueChanged += sessionListener;
                                        }
                                    }
                                }
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
}