using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Firebase;
using Photon.Pun;
using Photon.Realtime;

public class EntryManager : Manager
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
        SignUpFailed,
        SignUpAlready,
        SignUpSuccess,
        SignInFailed,
        SignInInvalidEmail,
        SignInAlready,
        LoseConnection
    }

    private Message _message = Message.None;

    private static readonly string IdentificationTag = "Identification";

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape) == true && _message != Message.LoseConnection)
        {
            ShowPopup(Quit, ClosePopup);
        }
    }

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
                ShowQuit();
            }
        });
    }

    protected override void ChangeText(Translation.Language language)
    {
        base.ChangeText(language);
        _titleText.Set(Translation.Get(Translation.Letter.Title));
        _identificationInputField.SetText(Translation.Get(Translation.Letter.Identification));
        _passwordInputField.SetText(Translation.Get(Translation.Letter.Password));
        _signUpButton.SetText(Translation.Get(Translation.Letter.SignUp));
        _signInButton.SetText(Translation.Get(Translation.Letter.SignIn));
        _versionText.Set(PhotonNetwork.GameVersion + " " + Translation.Get(Translation.Letter.Version));
        ShowMessage();
    }

    protected override void SetInteractable(bool value)
    {
        base.SetInteractable(value);
        _identificationInputField.SetInteractable(value);
        _passwordInputField.SetInteractable(value);
        _signUpButton.SetInteractable(value);
        _signInButton.SetInteractable(value);
    }

    protected override void ShowQuit()
    {
        ShowMessage(Message.LoseConnection);
        base.ShowQuit();
    }

    private void ShowMessage()
    {
        switch (_message)
        {
            case Message.Trying:
                _messageText.Set(Translation.Get(Translation.Letter.TryConnection));
                break;
            case Message.RequestIdentification:
            case Message.SignInInvalidEmail:
                _messageText.Set(string.Format(Translation.Get(Translation.Letter.RequestValid), Translation.Get(Translation.Letter.Identification)));
                break;
            case Message.RequestPassword:
                _messageText.Set(string.Format(Translation.Get(Translation.Letter.RequestValid), Translation.Get(Translation.Letter.Password)));
                break;
            case Message.SignUpFailed:
                _messageText.Set(Translation.Get(Translation.Letter.SignUp) + " " + Translation.Get(Translation.Letter.Failed));
                break;
            case Message.SignUpAlready:
                _messageText.Set(string.Format(Translation.Get(Translation.Letter.Duplicated), Translation.Get(Translation.Letter.Identification)));
                break;
            case Message.SignUpSuccess:
                _messageText.Set(Translation.Get(Translation.Letter.SignUp) + " " + Translation.Get(Translation.Letter.Success));
                break;
            case Message.SignInFailed:
                _messageText.Set(Translation.Get(Translation.Letter.SignIn) + " " +  Translation.Get(Translation.Letter.Failed));
                break;
            case Message.SignInAlready:
                _messageText.Set(Translation.Get(Translation.Letter.AlreadyConnected));
                break;
            default:
                _messageText.Set(null);
                break;
        }
        if(_message == Message.LoseConnection)
        {
            SetExplain(Translation.Get(Translation.Letter.LoseConnection));
        }
        else
        {
            SetExplain(string.Format(Translation.Get(Translation.Letter.DoYouWantTo), Translation.Get(Translation.Letter.Quit)));
        }
    }

    private void ShowMessage(Message value)
    {
        _message = value;
        ShowMessage();
        switch(_message)
        {
            case Message.SignUpFailed:
            case Message.SignUpAlready:
            case Message.SignUpSuccess:
            case Message.SignInFailed:
            case Message.SignInInvalidEmail:
            case Message.SignInAlready:
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
                            ShowQuit();
                            break;
                        case Authentication.State.SignUpFailure:
                            ShowMessage(Message.SignUpFailed);
                            break;
                        case Authentication.State.SignUpAlready:
                            ShowMessage(Message.SignUpAlready);
                            break;
                        case Authentication.State.SignUpSuccess:
                            ShowMessage(Message.SignUpSuccess);
                            PlayerPrefs.SetString(IdentificationTag, identification);
                            break;
                        case Authentication.State.SignInFailure:
                            ShowMessage(Message.SignInFailed);
                            break;
                        case Authentication.State.SignInInvalidEmail:
                            ShowMessage(Message.SignInInvalidEmail);
                            break;
                        case Authentication.State.SignInAlready:
                            ShowMessage(Message.SignInAlready);
                            break;
                        case Authentication.State.SignInSuccess:
                            PlayerPrefs.SetString(IdentificationTag, identification);
                            PhotonNetwork.ConnectUsingSettings();
                            break;
                    }
                });
            }
        }
    }

    public override void OnConnectedToMaster()
    {
        SceneManager.LoadScene(LobbyManager.SceneName);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        ShowQuit();
    }
}