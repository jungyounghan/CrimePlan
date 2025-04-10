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
            Debug.Log("������");
        }
        else
        {
            PhotonNetwork.JoinLobby();
        }
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("�κ� ���� ����");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log(roomList.Count);
    }
}
