using System.Collections;
using System.Collections.Generic;
using FishNet;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

public class HandHolder : NetworkBehaviour
{
    [Header("Hand Settings")]
    [SerializeField] private Transform dropPoint;
    [SerializeField] private InventoryHolder inventoryHolder;

    public InventorySlot activeSlot;
    public GameObject currentItemObject;

    public Coroutine dropItemProcess;

    public float dropForce = 2;

    public bool dropItemFlag;

    public float timeBefDrop;

    private void Update()
    {
        dropItemFlag = dropItemProcess != null;
        if (!IsOwner) return;
        HandleInput();
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

    private void SelectItem(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= inventoryHolder.InventorySystem.InventorySize) return;

        InventorySlot slot = inventoryHolder.InventorySystem.InventorySlots[slotIndex];
        if (slot.ItemData == null) return;

        activeSlot = slot;
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
                Spawn(netObj, Owner);
                currentItemObject = item;
                UpdateHandItemClientRpc((ushort)netObj.ObjectId);
            }
        }
    }

    [ObserversRpc]
    private void UpdateHandItemClientRpc(ushort objectId)
    {
        NetworkObject netObj = InstanceFinder.ServerManager.Objects.Spawned[objectId];
        
        
        if (netObj != null)
        {
            currentItemObject = netObj.gameObject;
            currentItemObject.transform.SetParent(dropPoint);
            currentItemObject.transform.localPosition = Vector3.zero;
            currentItemObject.transform.localRotation = Quaternion.identity;
            TurnOffItem(currentItemObject);
        }
    }

    private void TurnOffItem(GameObject item)
    {
        if (item.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.useGravity = false;
            rb.isKinematic = true;
        }

        if (item.TryGetComponent<ItemPickUp>(out var pickup))
        {
            pickup.enabled = false;
        }

        foreach (var col in item.GetComponents<Collider>())
        {
            col.enabled = false;
        }
    }

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
    }

    private IEnumerator DropItem()
    {
        yield return new WaitForSeconds(timeBefDrop);
        
        GameObject droppedItem = Instantiate(
            activeSlot.ItemData.ItemPrefab,
            dropPoint.position,
            Quaternion.identity
        );

        if (droppedItem.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.AddForce(transform.forward * dropForce, ForceMode.Impulse);
        }

        if (droppedItem.TryGetComponent<NetworkObject>(out var netObj))
        {
            Spawn(netObj);
        }

        activeSlot.RemoveFromStack(1);

        if (activeSlot.StackSize <= 0)
        {
            activeSlot.ClearSlot();
        }

        UpdateHandItemServerRpc();

        dropItemProcess = null;
    }

    [ServerRpc]
    private void RequestUseItemServerRpc()
    {
        UseItem();
    }

    [ServerRpc]
    private void RequestDropItemServerRpc()
    {
        if(activeSlot == null || activeSlot.ItemData == null)
            return;
        
        if(dropItemProcess == null)
            dropItemProcess = StartCoroutine(DropItem());
    }
}
