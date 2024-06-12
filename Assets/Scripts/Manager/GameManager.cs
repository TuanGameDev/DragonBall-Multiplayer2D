using UnityEngine;
using Photon.Pun;
using System.Linq;
using System.Collections.Generic;
using Photon.Realtime;
using Photon.Pun.Demo.PunBasics;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks
{
    [Header("GameManager")]
    public Transform[] spawnPoints;
    public float respawnTime;
    public static GameManager gamemanager;

    private void Awake()
    {
        gamemanager = this;
    }

    private void Start()
    {
        if (PhotonNetwork.IsConnected)
        {
            if (PhotonNetwork.InRoom)
            {
                SpawnPlayer();
            }
        }
    }
    void SpawnPlayer()
    {
        if (spawnPoints.Length > 0)
        {
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            GameObject playerObject = PhotonNetwork.Instantiate(PlayerSelection.playerselection.playerPrefabName, spawnPoint.transform.position, Quaternion.identity);
            playerObject.GetComponent<PhotonView>().RPC("InitializePlayer", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer);
        }
    }
    public override void OnJoinedRoom()
    {
        SpawnPlayer();
    }
    public void LeaveRoom()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
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