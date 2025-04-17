using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
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
        #region 나중에는 지워주자        
        Dictionary<int, Player> players = PhotonNetwork.CurrentRoom.Players;
        foreach (Player player in players.Values)
        {
            if (player == PhotonNetwork.LocalPlayer)
            {
                PhotonNetwork.NickName = player.UserId;
                break;
            }
        }
        #endregion
        _testText.Set(PhotonNetwork.CurrentRoom.PlayerCount.ToString());
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        _testText.Set(PhotonNetwork.CurrentRoom.PlayerCount.ToString());

        //PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable() { { IdentityKey, Person.Criminal } });
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