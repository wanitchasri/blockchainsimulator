using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnPlayers : MonoBehaviour
{
    public GameObject playerPrefab;
    private float spawnRange = 3;

    void Start()
    {
        float spawnPosX = Random.Range(spawnRange, -spawnRange);
        float spawnPosZ = Random.Range(spawnRange, -spawnRange);
        Vector3 randomPos = new Vector3(spawnPosX, 0, spawnPosZ);

        PhotonNetwork.Instantiate(playerPrefab.name, randomPos, playerPrefab.transform.rotation);
    }

    void Update()
    {
        
    }
}
