using UnityEngine;
using Photon.Pun;
using System.Linq;

public class GameManager : MonoBehaviourPun
{
    public Transform[] spawnPoint;
    public float respawnTime;
    public static GameManager gamemanager;
    public void Awake()
    {
        gamemanager = this;
    }
    void Start()
    {
        SpawnPlayer();
    }
    void SpawnPlayer()
    {
        GameObject playerObject = PhotonNetwork.Instantiate(PlayerSelection.playerselection.playerPrefabName, spawnPoint[Random.Range(0, spawnPoint.Length)].position, Quaternion.identity);
       playerObject.GetComponent<PhotonView>().RPC("InitializePlayer", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer);
    }
}