using UnityEngine;
using Photon.Pun;

public class GameManager : MonoBehaviourPunCallbacks
{
    public GameObject playerPrefab;
    public Transform[] spawnPositions;
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
       GameObject playerObject = PhotonNetwork.Instantiate(PlayerSelection.playerselection.playerPrefabName, Vector3.zero, Quaternion.identity);
       playerObject.GetComponent<PhotonView>().RPC("InitializePlayer", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer);
    }
}