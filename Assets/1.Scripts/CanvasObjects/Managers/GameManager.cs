using System.Collections.Generic;
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

    public const string TurnKey = "Turn";
    public const string TimeKey = "Time";

    public static readonly float TimeLimitValue = 10;
    public static readonly string SceneName = "GameScene";

    private void Update()
    {
        if (_waitingTime > 0)
        {
            double currentTime = _waitingTime - PhotonNetwork.Time;
            if (currentTime <= 0)
            {
                _waitingTime = 0;
                _stageController?.UpdateTurn();
            }
            else if (Input.GetMouseButtonDown(0) && _stageController != null)
            {
                getStateController.UpdateSelect(_stageController.GetPerson());
            }
            getStateController.UpdateTime(currentTime);
        }
    }

    protected override void Initialize()
    {
        base.Initialize();
        SetInteractable(true);
        _stageController?.Initialize((value) => { getStateController.ShowRemains(value); });
        if(PhotonNetwork.IsMasterClient == true)
        {
            PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable() { { TimeKey, PhotonNetwork.Time + TimeLimitValue }, { TurnKey, 0 } });
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
                        byte turn = 0;
                        if (hashtable[key] != null)
                        {
                            byte.TryParse(hashtable[key].ToString(), out turn);
                        }
                        getStateController.OnRoomPropertiesUpdate(turn);
                        //게임 승패 나오면
                        break;
                }
            }
        }
    }
}