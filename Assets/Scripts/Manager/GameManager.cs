using UnityEngine;
using Photon.Pun;
using System.Linq;

public class GameManager : MonoBehaviourPun
{
    [Header("GameManager")]
    public Transform[] spawnPoint;
    public Transform spawnPointMap;
    public float respawnTime;
    public int currentSpawnIndex = 0;
    public static GameManager gamemanager;
    [Header("Loading Map")]
    public GameObject canvasHealh;
    public GameObject[] MapPrefabs;
    public int currentMapIndex = 0;
    public GameObject currentMapObject;
    public void Awake()
    {
        gamemanager = this;
    }
    void Start()
    {
        SpawnPlayer();
        SpawnMap();
    }
    void SpawnPlayer()
    {
        GameObject playerObject = PhotonNetwork.Instantiate(PlayerSelection.playerselection.playerPrefabName, spawnPoint[currentSpawnIndex].position, Quaternion.identity);
        playerObject.GetComponent<PhotonView>().RPC("InitializePlayer", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer);
        currentSpawnIndex = (currentSpawnIndex + 1) % spawnPoint.Length;
    }
    void SpawnMap()
    {
        // Sinh ra map mới
        if (currentMapIndex < MapPrefabs.Length)
        {
            currentMapObject = Instantiate(MapPrefabs[currentMapIndex], spawnPointMap.position, Quaternion.identity);
            PhotonNetwork.AllocateViewID(currentMapObject.GetPhotonView());
            currentMapObject.GetPhotonView().TransferOwnership(PhotonNetwork.LocalPlayer);
            currentMapIndex++;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PhotonView photonView = collision.GetComponent<PhotonView>();
            if (photonView != null && photonView.IsMine)
            {
                canvasHealh.SetActive(true);
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PhotonView photonView = collision.GetComponent<PhotonView>();
            if (photonView != null && photonView.IsMine)
            {
                canvasHealh.SetActive(false);
            }
        }
    }
    public void ButtonMap(int current)
    {
        currentMapIndex = current;

        // Hủy bỏ map hiện tại (nếu có)
        if (currentMapObject != null)
        {
            Destroy(currentMapObject);
        }
        SpawnMap();
        currentSpawnIndex = 0;
    }
}