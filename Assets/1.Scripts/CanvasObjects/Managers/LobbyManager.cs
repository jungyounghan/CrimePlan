using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class LobbyManager : Manager
{
    public static readonly string SceneName = "LobbyScene";

    protected override void Initialize()
    {
        base.Initialize();
        if(PhotonNetwork.IsConnectedAndReady == false)
        {
            Debug.Log("나가기");
        }
        else
        {
            PhotonNetwork.JoinLobby();
        }
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("로비 입장 성공");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log(roomList.Count);
    }
}
