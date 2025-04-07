using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class EntryManager : Manager
{
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

    }

    private Message _message = Message.None;

    protected override void SetInteractable(bool value)
    {
        _identificationInputField.SetInteractable(value);
        _passwordInputField.SetInteractable(value);
        _signUpButton.SetInteractable(value);
        _signInButton.SetInteractable(value);
    }

    protected override void ChangeText(Translation.Language language)
    {
        _titleText.Set(Translation.GetTitle(language));
        _identificationInputField.SetText(Translation.GetIdentification(language));
        _passwordInputField.SetText(Translation.GetPassword(language));
        _signUpButton.SetText(Translation.GetSignUp(language));
        _signInButton.SetText(Translation.GetSignIn(language));
        _versionText.Set(PhotonNetwork.GameVersion + " " + Translation.GetVersion(language));
    }
}