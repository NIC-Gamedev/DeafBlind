using FishNet.Object;
using System;
using UnityEngine;

public class ObjectSpawner : NetworkBehaviour
{
    [SerializeField]
    private GameObject objectToSpawn;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (IsServer)
        {
            SpawnObject();
        }
    }

    private void SpawnObject()
    {
        if (objectToSpawn == null)
        {
            Debug.LogWarning("Object to spawn is not assigned!");
            return;
        }

        // Спавним объект на сервере
        GameObject spawnedObject = Instantiate(objectToSpawn, transform.position, transform.rotation);

        // Проверяем, есть ли у объекта NetworkObject
        NetworkObject networkObject = spawnedObject.GetComponent<NetworkObject>();
        if (networkObject != null)
        {
            // Спавним объект в сети
            Spawn(networkObject);
        }
        else
        {
            Debug.LogWarning("Spawned object does not have a NetworkObject component!");
        }
    }
}
