using UnityEngine;
using Photon.Pun;

public class EnemySpawner : MonoBehaviourPunCallbacks
{
    public GameObject enemyPrefab;
    public Transform spawnPoint;
    private void Start()
    {
        SpawnEnemy();
    }
    public void SpawnEnemy()
    {
        PhotonNetwork.Instantiate(enemyPrefab.name, spawnPoint.position, Quaternion.identity);
    }
}