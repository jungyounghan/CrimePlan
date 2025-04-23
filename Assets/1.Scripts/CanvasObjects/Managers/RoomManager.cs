using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class RoomManager : Manager
{
    [Header(nameof(RoomManager))]
    [SerializeField]
    private TMP_Text _roomNameText;
    [SerializeField]
    private ScrollRect _scrollRect;

    [SerializeField]
    private Button _exitButton;

    [SerializeField]
    private Button _buttonPrefab;
    private List<Button> _buttonList = new List<Button>();

    private bool _disconnection = false;

    public const string IdentityKey = "Identity";
    public const string PersonFormKey = "PersonForm";
    public const string MembersKey = "Members";
    public static readonly string SceneName = "RoomScene";

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) == true)
        {
            ShowPopup(LeaveRoom, ClosePopup); //·ë ³ª°¡±â
        }
    }

    private void ShowMessage()
    {
        switch(_disconnection)
        {
            case false:
                SetExplain(string.Format(Translation.Get(Translation.Letter.DoYouWantTo), Translation.Get(Translation.Letter.Leave)));
                break;
            case true:
                SetExplain(Translation.Get(Translation.Letter.LoseConnection));
                break;
        }
    }

    private void ShowMessage(bool value)
    {
        _disconnection = value;
        ShowMessage();
    }

    private void LeaveRoom()
    {
        if(_disconnection == false)
        {
            SetInteractable(false);
        }
    }

    protected override void Initialize()
    {
        base.Initialize();
        SetInteractable(true);
        Room room = PhotonNetwork.CurrentRoom;
        if(room == null)
        {
            OnDisconnected(DisconnectCause.None);
        }
        else
        {
            _roomNameText.Set(Translation.Get(Translation.Letter.Room) + " " + Translation.Get(Translation.Letter.Name) + ":" + room.Name);
            if (_scrollRect != null && _scrollRect.content != null && _buttonPrefab != null)
            {
                Dictionary<int, Player> players = room.Players;
                foreach (Player player in players.Values)
                {
                    Button button = Instantiate(_buttonPrefab, _scrollRect.content);
                    button.SetText(player.NickName, false);
                    _buttonList.Add(button);
                }
            }
            _exitButton.SetListener(() => ShowPopup(LeaveRoom, ClosePopup));
            SetInteractable(true);
        }
    }

    protected override void ChangeText(Translation.Language language)
    {
        base.ChangeText(language);
        ShowMessage();
        Room room = PhotonNetwork.CurrentRoom;
        string roomName = room != null ? room.Name : null;
        _roomNameText.Set(Translation.Get(Translation.Letter.Room) + " " + Translation.Get(Translation.Letter.Name) + ":" + roomName);

    }

    protected override void SetInteractable(bool value)
    {
        base.SetInteractable(value);

        _exitButton.SetInteractable(value);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        ShowMessage(true);
        ShowPopup(() => SceneManager.LoadScene(EntryManager.SceneName));
    }

    public override void OnLeftRoom()
    {
        Player player = PhotonNetwork.LocalPlayer;
        if (player != null)
        {
            Hashtable hashtable = player.CustomProperties;
            if (hashtable != null)
            {
                List<string> list = new List<string>();
                foreach (string key in hashtable.Keys)
                {
                    list.Add(key);
                }
                hashtable = new Hashtable();
                for(int i = 0; i < list.Count; i++)
                {
                    hashtable.Add(list[i], null);
                }
                player.SetCustomProperties(hashtable);
            }
        }
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        SceneManager.LoadScene(LobbyManager.SceneName);
    }

    public override void OnPlayerEnteredRoom(Player player)
    {

    }

    public override void OnPlayerLeftRoom(Player player)
    {

    }

    public override void OnRoomPropertiesUpdate(Hashtable hashtable)
    {

    }
}