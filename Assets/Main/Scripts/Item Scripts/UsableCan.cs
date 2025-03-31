using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet;
using FishNet.Object;
public class UsableCan : NetworkBehaviour, IUsable
{
    private ItemPickUp _data;
    private GameObject _gameObject;
    private void Start()
    {
        _data = GetComponent<ItemPickUp>();
        _gameObject = _data.ItemData.ItemPrefab;
    }
    public void Use()
    {
        if (_data == null || _gameObject == null)
        {
            Debug.LogWarning("Item data or prefab is missing!");
            return;
        }

        // ????? ?????????? ?????? ??? ?????? ???????
        SpawnObjectServerRpc(transform.position, transform.rotation);
    }

    [ServerRpc]
    private void SpawnObjectServerRpc(Vector3 position, Quaternion rotation)
    {
        // ??????? ?????? ?? ???????
        GameObject spawnedObject = Instantiate(_gameObject, position, rotation);

        // ?????????, ???? ?? ? ??????? NetworkObject
        NetworkObject networkObject = spawnedObject.GetComponent<NetworkObject>();
        if (networkObject != null)
        {
            // ??????? ?????? ? ????
            Spawn(networkObject);
        }
        else
        {
            Debug.LogWarning("Spawned object does not have a NetworkObject component!");
        }

        // ?????????, ???? ?? ? ??????? Rigidbody
        Rigidbody rb = spawnedObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            // ????????? ?????? ??????
            rb.AddForce(transform.forward * 5f, ForceMode.Impulse);
        }
        else
        {
            Debug.LogWarning("Spawned object does not have a Rigidbody component!");
        }
    }

}