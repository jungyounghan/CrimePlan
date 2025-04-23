using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

[RequireComponent(typeof(StateController))]
public class GameManager : Manager
{
    private bool _hasStateController = false;

    private StateController _stateController = null;

    private StateController getStateController
    {
        get
        {
            if (_hasStateController == false)
            {
                _hasStateController = TryGetComponent(out _stateController);
            }
            return _stateController;
        }
    }

    [Header(nameof(GameManager)), SerializeField]
    private StageController _stageController;

    private enum Message
    {
        None,
        Disconnect,
        Citizen,
        Criminal,
    }

    private Message _message = Message.None;

    public enum Cycle: byte
    {
        Evening,
        Morning,
        Midday,
        End
    }

    private double _waitingTime = 0;
    private string _targetName = null;
    private static readonly float EndTimeValue = 3f;

    public const string TurnKey = "Turn";
    public const string TimeKey = "Time";
    public const string TargetKey = "Target";
    public const string EndKey = "End";

    public static readonly double TimeLimitValue = 10;
    public static readonly string SceneName = "GameScene";

    private void Update()
    {
        if (_waitingTime > 0)
        {
            double currentTime = _waitingTime - PhotonNetwork.Time;
            if (currentTime <= 0)
            {
                UpdateTurn();
            }
            else if (Input.GetMouseButtonDown(0) && _stageController != null)
            {
                getStateController.SelectPerson(_stageController.GetSelectInfo());
            }
            getStateController.UpdateState(currentTime);
        }
    }

    private void LateUpdate()
    {
        if (_stageController != null)
        {
            getStateController.ShowRemains(_stageController.GetRemainsCount());
        }
    }

    private void SelectTime(bool increasing)
    {
        Room room = PhotonNetwork.CurrentRoom;
        if (room != null)
        {
            double time = PhotonNetwork.Time;
            double currentTime = _waitingTime - time;
            if (currentTime > 0)
            {
                if (increasing == true)
                {
                    room.SetCustomProperties(new Hashtable() { { TimeKey, _waitingTime + TimeLimitValue } });
                }
                else if (currentTime - TimeLimitValue > 0)
                {
                    room.SetCustomProperties(new Hashtable() { { TimeKey, _waitingTime - TimeLimitValue } });
                }
                else
                {
                    room.SetCustomProperties(new Hashtable() { { TimeKey, time } });
                }
            }
        }
    }

    private void ShowMessage()
    {
        switch (_message)
        {
            case Message.Disconnect:
                SetExplain(Translation.Get(Translation.Letter.LoseConnection));
                break;
            case Message.Citizen:
                SetExplain(Translation.Get(Translation.Letter.Citizen) + " " + Translation.Get(Translation.Letter.Victory));
                break;
            case Message.Criminal:
                SetExplain(Translation.Get(Translation.Letter.Criminal) + " " + Translation.Get(Translation.Letter.Victory));
                break;
        }
    }

    private void ShowMessage(Message message)
    {
        _message = message;
        ShowMessage();
    }

    private void UpdateTurn()
    {
        _waitingTime = 0;
        if (string.IsNullOrEmpty(_targetName) == false)
        {
            _targetName = null;
        }
        _stageController?.UpdateTurn();
    }

    protected override void Initialize()
    {
        base.Initialize();
        SetInteractable(true);
        _stageController?.Initialize();
        getStateController.Initialize(SelectTime, (value) => { _stageController?.SelectTarget(_targetName, value); });
        Room room = PhotonNetwork.CurrentRoom;
        if (room != null)
        {
            if(PhotonNetwork.IsMasterClient == true)
            {
                room.SetCustomProperties(new Hashtable() { { TimeKey, PhotonNetwork.Time + TimeLimitValue }, { TurnKey, 0 } });
            }
            else
            {
                OnRoomPropertiesUpdate(room.CustomProperties);
            }
        }
        else
        {
            ShowQuit();
        }
    }

    protected override void ChangeText(Translation.Language language)
    {
        base.ChangeText(language);
        _stageController?.ChangeText();
        getStateController.ChangeText();
        ShowMessage();
    }

    protected override void ShowQuit()
    {
        ShowMessage(Message.Disconnect);
        base.ShowQuit();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        ShowMessage(Message.Disconnect);
        ShowPopup(() => { SceneManager.LoadScene(EntryManager.SceneName); });
    }

    public override void OnRoomPropertiesUpdate(Hashtable hashtable)
    {
        if (hashtable != null)
        {
            foreach (string key in hashtable.Keys)
            {
                switch(key)
                {
                    case TimeKey:
                        if (hashtable[key] != null && double.TryParse(hashtable[key].ToString(), out double time) == true)
                        {
                            _waitingTime = time;
                            double currentTime = _waitingTime - PhotonNetwork.Time;
                            if (currentTime <= 0)
                            {
                                UpdateTurn();
                                getStateController.UpdateState(currentTime);
                            }
                        }
                        break;
                    case TurnKey:
                        byte turn = 0;
                        if (hashtable[key] != null)
                        {
                            byte.TryParse(hashtable[key].ToString(), out turn);
                        }
                        getStateController.OnRoomPropertiesUpdate(turn);
                        _stageController?.OnRoomPropertiesUpdate(turn);
                        break;
                    case TargetKey:
                        _targetName = hashtable[key] != null ? hashtable[key].ToString() : null;
                        _stageController?.OnRoomPropertiesUpdate(_targetName);
                        break;
                    case EndKey:
                        if (hashtable[key] != null && bool.TryParse(hashtable[key].ToString(), out bool end) == true)
                        {
                            StartCoroutine(DoPlayEnd());
                            System.Collections.IEnumerator DoPlayEnd()
                            {
                                yield return new WaitForSeconds(EndTimeValue);
                                PhotonNetwork.LeaveRoom();
                                switch (end)
                                {
                                    case Person.Citizen:
                                        ShowMessage(Message.Citizen);
                                        break;
                                    case Person.Criminal:
                                        ShowMessage(Message.Criminal);
                                        break;
                                }
                            }
                        }
                        break;
                    case RoomManager.MembersKey:
                        break;
                    default:
                        _stageController?.OnRoomPropertiesUpdate(key, hashtable[key]);
                        break;
                }
            }
        }
    }

    public override void OnJoinedLobby()
    {
        SceneManager.LoadScene(LobbyManager.SceneName);
    }

    public override void OnLeftRoom()
    {
        ShowPopup(() => PhotonNetwork.JoinLobby());
    }
}