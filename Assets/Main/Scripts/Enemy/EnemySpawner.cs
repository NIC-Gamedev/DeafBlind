using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    public StateController enemyPrefab;

    public void Initialize(MapData mapData)
    {
        Spawn(mapData.allWayPoints[Random.Range(0,mapData.allWayPoints.Count)].transform.position);
    }
    public void Spawn(Vector3 position)
    {
        Instantiate(enemyPrefab, position,Quaternion.identity);
    }
}
