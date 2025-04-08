using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase;
using Firebase.Database;
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
        SignInAlready
    }

    private Message _message = Message.None;

    private static DatabaseReference _databaseReference = null;

    private static readonly string IdentificationTag = "Identification";

    protected override void Initialize()
    {
        base.Initialize();
        Authentication.Initialize((value) => { 
            if(value == DependencyStatus.Available)
            {
                SetInteractable(true);
                _identificationInputField.SetText(PlayerPrefs.GetString(IdentificationTag), false);
            }
            else
            {
                Quit();
            }
        });
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
        switch(_message)
        {
            case Message.SignUpFailure:
            case Message.SignUpDuplicate:
            case Message.SignUpSuccess:
            case Message.SignInFailure:
            case Message.SignInInvalidEmail:
                SetInteractable(true);
                break;
        }
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
            else
            {
                ShowMessage(Message.Trying);
                SetInteractable(false);
                Authentication.Sign(identification, password, creation, (state) =>
                {
                    switch (state)
                    {
                        case Authentication.State.EmptyAccount:
                            Quit();
                            break;
                        case Authentication.State.SignUpFailure:
                            ShowMessage(Message.SignUpFailure);
                            break;
                        case Authentication.State.SignUpDuplicate:
                            ShowMessage(Message.SignUpDuplicate);
                            break;
                        case Authentication.State.SignUpSuccess:
                            ShowMessage(Message.SignUpSuccess);
                            PlayerPrefs.SetString(IdentificationTag, identification);
                            break;
                        case Authentication.State.SignInFailure:
                            ShowMessage(Message.SignInFailure);
                            break;
                        case Authentication.State.SignInInvalidEmail:
                            ShowMessage(Message.SignInInvalidEmail);
                            break;
                    }
                });
            }
        }
    }
}