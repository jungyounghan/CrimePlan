using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class RoomManager : Manager
{
    [Header(nameof(RoomManager)),SerializeField]
    private TMP_Text _testText;

    public const string IdentityKey = "Identity";
    public const string PersonFormKey = "PersonForm";

    protected override void Initialize()
    {
        base.Initialize();
        SetInteractable(true);
        PhotonNetwork.ConnectUsingSettings();
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
        _testText.Set(PhotonNetwork.CurrentRoom.PlayerCount.ToString());
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        _testText.Set(PhotonNetwork.CurrentRoom.PlayerCount.ToString());
    }

    public override void OnRoomPropertiesUpdate(Hashtable hashtable)
    {
        if(hashtable != null)
        {
            foreach(string key in hashtable.Keys)
            {
                if(key == "Start")
                {
                    PhotonNetwork.LoadLevel(GameManager.SceneName);
                }
            }
        }
    }

    public void PlayGame()
    {
        Room room = PhotonNetwork.CurrentRoom;
        if(room != null)
        {
            room.SetCustomProperties(new Hashtable() { {"Start", null} });
        }
    }
}