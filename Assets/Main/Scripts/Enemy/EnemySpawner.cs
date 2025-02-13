using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : NetworkBehaviour
{
    public StateController enemyPrefab;

    [Server]
    public void Spawn(Vector3 position)
    {
        if (enemyPrefab)
        {
            GameObject inst = Instantiate(enemyPrefab.gameObject, position, Quaternion.identity);
            Spawn(inst);
        }
    }
}
