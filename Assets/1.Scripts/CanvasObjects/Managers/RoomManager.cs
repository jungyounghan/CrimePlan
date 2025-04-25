using System.Text;
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
    private GameObject[] _persons = new GameObject[Person.FormCount];
    [SerializeField]
    private TMP_Text _roomNameText;
    [SerializeField]
    private ScrollRect _scrollRect;
    [SerializeField]
    private Button _leftButton;
    [SerializeField]
    private Button _rightButton;
    [SerializeField]
    private Button _citizenButton;
    [SerializeField]
    private Button _criminalButton;
    [SerializeField]
    private Button _readyButton;
    [SerializeField]
    private Button _exitButton;

    [SerializeField]
    private Button _buttonPrefab;
    private List<(Player, Button)> _playerList = new List<(Player, Button)>();

    private bool _disconnection = false;
    private bool _identity = false;
    private Person.Form _form = Person.Form.Capper;

    public const string IdentityKey = "Identity";
    public const string PersonFormKey = "PersonForm";
    public const string MembersKey = "Members";
    public static readonly string SceneName = "RoomScene";

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
        if(this == _instance)
        {
            int length = _persons.Length;
            int end = Person.FormCount;
            if (length > end)
            {
                _persons = new GameObject[] { _persons[((int)Person.Form.Capper)], _persons[((int)Person.Form.Lady)], _persons[((int)Person.Form.Strider)] };
            }
            else
            {
                if (length < end)
                {
                    GameObject[] persons = new GameObject[end];
                    for (int i = 0; i < length; i++)
                    {
                        persons[i] = _persons[i];
                    }
                    _persons = persons;
                }
                ExtensionMethod.Sort(ref _persons);
            }
            ShowPerson();
        }
    }
#endif

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) == true)
        {
            ShowPopup(LeaveRoom, ClosePopup);
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

    private void ShowPerson()
    {
        for (int i = 0; i < _persons.Length; i++)
        {
            _persons[i].Set((int)_form == i);
        }
    }

    private void SetReadyButtonText()
    {
        if(PhotonNetwork.IsMasterClient == true)
        {
            _readyButton.SetText(Translation.Get(Translation.Letter.Start));
        }
        else
        {
            _readyButton.SetText(Translation.Get(Translation.Letter.Ready));
        }
    }

    private void LeaveRoom()
    {
        if(_disconnection == false)
        {
            SetInteractable(false);
            PhotonNetwork.LeaveRoom();
        }
    }

    private void SetPerson(bool right)
    {
        switch(right)
        {
            case false:
                if(_form > Person.Form.Capper)
                {
                    _form--;
                    ShowPerson();
                }
                break;
            case true:
                if (_form < Person.Form.Strider)
                {
                    _form++;
                    ShowPerson();
                }
                break;
        }
    }

    private void SetInteractableSelect(bool value)
    {
        _leftButton.SetInteractable(value);
        _rightButton.SetInteractable(value);
        _citizenButton.SetInteractable(value);
        _criminalButton.SetInteractable(value);
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
            SetReadyButtonText();
            if (_scrollRect != null && _scrollRect.content != null && _buttonPrefab != null)
            {
                Dictionary<int, Player> players = room.Players;
                foreach (Player player in players.Values)
                {
                    Button button = Instantiate(_buttonPrefab, _scrollRect.content);
                    if (player == PhotonNetwork.MasterClient)
                    {
                        button.SetText(player.NickName + "(" + Translation.Get(Translation.Letter.Host) + ")", false);
                    }
                    else
                    {
                        button.SetText(player.NickName, false);
                    }
                    _playerList.Add((player, button));
                }
            }
            _leftButton.SetListener(() => SetPerson(false));
            _rightButton.SetListener(() => SetPerson(true));
            _citizenButton.SetListener(() => _identity = Person.Citizen);
            _criminalButton.SetListener(() => _identity = Person.Criminal);
            _readyButton.SetListener(() =>
            {
                if(PhotonNetwork.IsMasterClient == false)
                {
                    Hashtable hashtable = PhotonNetwork.LocalPlayer.CustomProperties;
                    if (hashtable != null)
                    {
                        bool done = hashtable.ContainsKey(IdentityKey) == false || hashtable[IdentityKey] == null;
                        if (done == true)
                        {
                            PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable() { { IdentityKey, _identity }, { PersonFormKey, (byte)_form } });
                        }
                        else
                        {
                            PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable() { { IdentityKey, null }, { PersonFormKey, null} });
                        }
                        SetInteractableSelect(!done);
                    }
                }
                else
                {
                    Dictionary<int, Player> players = room.Players;
                    StringBuilder stringBuilder = new StringBuilder(PhotonNetwork.NickName);
                    foreach(Player player in players.Values)
                    {
                        if(player != PhotonNetwork.LocalPlayer)
                        {
                            if(player.CustomProperties != null && (player.CustomProperties.ContainsKey(IdentityKey) == false || player.CustomProperties[IdentityKey] == null))
                            {
                                return;
                            }
                            stringBuilder.Append(" " + player.NickName);
                        }
                    }
                    room.SetCustomProperties(new Hashtable() { {MembersKey , stringBuilder.ToString()} });
                    PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable() { {IdentityKey, _identity}, { PersonFormKey, (byte)_form} });
                    SetInteractable(false);
                }
            });
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
        _citizenButton.SetText(string.Format(Translation.Get(Translation.Letter.Apply), Translation.Get(Translation.Letter.Citizen)));
        _criminalButton.SetText(string.Format(Translation.Get(Translation.Letter.Apply), Translation.Get(Translation.Letter.Criminal)));
        SetReadyButtonText();
        _exitButton.SetText(Translation.Get(Translation.Letter.Exit));
        foreach ((Player, Button)item in _playerList)
        {
            if (item.Item1 != null)
            {
                if (item.Item1 == PhotonNetwork.MasterClient)
                {
                    item.Item2.SetText(item.Item1.NickName + "(" + Translation.Get(Translation.Letter.Host) + ")");
                }
                else
                {
                    if(item.Item1.CustomProperties.ContainsKey(IdentityKey) == false || item.Item1.CustomProperties[IdentityKey] == null)
                    {
                        item.Item2.SetText(item.Item1.NickName);
                    }
                    else
                    {
                        item.Item2.SetText(item.Item1.NickName + "(" + Translation.Get(Translation.Letter.Ready) + ")");
                    }
                }
            }
        }
    }

    protected override void SetInteractable(bool value)
    {
        base.SetInteractable(value);
        SetInteractableSelect(value);
        _readyButton.SetInteractable(value);
        _exitButton.SetInteractable(value);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        _disconnection = true;
        ShowMessage();
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
        SceneManager.LoadScene(LobbyManager.SceneName);
    }

    public override void OnPlayerEnteredRoom(Player player)
    {
        for (int i = 0; i < _playerList.Count; i++)
        {
            (Player, Button) item = _playerList[i];
            if (item.Item1 == null)
            {
                item.Item2.SetActive(true, player.NickName);
                _playerList[i] = (player, item.Item2);
                return;
            }
        }
        if (_scrollRect != null && _scrollRect.content != null && _buttonPrefab != null)
        {
            Button button = Instantiate(_buttonPrefab, _scrollRect.content);
            button.SetText(player.NickName, false);
            _playerList.Add((player, button));
        }
    }

    public override void OnPlayerLeftRoom(Player player)
    {
        for(int i = 0; i < _playerList.Count; i++)
        {
            (Player, Button) item = _playerList[i];
            if(item.Item1 == player)
            {
                item.Item2.SetActive(false);
                _playerList[i] = (null, item.Item2);
            }
            else if (item.Item1 == null && item.Item2.gameObject.activeSelf == true)
            {
                item.Item2.gameObject.SetActive(false);
            }
        }
    }

    public override void OnMasterClientSwitched(Player player)
    {
        foreach((Player, Button) item in _playerList)
        {
            if(item.Item1 == player)
            {
                item.Item2.SetText(item.Item1.NickName + "(" + Translation.Get(Translation.Letter.Host) + ")");
            }
            else if(item.Item1 != null)
            {
                Hashtable hashtable = item.Item1.CustomProperties;
                if(hashtable != null)
                {
                    if (hashtable[IdentityKey] != null && bool.TryParse(hashtable[IdentityKey].ToString(), out bool value))
                    {
                        item.Item2.SetText(item.Item1.NickName + "(" + Translation.Get(Translation.Letter.Ready) + ")");
                    }
                    else
                    {
                        item.Item2.SetText(item.Item1.NickName);
                    }
                }
            }
        }
        if(PhotonNetwork.LocalPlayer == player)
        {
            _readyButton.SetText(Translation.Get(Translation.Letter.Start));
        }
    }

    public override void OnPlayerPropertiesUpdate(Player player, Hashtable hashtable)
    {
        if(hashtable.ContainsKey(IdentityKey) == true)
        {
            if (player != PhotonNetwork.MasterClient)
            {
                foreach((Player, Button)item in _playerList)
                {
                    if(item.Item1 == player)
                    {
                        if (hashtable[IdentityKey] != null && bool.TryParse(hashtable[IdentityKey].ToString(), out bool value))
                        {
                            item.Item2.SetText(item.Item1.NickName + "(" + Translation.Get(Translation.Letter.Ready) + ")");
                        }
                        else
                        {
                            item.Item2.SetText(item.Item1.NickName);
                        }
                        break;
                    }
                }
            }
            else
            {
                PhotonNetwork.LoadLevel(GameManager.SceneName);
            }
        }
    }
}