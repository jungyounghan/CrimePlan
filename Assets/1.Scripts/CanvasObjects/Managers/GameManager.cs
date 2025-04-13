using UnityEngine;
using Cinemachine;
using Photon.Pun;
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

    [Header(nameof(GameManager))]
    [SerializeField]
    private StageController _stageController;

    private double _waitingTime = 0;

    public static readonly string SceneName = "GameScene";

    public const string TurnKey = "Turn";

    private void Update()
    {
        if (_waitingTime > 0)
        {
            double currentTime = _waitingTime - PhotonNetwork.Time;
            if (currentTime <= 0)
            {
                _waitingTime = 0;
            }
            else if (Input.GetMouseButtonDown(0))
            {
                Camera camera = Camera.main;
                if (camera != null)
                {
                    //누르고 명령 내리기(return 값이 있고 그것이 캔버스로 전송되게하자)
                    _stageController?.UpdateInput(camera.ScreenPointToRay(Input.mousePosition)); 
                }
            }
            getStateController.SetTimer(currentTime);
        }
        _stageController?.UpdateMove();
    }

    private void Watch(Transform transform)
    {
        FindObjectOfType<CinemachineVirtualCamera>().Set(transform);
    }

    protected override void Initialize()
    {
        base.Initialize();
        _stageController?.Initialize(Watch);
        getStateController.Initialize();
    }

    protected override void ChangeText(Translation.Language language)
    {
        base.ChangeText(language);
        getStateController.ChangeText();
    }

#if UNITY_EDITOR
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
        _waitingTime = PhotonNetwork.Time + 10;
    }
#endif

    public override void OnRoomPropertiesUpdate(Hashtable hashtable)
    {
        _stageController?.OnRoomPropertiesUpdate(hashtable);
    }
}