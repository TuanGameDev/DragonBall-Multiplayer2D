using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnEnemy : MonoBehaviourPun
{
    [Header("Spawn Enemy")]
    public Transform[] spawnPoint;
    public GameObject spawnenemy;
    private float lastCheckTime;
    public float spawnenemyCheckTime;
    private void Start()
    {
        SpawnEnemyPoint();
    }
    void Update()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;
    }

    public void SpawnEnemyPoint()
    {
        if (spawnPoint.Length == 0 || spawnenemy == null)
        {
            Debug.LogError("Spawn points or enemy prefab is not set.");
            return;
        }

        int randomIndex = Random.Range(0, spawnPoint.Length);
        Transform spawnLocation = spawnPoint[randomIndex];
        GameObject newEnemy = PhotonNetwork.Instantiate(spawnenemy.name, spawnLocation.position, spawnLocation.rotation);
    }
}
