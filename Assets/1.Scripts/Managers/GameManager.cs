using Photon.Pun;
using UnityEngine;

[DisallowMultipleComponent]
public class GameManager : MonoBehaviour
{
    private void Awake()
    {
        if (PhotonNetwork.IsMasterClient == true)
        {

        }
    }

    private void Update()
    {
        if(PhotonNetwork.IsMasterClient == true)
        {

        }
    }
}
