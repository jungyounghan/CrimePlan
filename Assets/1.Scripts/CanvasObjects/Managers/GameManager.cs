using UnityEngine;
using Cinemachine;
using Photon.Pun;
using ExitGames.Client.Photon;

[RequireComponent(typeof(PlayerProfile))]
public class GameManager : Manager
{
    private bool _hasPlayerProfile = false;

    private PlayerProfile _playerProfile = null;

    private PlayerProfile getPlayerProfile
    {
        get
        {
            if (_hasPlayerProfile == false)
            {
                _hasPlayerProfile = TryGetComponent(out _playerProfile);
            }
            return _playerProfile;
        }
    }

    [Header(nameof(GameManager))]
    [SerializeField]
    private PersonObject[] _personObjectPrefabs = new PersonObject[(int)PersonObject.Type.Strider + 1];

    private double _waitingTime = 0;

#if UNITY_EDITOR
    [Header("테스트 모드")]
    [SerializeField]
    private PersonObject.Type _testType = PersonObject.Type.Capper;
    [SerializeField]
    private uint _citizenCount = 1;
    [SerializeField]
    private uint _policeCount = 0;
    [SerializeField]
    private uint _criminalCount = 0;

    protected override void OnValidate()
    {
        base.OnValidate();
        if(_citizenCount == 0)
        {
            _citizenCount = 1;
        }
        switch (_testType)
        {
            case PersonObject.Type.Police:
                if (_criminalCount == 0)
                {
                    _criminalCount = 1;
                }
                break;
            default:
                if (_policeCount == 0)
                {
                    _policeCount = 1;
                }
                break;
        }
    }
#endif

    public static readonly string SceneName = "GameScene";

    private void Update()
    {
        if (_waitingTime > 0)
        {
            double currentTime = _waitingTime - PhotonNetwork.Time;
            if (currentTime <= 0)
            {
                _waitingTime = 0;
            }
            getPlayerProfile.SetTimer(currentTime);
        }
    }

    private void OnDestroy()
    {
        PersonObject.createAction -= Create;
    }

    private void Create(PersonObject personObject)
    {
        if(PhotonNetwork.InRoom == true)
        {

        }
        else
        {

        }
    }

    protected override void Initialize()
    {
        base.Initialize();
        PersonObject.createAction += Create;
        if (PhotonNetwork.InRoom == true)
        {

        }
        else
        {
#if UNITY_EDITOR
            if ((int)_testType < _personObjectPrefabs.Length && _personObjectPrefabs[(int)_testType] != null)
            {
                GameObject gameObject = Instantiate(_personObjectPrefabs[(int)_testType].gameObject);
                FindObjectOfType<CinemachineVirtualCamera>().Set(gameObject.transform);
            }
            else
            {
                Debug.LogError("프리팹 삽입 필요");
            }
#endif
        }
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
        _waitingTime = PhotonNetwork.Time + 10;
    }

    public override void OnRoomPropertiesUpdate(Hashtable hashtable)
    {

    }
}