using FishNet;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;
using static UnityEditor.Progress;

public class HandHolder : NetworkBehaviour
{
    [Header("Hand Settings")]
    [SerializeField] private Transform dropPoint;
    [SerializeField] private InventoryHolder inventoryHolder;

    public InventorySlot activeSlot;
    public GameObject currentItemObject;

    public static event System.Action<GameObject> OnHandItemChanged;

    private void Update()
    {
        if (!IsOwner) return;
        HandleInput();
    }
    [ObserversRpc]
    public void TriggerHandItemChanged(GameObject item)
    {
        OnHandItemChanged?.Invoke(item);
    }
    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Q)) // Drop item
        {
            RequestDropItemServerRpc();
        }
        if (Input.GetKeyDown(KeyCode.Mouse1)) // Use item
        {
            RequestUseItemServerRpc();
        }
        if (Input.GetKeyDown(KeyCode.Alpha1)) SelectItem(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SelectItem(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SelectItem(2);
    }

    [ServerRpc(RequireOwnership = true)]

    private void SelectItem(int slotIndex)
    {
        // Вместо чтения данных локально просто делаем вызов серверного метода
        RequestInventorySlotDataServerRpc(slotIndex);
    }
    [ServerRpc(RequireOwnership = false)]
    private void RequestInventorySlotDataServerRpc(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= inventoryHolder.InventorySystem.InventorySize)
        {
            Debug.Log("Slot index out of range (server check)");
            return;
        }

        InventorySlot slot = inventoryHolder.InventorySystem.InventorySlots[slotIndex];

        if (slot.ItemData == null)
        {
            Debug.Log("Slot is null (server check)");
            return;
        }
        else
        {
            Debug.Log("Slot item data is not null (server check)");
        }

        // Обновляем активный слот на сервере на основе серверных данных
        activeSlot = slot;

        // Затем вызываем серверный метод обновления руки (где производится спавн нового объекта)
        UpdateHandItemServerRpc(slotIndex);
    }

    [ServerRpc(RequireOwnership = false)]
    private void UpdateHandItemServerRpc(int slotIndex)
    {
        // Дополнительная проверка (при желании)
        if (slotIndex < 0 || slotIndex >= inventoryHolder.InventorySystem.InventorySize)
        {
            Debug.Log("Slot index out of range (UpdateHandItemServerRpc)");
            return;
        }

        InventorySlot slot = inventoryHolder.InventorySystem.InventorySlots[slotIndex];

        UpdateHandItemServerRpc();
    }
    

    [ServerRpc(RequireOwnership = false)]
    private void UpdateHandItemServerRpc()
    {
        if (currentItemObject != null)
        {
            NetworkObject oldNetObj = currentItemObject.GetComponent<NetworkObject>();
            if (oldNetObj != null && oldNetObj.IsSpawned)
            {
                Despawn(oldNetObj);
            }
            Destroy(currentItemObject);
            currentItemObject = null;
        }

        if (activeSlot != null && activeSlot.ItemData != null && activeSlot.ItemData.ItemPrefab != null)
        {
            GameObject item = Instantiate(activeSlot.ItemData.ItemPrefab, dropPoint.position, dropPoint.rotation);
            item.transform.SetParent(dropPoint);

            TurnOffItem(item);
            NetworkObject netObj = item.GetComponent<NetworkObject>();
            if (netObj != null)
            {
                Spawn(netObj);
                currentItemObject = item;
                currentItemObject = netObj.gameObject;
                currentItemObject.transform.SetParent(dropPoint);
                currentItemObject.transform.localPosition = Vector3.zero;
                currentItemObject.transform.localRotation = Quaternion.identity;
                TurnOffItem(currentItemObject);

                OnHandItemChanged(currentItemObject);
            }

        }

        OnHandItemChanged(currentItemObject);


    }


    #region Must be fixed
    [ServerRpc(RequireOwnership = false)]
    private void TurnOffItem(GameObject item)
    {
        // Отключаем физику и задаём полное замораживание
        if (item.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.useGravity = false;
            rb.isKinematic = true;
            rb.constraints = RigidbodyConstraints.FreezeAll; // Фиксируем все оси
        }

        // Отключаем скрипт подбора
        if (item.TryGetComponent<ItemPickUp>(out var pickup))
        {
            pickup.enabled = false;
        }

        // Отключаем все коллайдеры
        foreach (var col in item.GetComponents<Collider>())
        {
            col.enabled = false;
        }

        // Фиксируем позицию и вращение объекта относительно dropPoint
        if (dropPoint != null)
        {
            //item.transform.SetParent(dropPoint);
            item.transform.localPosition = Vector3.zero;
            item.transform.localRotation = Quaternion.identity;
        }
        TurnOffItemClient(item);
        OnHandItemChanged?.Invoke(currentItemObject);
    }

    [ObserversRpc]
    private void TurnOffItemClient(GameObject item)
    {
        // Отключаем физику и задаём полное замораживание
        if (item.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.useGravity = false;
            rb.isKinematic = true;
            rb.constraints = RigidbodyConstraints.FreezeAll; // Фиксируем все оси
        }

        // Отключаем скрипт подбора
        if (item.TryGetComponent<ItemPickUp>(out var pickup))
        {
            pickup.enabled = false;
        }

        // Отключаем все коллайдеры
        foreach (var col in item.GetComponents<Collider>())
        {
            col.enabled = false;
        }

        // Фиксируем позицию и вращение объекта относительно dropPoint
        if (dropPoint != null)
        {
            item.transform.SetParent(dropPoint);
            item.transform.localPosition = Vector3.zero;
            item.transform.localRotation = Quaternion.identity;
        }

        OnHandItemChanged?.Invoke(currentItemObject);
    }
    #endregion
    private void UseItem()
    {
        if (activeSlot == null || activeSlot.ItemData == null || currentItemObject == null) return;

        IUsable usableComponent = currentItemObject.GetComponent<IUsable>();
        if (usableComponent != null)
        {
            usableComponent.Use();

            if (activeSlot.ItemData.isOneUse)
                activeSlot.RemoveFromStack(1);

            if (activeSlot.StackSize <= 0)
                activeSlot.ClearSlot();

            UpdateHandItemServerRpc();
        }
        else
        {
            Debug.Log("This item cannot be used.");
        }
        OnHandItemChanged(currentItemObject);

    }

    private void DropItem()
    {
        if (activeSlot == null || activeSlot.ItemData == null) return;

        GameObject droppedItem = Instantiate(
            activeSlot.ItemData.ItemPrefab,
            dropPoint.position,
            Quaternion.identity
        );
        NetworkObject netObj = droppedItem.GetComponent<NetworkObject>();
        if (netObj != null)
        {
            Spawn(netObj);
        }
        TurnOnItemServerRpc(droppedItem);

        ThrowableObjects objectthrow = droppedItem.GetComponent<ThrowableObjects>();
        if (objectthrow)
        {
            objectthrow.Throw(Vector3.up);
        }



        activeSlot.RemoveFromStack(1);

        if (activeSlot.StackSize <= 0)
        {
            activeSlot.ClearSlot();
        }

        UpdateHandItemServerRpc();
        OnHandItemChanged(currentItemObject);

    }

    [ServerRpc]
    private void RequestUseItemServerRpc()
    {
        UseItem();
    }

    [ServerRpc]
    private void RequestDropItemServerRpc()
    {
        DropItem();
    }

    [ServerRpc(RequireOwnership = false)]
    private void TurnOnItemServerRpc(GameObject item)
    {
        if (item == null) return;

        // Включаем физику
        if (item.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.useGravity = true;
            rb.isKinematic = false;
            rb.constraints = RigidbodyConstraints.None;
        }

        // Включаем скрипт подбора
        if (item.TryGetComponent<ItemPickUp>(out var pickup))
        {
            pickup.enabled = true;
        }

        // Включаем все коллайдеры
        foreach (var col in item.GetComponents<Collider>())
        {
            col.enabled = true;
        }

        // Отсоединяем от dropPoint
        item.transform.SetParent(null);

        // Уведомляем клиентов
        TurnOnItemObserversRpc(item);
    }


    [ObserversRpc]
    private void TurnOnItemObserversRpc(GameObject item)
    {
        if (item == null) return;

        // Включаем физику
        if (item.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.useGravity = true;
            rb.isKinematic = false;
            rb.constraints = RigidbodyConstraints.None;
        }

        // Включаем скрипт подбора
        if (item.TryGetComponent<ItemPickUp>(out var pickup))
        {
            pickup.enabled = true;
        }

        // Включаем все коллайдеры
        foreach (var col in item.GetComponents<Collider>())
        {
            col.enabled = true;
        }

        // Отсоединяем от dropPoint
        item.transform.SetParent(null);
    }

}
