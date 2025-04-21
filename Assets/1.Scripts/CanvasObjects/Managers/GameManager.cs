using UnityEngine;
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

    public enum Cycle: byte
    {
        Evening,
        Morning,
        Midday,
        End
    }

    private double _waitingTime = 0;
    private string _targetName = null;

    public const string TurnKey = "Turn";
    public const string TimeKey = "Time";
    public const string TargetKey = "Target";
    public const string EndKey = "End";

    public static readonly double EveningTimeValue = 10;
    public static readonly double MorningTimeValue = 30;
    public static readonly double MiddayTimeValue = 10;
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

    private void SelectTarget(bool agree)
    {
        if(string.IsNullOrEmpty(_targetName) == false)
        {
            Room room = PhotonNetwork.CurrentRoom;
            if (room != null)
            {
                if (agree == true)
                {
                    room.SetCustomProperties(new Hashtable() { { PhotonNetwork.NickName, _targetName} });
                }
                else
                {
                    room.SetCustomProperties(new Hashtable() { { PhotonNetwork.NickName, null } });
                }
            }
        }
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
        getStateController.Initialize(SelectTime, SelectTarget);
        if(PhotonNetwork.IsMasterClient == true)
        {
            PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable() { { TimeKey, PhotonNetwork.Time + EveningTimeValue }, { TurnKey, 0} });
        }
        else
        {
            Room room = PhotonNetwork.CurrentRoom;
            if (room != null)
            {
                OnRoomPropertiesUpdate(room.CustomProperties);
            }
            else
            {
#if UNITY_EDITOR
                Debug.LogError("방에 접속되어 있지 않음");
#endif
            }
        }
    }

    protected override void ChangeText(Translation.Language language)
    {
        base.ChangeText(language);
        _stageController?.ChangeText();
        getStateController.ChangeText();
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
                        break;
                    case EndKey:
                        //결과 보고
                        break;
                    default:
                        _stageController?.OnRoomPropertiesUpdate(key, hashtable[key]);
                        break;
                }
            }
        }
    }
}