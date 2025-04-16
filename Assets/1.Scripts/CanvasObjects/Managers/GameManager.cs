using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;
using System.Collections.Generic;

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

    private double _waitingTime = 0;

    public const string TurnKey = "Turn";
    public const string TimeKey = "Time";

    private static readonly float TimeLimitValue = 10;
    public static readonly string SceneName = "GameScene";

    private void Update()
    {
        if (_waitingTime > 0)
        {
            double currentTime = _waitingTime - PhotonNetwork.Time;
            if (currentTime <= 0)
            {
                _waitingTime = 0;
                if (PhotonNetwork.IsMasterClient == true)
                {
                    Hashtable hashtable = PhotonNetwork.CurrentRoom.CustomProperties;
                    int turn = hashtable.ContainsKey(TurnKey) && hashtable[TurnKey] != null && int.TryParse(hashtable[TurnKey].ToString(), out turn) ? turn : 0;
                    switch(turn)
                    {
                        case 0:
                            break;
                        case 1:
                            break;
                        case 2:
                            break;
                    }
                    PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable() { { TimeKey, PhotonNetwork.Time + TimeLimitValue }, { TurnKey, turn + 1 } });
                }
            }
            else if (Input.GetMouseButtonDown(0))
            {
                Camera camera = Camera.main;
                if (camera != null)
                {                    //누르고 명령 내리기(return 값이 있고 그것이 캔버스로 전송되게하자)
                    _stageController?.UpdateInput(camera.ScreenPointToRay(Input.mousePosition)); 
                }
            }
            getStateController.UpdateTime(currentTime);
        }
        _stageController?.UpdateMove();
    }

    protected override void Initialize()
    {
        base.Initialize();        //_stageController?.Initialize(FindObjectOfType<CinemachineVirtualCamera>().Set(transform));        //getStateController.Initialize();
        SetInteractable(true);
        PhotonNetwork.ConnectUsingSettings();
    }

    protected override void ChangeText(Translation.Language language)
    {
        base.ChangeText(language);
        getStateController.ChangeText();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        PhotonNetwork.JoinRandomOrCreateRoom();
    }

    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.IsMasterClient == true)
        {
            PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable() { { TimeKey, PhotonNetwork.Time + TimeLimitValue } });
            _stageController?.Initialize((identity) => { getStateController.Set(identity); });
        }
        else
        {
            OnRoomPropertiesUpdate(PhotonNetwork.CurrentRoom.CustomProperties);
        }
    }

    public override void OnRoomPropertiesUpdate(Hashtable hashtable)
    {
        if (hashtable != null)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            foreach (string key in hashtable.Keys)
            {
                switch(key)
                {
                    case TimeKey:
                        if (hashtable[key] != null && float.TryParse(hashtable[key].ToString(), out float time) == true)
                        {
                            _waitingTime = time;
                        }
                        else
                        {
                            _waitingTime = 0;
                        }
                        break;
                    case TurnKey:
                        if (hashtable[key] != null && byte.TryParse(hashtable[key].ToString(), out byte turn) == true)
                        {
                            getStateController.OnRoomPropertiesUpdate(turn);
                        }
                        else
                        {
                            getStateController.OnRoomPropertiesUpdate(0);
                        }
                        break;
                }
            }
        }
    }
}