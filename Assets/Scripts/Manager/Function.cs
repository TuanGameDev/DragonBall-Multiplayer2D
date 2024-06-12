using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using TMPro;
using ExitGames.Client.Photon;
using Photon.Realtime;
public class Function : MonoBehaviourPunCallbacks
{
    public void LeaveRoom()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
            PhotonNetwork.Disconnect();
        }
        else
        {
            Debug.LogWarning("Bạn không ở trong phòng!");
        }
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("Main");
    }
}
