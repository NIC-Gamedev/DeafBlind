using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsableCan : MonoBehaviour,IUsable
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

        // Спавним объект на текущей позиции
        GameObject spawnedObject = Instantiate(_gameObject, transform.position, transform.rotation);

        // Проверяем, есть ли у объекта Rigidbody
        Rigidbody rb = spawnedObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            // Применяем толчок вперед
            rb.AddForce(transform.forward * 5f, ForceMode.Impulse);
        }
        else
        {
            Debug.LogWarning("Spawned object does not have a Rigidbody component!");
        }
    }

}
