﻿using UnityEngine;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviourPun
{
    public Transform spawnPoint;
    public string enemyprefabPath;
    public float maxEnemies;
    public float spawnCheckTime;
    private float lastSpawnCheckTime;
    public List<GameObject> currentEnemies = new List<GameObject>();
    private void Update()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;
        if(Time.time-lastSpawnCheckTime>spawnCheckTime)
        {
            lastSpawnCheckTime = Time.time;
            TrySpawn();
        }
    }
    void TrySpawn()
    {
        for(int x=0;x<currentEnemies.Count;x++)
        {
            if (!currentEnemies[x])
            {
                currentEnemies.RemoveAt(x);
            }
        }
        if (currentEnemies.Count >= maxEnemies)
            return;
        GameObject enemy = PhotonNetwork.Instantiate(enemyprefabPath, spawnPoint.position, Quaternion.identity);
        currentEnemies.Add(enemy);
    }
}