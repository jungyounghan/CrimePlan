using System;
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
        SignUpDuplicate,
        SignUpSuccess,
        SignInFailure,
        SignInInvalidEmail,
    }

    private static FirebaseAuth firebaseAuth = null;
    private static DatabaseReference databaseReference = null;

    private static readonly string sessionIdTag = "sessionId";

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
                    if (task.IsFaulted == true)
                    {
                        foreach (Exception exception in task.Exception.Flatten().InnerExceptions)
                        {
                            if (exception is FirebaseException firebaseException)
                            {
                                AuthError errorCode = (AuthError)firebaseException.ErrorCode;
                                if (errorCode == AuthError.EmailAlreadyInUse)
                                {
                                    action?.Invoke(State.SignUpDuplicate);
                                    return;
                                }
                            }
                        }
                        action?.Invoke(State.SignUpFailure);
                    }
                    else if(task.IsCanceled == true)
                    {
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
                    if (task.IsFaulted)
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
                    else if(task.IsCanceled)
                    {
                        action?.Invoke(State.SignInFailure);
                    }
                    else
                    {
                        //FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser;
                        //if (user == null)
                        //{
                        //    done = true;
                        //    ShowMessage(Message.SignInFailure);
                        //}
                        //else
                        //{
                        //    string userId = task.Result.User.UserId;
                        //    string sessionId = Guid.NewGuid().ToString();
                        //    _databaseReference = FirebaseDatabase.DefaultInstance.GetReference(UsersTag).Child(userId).Child("sessionId");
                        //    _databaseReference.GetValueAsync().ContinueWithOnMainThread(getTask =>
                        //    {
                        //        done = true;
                        //        if (getTask.IsFaulted || getTask.IsCanceled)
                        //        {
                        //            Debug.LogError("세션 조회 실패");
                        //        }
                        //        else
                        //        {
                        //            string existingSession = getTask.Result?.Value?.ToString();
                        //            if (!string.IsNullOrEmpty(existingSession) && existingSession != sessionId)
                        //            {
                        //                Debug.Log("다른 기기에서 이미 로그인 중입니다.");
                        //                            //FirebaseAuth.DefaultInstance.SignOut();
                        //                        }
                        //            else
                        //            {

                        //                            // 세션 저장 및 OnDisconnect 제거 설정
                        //                            _databaseReference.SetValueAsync(sessionId);
                        //                _databaseReference.OnDisconnect().RemoveValue();

                        //                            // 실시간 감지
                        //                            //_databaseReference.ValueChanged += OnSessionChanged;

                        //                            Debug.Log("로그인 및 세션 설정 완료");
                        //                            // 포톤 로그인 등 이어서 처리
                        //                        }
                        //        }
                        //    });
                        //}
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