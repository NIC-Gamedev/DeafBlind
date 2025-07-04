using System;
using System.Collections;
using FishNet.Object;
using UnityEngine;
using UnityEngine.InputSystem;

public class HandHolder : NetworkBehaviour
{
    [Header("Hand Settings")]
    [SerializeField] private Transform dropPoint;
    [SerializeField] private InventoryHolder inventoryHolder;

    public InventorySlot activeSlot;
    public GameObject currentItemObject;
    public Coroutine dropItemProcess;
    public float timeBefDrop;
    public static event Action<GameObject> OnHandItemChanged;

    public Action<InputAction.CallbackContext> pickHandle;
    public Action<InputAction.CallbackContext> dropHandle;
    public Action<InputAction.CallbackContext> useHandle;
    
    public bool isDropping;

    public float pickUpRadius = 5;
    public LayerMask itemLayer;


    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!IsOwner) return;
        pickHandle = c => RequestPickItem();
        dropHandle = c =>
        {
            RequestDropItemServerRpc();
            OnHandItemChanged?.Invoke(currentItemObject);
        };

        
        useHandle = c => RequestUseItemServerRpc();

        InputManager.inputActions.Player.Enable();
        InputManager.inputActions.Player.Interact.performed += pickHandle;
        InputManager.inputActions.Player.DropItem.performed += dropHandle;
        InputManager.inputActions.Player.UseItem.performed += useHandle;
    }

    private void OnDestroy()
    {
        if (!IsOwner) return;
        InputManager.inputActions.Player.Interact.performed -= pickHandle;
        InputManager.inputActions.Player.DropItem.performed -= dropHandle;
        InputManager.inputActions.Player.UseItem.performed -= useHandle;
    }
    
    private void RequestPickItem()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, pickUpRadius, itemLayer);
    
        if (colliders.Length == 0)
            return;

        PickAbleItem nearestItem = null;
        float minDistance = float.MaxValue;

        foreach (var col in colliders)
        {
            PickAbleItem item = col.GetComponent<PickAbleItem>();
            if (item == null)
                continue;

            float distance = Vector3.Distance(transform.position, col.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestItem = item;
            }
        }

        if (nearestItem != null)
        {
            nearestItem.PickUp(inventoryHolder);
            OnHandItemChanged?.Invoke(nearestItem.gameObject);
        }
    }
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
        
        activeSlot = slot;
        SyncSlotObserverRpc(slotIndex);
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

        UpdateHandItemServerRpc();
    }
    [ObserversRpc]
    private void SetIsDroppingObserversRpc(bool state)
    {
        isDropping = state;
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
    public void SyncSlotObserverRpc(int slotIndex)
    {
        InventorySlot slot = inventoryHolder.InventorySystem.InventorySlots[slotIndex];
        activeSlot = slot;
    }
    
    [ObserversRpc]
    private void SyncHandItemObserversRpc(NetworkObject itemNetObj)
    {
        currentItemObject = itemNetObj.gameObject;
        currentItemObject.transform.SetParent(dropPoint);
        currentItemObject.transform.localPosition = Vector3.zero;
        currentItemObject.transform.localRotation = Quaternion.identity;
        TurnOffItem(currentItemObject);
        OnHandItemChanged?.Invoke(currentItemObject);
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
        if (item.TryGetComponent<PickAbleItem>(out var pickup))
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
        if (item.TryGetComponent<PickAbleItem>(out var pickup))
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
    private void UseItem()
    {
        if (activeSlot == null || activeSlot.ItemData == null || currentItemObject == null) return;

        IUsable usableComponent = currentItemObject.GetComponent<IUsable>();
        if (usableComponent != null)
        {
            usableComponent.Use(gameObject);

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
        OnHandItemChanged?.Invoke(currentItemObject);

    }

    private IEnumerator ServerDropItem()
    {
        yield return new WaitForSeconds(timeBefDrop);

        GameObject droppedItem = Instantiate(
            activeSlot.ItemData.ItemPrefab,
            dropPoint.position,
            Quaternion.identity
        );

        Spawn(droppedItem);

        TurnOnItemServerRpc(droppedItem);
        ThrowHeldItemServerRpc(transform.forward, droppedItem);

        activeSlot.RemoveFromStack(1);

        if (activeSlot.StackSize <= 0)
            activeSlot.ClearSlot();

        UpdateHandItemServerRpc();
        OnHandItemChanged?.Invoke(currentItemObject);

        dropItemProcess = null;
        SetIsDroppingObserversRpc(false);
    }

    
    [ServerRpc(RequireOwnership = false)]
    private void ThrowHeldItemServerRpc(Vector3 direction, GameObject droppedItem)
    {
        if (droppedItem.TryGetComponent<ThrowableObjects>(out var throwable))
        {
            throwable.Throw(direction);
        }
    }
    [ServerRpc(RequireOwnership = false)]
    private void RequestUseItemServerRpc()
    {
        UseItem();
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void RequestDropItemServerRpc()
    {
        if (activeSlot == null || activeSlot.ItemData == null)
            return;

        if (dropItemProcess != null)
            return;

        dropItemProcess = StartCoroutine(ServerDropItem());
        SetIsDroppingObserversRpc(true);
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
        if (item.TryGetComponent<PickAbleItem>(out var pickup))
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
        if (item.TryGetComponent<PickAbleItem>(out var pickup))
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
