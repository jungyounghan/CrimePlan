using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using Photon.Pun;

public class EntryManager : Manager<EntryManager>
{
    [Header(nameof(EntryManager))]
    [SerializeField]
    private TMP_Text _titleText;
    [SerializeField]
    private TMP_Text _messageText;
    [SerializeField]
    private TMP_Text _versionText;
    [SerializeField]
    private TMP_InputField _identificationInputField;
    [SerializeField]
    private TMP_InputField _passwordInputField;
    [SerializeField]
    private Button _signUpButton;
    [SerializeField]
    private Button _signInButton;

    private enum Message
    {
        None,
        Trying,
        RequestIdentification,
        RequestPassword,
        SignUpFailure,
        SignUpDuplicate,
        SignUpSuccess,
        SignInFailure,
        SignInInvalidEmail,
        SignInWrongPassword,
        SignInUserNotFound,
        SignInAlready
    }

    private Message _message = Message.None;

    private static DatabaseReference _databaseReference = null;

    private static readonly string IdentificationTag = "Identification";

    public override void OnEnable()
    {
        base.OnEnable();
        if(_coroutine == null)
        {
            _coroutine = StartCoroutine(DoInitialize());
            IEnumerator DoInitialize()
            {
                DependencyStatus dependencyStatus = (DependencyStatus)(int)-1;
                FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
                {
                    dependencyStatus = task.Result;
                });
                yield return new WaitUntil(() => dependencyStatus >= DependencyStatus.Available);
                if (dependencyStatus == DependencyStatus.Available)
                {
                    SetInteractable(true);
                    string identification = PlayerPrefs.GetString(IdentificationTag);
                    if (string.IsNullOrWhiteSpace(identification) == false)
                    {
                        _identificationInputField.SetText(identification, false);
                    }
                }
                else
                {
                    Quit();
                }
                _coroutine = null;
            }
        }
    }

    protected override void SetInteractable(bool value)
    {
        base.SetInteractable(value);
        _identificationInputField.SetInteractable(value);
        _passwordInputField.SetInteractable(value);
        _signUpButton.SetInteractable(value);
        _signInButton.SetInteractable(value);
    }

    protected override void ChangeText(Translation.Language language)
    {
        base.ChangeText(language);
        _titleText.Set(Translation.GetTitle(language));
        _identificationInputField.SetText(Translation.GetIdentification(language));
        _passwordInputField.SetText(Translation.GetPassword(language));
        _signUpButton.SetText(Translation.GetSignUp(language));
        _signInButton.SetText(Translation.GetSignIn(language));
        _versionText.Set(PhotonNetwork.GameVersion + " " + Translation.GetVersion(language));
        ShowMessage(language);
    }

    public override void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void ShowMessage(Translation.Language language)
    {
        switch (_message)
        {
            case Message.Trying:
                _messageText.Set(Translation.GetTrying(language));
                break;
            case Message.RequestIdentification:
                _messageText.Set(Translation.GetRequestIdentification(language));
                break;
            case Message.RequestPassword:
                _messageText.Set(Translation.GetRequestPassword(language));
                break;
            case Message.SignUpFailure:
                _messageText.Set(Translation.GetSignUpFailure(language));
                break;
            case Message.SignUpDuplicate:
                _messageText.Set(Translation.GetSignUpDuplicate(language));
                break;
            case Message.SignUpSuccess:
                _messageText.Set(Translation.GetSignUpSuccess(language));
                break;
            case Message.SignInFailure:
                _messageText.Set(Translation.GetSignInFailure(language));
                break;
            case Message.SignInInvalidEmail:
                _messageText.Set(Translation.GetSignInInvalidEmail(language));
                break;
            case Message.SignInWrongPassword:
                _messageText.Set(Translation.GetSignInWrongPassword(language));
                break;
            case Message.SignInUserNotFound:
                _messageText.Set(Translation.GetSignInUserNotFound(language));
                break;
            case Message.SignInAlready:
                _messageText.Set(Translation.GetSignInAlready(language));
                break;
            default:
                _messageText.Set(null);
                break;
        }
    }

    private void ShowMessage(Message value)
    {
        _message = value;
        ShowMessage((Translation.Language)PlayerPrefs.GetInt(LanguageTag));
    }

    private void OnSessionChanged(object sender, ValueChangedEventArgs args)
    {
        //if (args.DatabaseError == null)
        //{
        //    string current = args.Snapshot?.Value?.ToString();
        //    if (current != sessionId)
        //    {
        //        Debug.LogWarning("다른 로그인이 감지되어 로그아웃합니다.");
        //        _databaseReference.ValueChanged -= OnSessionChanged;
        //        auth.SignOut();
        //    }
        //}      
    }

    public void Sign(bool creation)
    {
        string identification = _identificationInputField.GetText();
        if (string.IsNullOrWhiteSpace(identification) == true)
        {
            ShowMessage(Message.RequestIdentification);
        }
        else
        {
            string password = _passwordInputField.GetText();
            if (string.IsNullOrWhiteSpace(password) == true)
            {
                ShowMessage(Message.RequestPassword);
            }
            else if(_coroutine == null && FirebaseAuth.DefaultInstance != null)
            {
                
                _coroutine = StartCoroutine(DoSign());
                IEnumerator DoSign()
                {
                    ShowMessage(Message.Trying);
                    SetInteractable(false);
                    bool done = false;
                    if (creation == true)
                    {
                        FirebaseAuth.DefaultInstance.CreateUserWithEmailAndPasswordAsync(identification, password).ContinueWithOnMainThread(task =>
                        {
                            done = true;
                            if (task.IsFaulted)
                            {
                                bool duplicate = false;
                                foreach (Exception exception in task.Exception.Flatten().InnerExceptions)
                                {
                                    if (exception is FirebaseException firebaseException)
                                    {
                                        AuthError errorCode = (AuthError)firebaseException.ErrorCode;
                                        if (errorCode == AuthError.EmailAlreadyInUse)
                                        {
                                            duplicate = true;
                                            break;
                                        }
                                    }
                                }
                                if(duplicate == true)
                                {
                                    ShowMessage(Message.SignUpDuplicate);
                                }
                                else
                                {
                                    ShowMessage(Message.SignUpFailure);
                                }
                            }
                            else
                            {
                                ShowMessage(Message.SignUpSuccess);
                                PlayerPrefs.SetString(IdentificationTag, identification);
                            }
                            SetInteractable(true);
                        });
                    }
                    else
                    {
                        FirebaseAuth.DefaultInstance.SignInWithEmailAndPasswordAsync(identification, password).ContinueWithOnMainThread(task =>
                        {
                            if (task.IsCanceled || task.IsFaulted)
                            {
                                done = true;
                                foreach (Exception exception in task.Exception.Flatten().InnerExceptions)
                                {
                                    if (exception is FirebaseException firebaseEx)
                                    {
                                        AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
                                        switch (errorCode)
                                        {
                                            case AuthError.InvalidEmail:
                                                ShowMessage(Message.SignInInvalidEmail);
                                                break;
                                            case AuthError.WrongPassword:
                                                ShowMessage(Message.SignInWrongPassword);
                                                break;
                                            case AuthError.UserNotFound:
                                                ShowMessage(Message.SignInUserNotFound);
                                                break;
                                            default:
                                                ShowMessage(Message.SignInFailure);
                                                break;
                                        }
                                    }
                                }
                                SetInteractable(true);
                            }
                            else
                            {
                                FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser;
                                if (user == null)
                                {
                                    done = true;
                                    ShowMessage(Message.SignInFailure);
                                }
                                else
                                {
                                    string userId = task.Result.User.UserId;
                                    string sessionId = Guid.NewGuid().ToString();
                                    _databaseReference = FirebaseDatabase.DefaultInstance.GetReference(UsersTag).Child(userId).Child("sessionId");
                                    _databaseReference.GetValueAsync().ContinueWithOnMainThread(getTask =>
                                    {
                                        done = true;
                                        if (getTask.IsFaulted || getTask.IsCanceled)
                                        {
                                            Debug.LogError("세션 조회 실패");
                                        }
                                        else
                                        {
                                            string existingSession = getTask.Result?.Value?.ToString();
                                            if (!string.IsNullOrEmpty(existingSession) && existingSession != sessionId)
                                            {
                                                Debug.Log("다른 기기에서 이미 로그인 중입니다.");
                                                //FirebaseAuth.DefaultInstance.SignOut();
                                            }
                                            else
                                            {

                                                // 세션 저장 및 OnDisconnect 제거 설정
                                                _databaseReference.SetValueAsync(sessionId);
                                                _databaseReference.OnDisconnect().RemoveValue();

                                                // 실시간 감지
                                                //_databaseReference.ValueChanged += OnSessionChanged;

                                                Debug.Log("로그인 및 세션 설정 완료");
                                                // 포톤 로그인 등 이어서 처리
                                            }
                                        }
                                    });
                                }
                            }
                        });
                    }
                    yield return new WaitUntil(() => done == true);
                    _coroutine = null;
                }
            }
        }
    }
}