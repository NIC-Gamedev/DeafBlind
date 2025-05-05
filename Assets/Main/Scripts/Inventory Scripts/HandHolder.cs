using System.Collections;
using FishNet.Object;
using UnityEngine;

public class HandHolder : NetworkBehaviour
{
    [Header("Hand Settings")]
    [SerializeField] private Transform dropPoint;
    [SerializeField] private InventoryHolder inventoryHolder;

    public InventorySlot activeSlot;
    public GameObject currentItemObject;
    public Coroutine dropItemProcess;
    public float timeBefDrop;
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
            OnHandItemChanged?.Invoke(currentItemObject);

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
        // ������ ������ ������ �������� ������ ������ ����� ���������� ������
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

        // ��������� �������� ���� �� ������� �� ������ ��������� ������
        activeSlot = slot;

        // ����� �������� ��������� ����� ���������� ���� (��� ������������ ����� ������ �������)
        UpdateHandItemServerRpc(slotIndex);
    }

    [ServerRpc(RequireOwnership = false)]
    private void UpdateHandItemServerRpc(int slotIndex)
    {
        // �������������� �������� (��� �������)
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
        Debug.Log("Update Hand");
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
            Debug.Log("I Was spawned");
            item.transform.SetParent(dropPoint);
            NetworkObject netObj = item.GetComponent<NetworkObject>();
            if (netObj != null)
            {
                Spawn(item);
                currentItemObject = netObj.gameObject;
                currentItemObject.transform.SetParent(dropPoint);
                currentItemObject.transform.localPosition = Vector3.zero;
                currentItemObject.transform.localRotation = Quaternion.identity;
                TurnOffItem(currentItemObject);
                SyncHandItemObserversRpc(netObj);
                OnHandItemChanged?.Invoke(currentItemObject);
            }

        }
        OnHandItemChanged?.Invoke(currentItemObject);
    }
    
    [ObserversRpc]
    public void SyncHandItemObserversRpc(NetworkObject networkObject)
    {
        currentItemObject = networkObject.gameObject;
    }


    #region Must be fixed
    [ServerRpc(RequireOwnership = false)]
    private void TurnOffItem(GameObject item)
    {
        // ��������� ������ � ����� ������ �������������
        if (item.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.useGravity = false;
            rb.isKinematic = true;
            rb.constraints = RigidbodyConstraints.FreezeAll; // ��������� ��� ���
        }

        // ��������� ������ �������
        if (item.TryGetComponent<ItemPickUp>(out var pickup))
        {
            pickup.enabled = false;
        }

        // ��������� ��� ����������
        foreach (var col in item.GetComponents<Collider>())
        {
            col.enabled = false;
        }

        // ��������� ������� � �������� ������� ������������ dropPoint
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
        // ��������� ������ � ����� ������ �������������
        if (item.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.useGravity = false;
            rb.isKinematic = true;
            rb.constraints = RigidbodyConstraints.FreezeAll; // ��������� ��� ���
        }

        // ��������� ������ �������
        if (item.TryGetComponent<ItemPickUp>(out var pickup))
        {
            pickup.enabled = false;
        }

        // ��������� ��� ����������
        foreach (var col in item.GetComponents<Collider>())
        {
            col.enabled = false;
        }

        // ��������� ������� � �������� ������� ������������ dropPoint
        if (dropPoint != null)
        {
            Debug.Log( "This is Item: "+ item);
            item.transform.SetParent(dropPoint);
            item.transform.localPosition = Vector3.zero;
            item.transform.localRotation = Quaternion.identity;
        }

        OnHandItemChanged?.Invoke(currentItemObject);
    }
    #endregion
    [ServerRpc]
    private void UseItem()
    {
        if (activeSlot == null || activeSlot.ItemData == null || currentItemObject == null) return;

        IUsable usableComponent = currentItemObject.GetComponent<IUsable>();
        if (usableComponent != null)
        {
            usableComponent.Use();

            /*if (activeSlot.ItemData.isOneUse)
                activeSlot.RemoveFromStack(1);

            if (activeSlot.StackSize <= 0)
                activeSlot.ClearSlot();

            UpdateHandItemServerRpc();*/
        }
        else
        {
            Debug.Log("This item cannot be used.");
        }
        OnHandItemChanged(currentItemObject);

    }

    private IEnumerator DropItem()
    {
        yield return new WaitForSeconds(timeBefDrop);
        GameObject droppedItem = Instantiate(
            activeSlot.ItemData.ItemPrefab,
            dropPoint.position,
            Quaternion.identity
        );
        
        Spawn(droppedItem);
        
        TurnOnItemServerRpc(droppedItem);
        
        ThrowHeldItemServerRpc(transform.forward,droppedItem);

        activeSlot.RemoveFromStack(1);

        if (activeSlot.StackSize <= 0)
        {
            activeSlot.ClearSlot();
        }

        UpdateHandItemServerRpc();
        OnHandItemChanged?.Invoke(currentItemObject);
        dropItemProcess = null;
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void ThrowHeldItemServerRpc(Vector3 direction, GameObject droppedItem)
    {
        if (droppedItem.TryGetComponent<ThrowableObjects>(out var throwable))
        {
            throwable.Throw(direction);
        }
    }
    
    private void RequestUseItemServerRpc()
    {
        UseItem();
    }
    
    private void RequestDropItemServerRpc()
    {
        if(activeSlot == null || activeSlot.ItemData == null)
            return;
        if (dropItemProcess == null)
        {
            Debug.Log($"I Request");
            dropItemProcess = StartCoroutine(DropItem());
        }
        OnHandItemChanged?.Invoke(currentItemObject);

    }

    [ServerRpc(RequireOwnership = false)]
    private void TurnOnItemServerRpc(GameObject item)
    {
        if (item == null) return;

        // �������� ������
        if (item.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.useGravity = true;
            rb.isKinematic = false;
            rb.constraints = RigidbodyConstraints.None;
        }

        // �������� ������ �������
        if (item.TryGetComponent<ItemPickUp>(out var pickup))
        {
            pickup.enabled = true;
        }

        // �������� ��� ����������
        foreach (var col in item.GetComponents<Collider>())
        {
            col.enabled = true;
        }

        // ����������� �� dropPoint
        item.transform.SetParent(null);

        // ���������� ��������
        TurnOnItemObserversRpc(item);
    }


    [ObserversRpc]
    private void TurnOnItemObserversRpc(GameObject item)
    {
        if (item == null) return;

        // �������� ������
        if (item.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.useGravity = true;
            rb.isKinematic = false;
            rb.constraints = RigidbodyConstraints.None;
        }

        // �������� ������ �������
        if (item.TryGetComponent<ItemPickUp>(out var pickup))
        {
            pickup.enabled = true;
        }

        // �������� ��� ����������
        foreach (var col in item.GetComponents<Collider>())
        {
            col.enabled = true;
        }

        // ����������� �� dropPoint
        item.transform.SetParent(null);
    }

}
