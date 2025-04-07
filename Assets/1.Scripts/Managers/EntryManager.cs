using UnityEngine;
using UnityEngine.UI;
using TMPro;
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
        Loading,
        Identification,
        Password,

    }

    private Message _message = Message.None;

    protected override void Awake()
    {
        base.Awake();
        Authentication.Initialize((value) => { 
            if(value == Firebase.DependencyStatus.Available)
            {
                SetInteractable(true);
                //¾ÆÀÌµð
            }
        });
    }

    protected override void SetInteractable(bool value)
    {
        Debug.Log(value);
        _identificationInputField.SetInteractable(value);
        _passwordInputField.SetInteractable(value);
        _signUpButton.SetInteractable(value);
        _signInButton.SetInteractable(value);
        Debug.Log(value);
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
            case Message.Loading:
                _messageText.Set(Translation.GetLoading(language));
                break;
            case Message.Identification:
                _messageText.Set(Translation.GetRequestIdentification(language));
                break;
            default:
                _messageText.Set(null);
                break;
        }
    }

    public void Sign(bool creation)
    {
        string identification = _identificationInputField.GetText();
        string password = _passwordInputField.GetText();
    }
}