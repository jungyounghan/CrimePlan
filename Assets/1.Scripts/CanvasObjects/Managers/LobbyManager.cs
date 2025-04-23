using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using ExitGames.Client.Photon;

public class LobbyManager : Manager
{
    [Header(nameof(LobbyManager))]
    [SerializeField]
    private TMP_Text _stateText;
    [SerializeField]
    private TMP_Text _playerCountText;
    [SerializeField]
    private TMP_Text _roomCountText;
    [SerializeField]
    private TMP_InputField _roomInputField;
    [SerializeField]
    private Button _createButton;
    [SerializeField]
    private Button _joinButton;
    [SerializeField]
    private Button _exitButton;
    [SerializeField]
    private ScrollRect _scrollRect;

    [SerializeField]
    private Button _buttonPrefab;
    private List<Button> _buttonList = new List<Button>();

    private enum Message
    {
        None,
        Disconnect,
        CreateFailed,
        JoinFailed,
    }

    private Message _message = Message.None;

    public static readonly string SceneName = "LobbyScene";

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) == true && _message == Message.None)
        {
            ShowPopup(Quit, ClosePopup);
        }
    }

    private void LateUpdate()
    {
        _playerCountText.Set(Translation.Get(Translation.Letter.Accessor) + ": " + PhotonNetwork.CountOfPlayers);
        _roomCountText.Set(Translation.Get(Translation.Letter.Room) + ": " + PhotonNetwork.CountOfRooms);
    }

    private void ShowMessage()
    {
        switch(_message)
        {
            case Message.Disconnect:
                SetExplain(Translation.Get(Translation.Letter.LoseConnection));
                break;
            case Message.CreateFailed:
                SetExplain(Translation.Get(Translation.Letter.Create) + " " + Translation.Get(Translation.Letter.Failed));
                break;
            case Message.JoinFailed:
                SetExplain(Translation.Get(Translation.Letter.Join) + " " + Translation.Get(Translation.Letter.Failed));
                break;
            default:
                SetExplain(string.Format(Translation.Get(Translation.Letter.DoYouWantTo), Translation.Get(Translation.Letter.Quit)));
                break;
        }
    }

    private void ShowMessage(Message message)
    {
        _message = message;
        ShowMessage();
        switch(_message)
        {
            case Message.Disconnect:
                ShowPopup(() => { SceneManager.LoadScene(EntryManager.SceneName); });
                break;
            case Message.CreateFailed:
            case Message.JoinFailed:
                ShowPopup(() => { ClosePopup(); ShowMessage(Message.None); });
                break;
            default:
                SetInteractable(true);
                break;
        }
    }

    protected override void Initialize()
    {
        base.Initialize();
        if(PhotonNetwork.InLobby == false)
        {
            if(PhotonNetwork.IsConnectedAndReady == true)
            {
                PhotonNetwork.Disconnect();
            }
            else
            {
                OnDisconnected(DisconnectCause.None);
            }
        }
        else
        {
            _stateText.Set(Translation.Get(Translation.Letter.Identification) + ":" + PhotonNetwork.NickName);
            _createButton.SetListener(() =>
            {
                if (_roomInputField != null)
                {
                    RoomOptions roomOptions = new RoomOptions
                    {
                        CustomRoomProperties = new Hashtable() { { RoomManager.MembersKey, null } },
                        CustomRoomPropertiesForLobby = new string[] { RoomManager.MembersKey }
                    };
                    PhotonNetwork.CreateRoom(_roomInputField.text, roomOptions);
                }
            });
            _joinButton.SetListener(()=>
            {
                if (_roomInputField != null)
                {
                    string text = _roomInputField.text;
                    if (string.IsNullOrEmpty(text) == false)
                    {
                        PhotonNetwork.JoinRoom(text);
                    }
                    else
                    {
                        PhotonNetwork.JoinRandomRoom();
                    }
                }
            });
            _exitButton.SetListener(() => ShowPopup(Quit, ClosePopup));
            SetInteractable(true);
        }
    }

    protected override void ChangeText(Translation.Language language)
    {
        base.ChangeText(language);
        ShowMessage();
        _stateText.Set(Translation.Get(Translation.Letter.Identification) + ": " + PhotonNetwork.NickName);
        _playerCountText.Set(Translation.Get(Translation.Letter.Accessor) + ": " + PhotonNetwork.CountOfPlayers);
        _roomCountText.Set(Translation.Get(Translation.Letter.Room) + ": " + PhotonNetwork.CountOfRooms);
        _roomInputField.SetText(Translation.Get(Translation.Letter.Room) + " " + Translation.Get(Translation.Letter.Name));
        _createButton.SetText(Translation.Get(Translation.Letter.Create));
        _joinButton.SetText(Translation.Get(Translation.Letter.Join));
        _exitButton.SetText(Translation.Get(Translation.Letter.Exit));
    }

    protected override void SetInteractable(bool value)
    {
        base.SetInteractable(value);
        _roomInputField.SetInteractable(value);
        _createButton.SetInteractable(value);
        _joinButton.SetInteractable(value);
        _exitButton.SetInteractable(value);
        foreach(Button button in _buttonList)
        {
            button.SetInteractable(value);
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        ShowMessage(Message.Disconnect);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomInfos)
    {
        string playing = null;
        int index = 0;
        for(int i = 0; i < roomInfos.Count; i++)
        {
            int playerCount = roomInfos[i].PlayerCount;
            if (playerCount > 0)
            {
                Hashtable hashtable = roomInfos[i].CustomProperties;
                if(hashtable.ContainsKey(RoomManager.MembersKey) == false || hashtable[RoomManager.MembersKey] == null)
                {
                    bool done = false;
                    if (index < _buttonList.Count)
                    {
                        _buttonList[index].SetListener(() =>
                        {
                            SetInteractable(false);
                            PhotonNetwork.JoinRoom(roomInfos[i].Name);
                        }, roomInfos[i].Name + "\t" + playerCount);
                        done = true;
                    }
                    else if (_buttonPrefab != null && _scrollRect != null && _scrollRect.content != null)
                    {
                        Button button = Instantiate(_buttonPrefab, _scrollRect.content);
                        button.SetListener(() =>
                        {
                            SetInteractable(false);
                            PhotonNetwork.JoinRoom(roomInfos[i].Name);
                        }, roomInfos[i].Name + "\t" + playerCount);
                        _buttonList.Add(button);
                        done = true;
                    }
                    if (done == true)
                    {
                        index++;
                    }
                }
                else if (hashtable[RoomManager.MembersKey].ToString().Contains(PhotonNetwork.NickName) == true)
                {
                    playing = roomInfos[i].Name;
                }
            }
        }
        for(int i = index; i < _buttonList.Count; i++)
        {
            _buttonList[i].SetActive(false);
        }
        if (playing != null)
        {
            SetInteractable(false);
            PhotonNetwork.JoinRoom(playing);
        }
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        ShowMessage(Message.CreateFailed);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        ShowMessage(Message.JoinFailed);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        ShowMessage(Message.JoinFailed);
    }

    public override void OnJoinedRoom()
    {
        Room room = PhotonNetwork.CurrentRoom;
        if(room != null)
        {
            Hashtable hashtable = room.CustomProperties;
            if (hashtable.ContainsKey(RoomManager.MembersKey) == true && hashtable[RoomManager.MembersKey] != null && hashtable[RoomManager.MembersKey].ToString().Contains(PhotonNetwork.NickName) == true)
            {
                PhotonNetwork.LoadLevel(GameManager.SceneName);
            }
            else
            {
                SceneManager.LoadScene(RoomManager.SceneName);
            }
        }
    }
}