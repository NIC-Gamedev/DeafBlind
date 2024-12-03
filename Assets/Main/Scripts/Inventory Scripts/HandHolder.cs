using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandHolder : MonoBehaviour
{
    [Header("Hand Settings")]
    [SerializeField] private Transform dropPoint; // Where the item will be dropped
    [SerializeField] private InventoryHolder inventoryHolder; // Reference to the player's inventory

    public InventorySlot activeSlot; // The currently selected inventory slot
    public GameObject currentItemObject; // The current item in hand

    
    private void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Q)) // Press 'Q' to drop the currently selected item
        {
            DropItem();
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
        UpdateHandItem();
    }

    private void UpdateHandItem()
    {
        if (currentItemObject != null)
        {
            Destroy(currentItemObject);
        }
        
        if (activeSlot != null && activeSlot.ItemData != null && activeSlot.ItemData.ItemPrefab != null)
        {
            currentItemObject = Instantiate(activeSlot.ItemData.ItemPrefab, dropPoint.position, dropPoint.rotation, dropPoint);
            currentItemObject.GetComponent<Rigidbody>().useGravity = false;
            currentItemObject.GetComponent<Rigidbody>().isKinematic = true;
            currentItemObject.GetComponent<ItemPickUp>().enabled = false;
            currentItemObject.GetComponent<CapsuleCollider>().enabled = false;
        }
    }

    private void DropItem()
    {
        if (activeSlot == null || activeSlot.ItemData == null) return;

        // Instantiate the item prefab in the world
        GameObject droppedItem = Instantiate(
            activeSlot.ItemData.ItemPrefab,
            dropPoint.position,
            Quaternion.identity
        );

        if (droppedItem.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.AddForce(transform.forward * 2f, ForceMode.Impulse); // Add some force to make it fall forward
        }

        // Remove the item from the inventory
        activeSlot.RemoveFromStack(1);

        // If the stack size reaches 0, clear the slot
        if (activeSlot.StackSize <= 0)
        {
            activeSlot.ClearSlot();
        }

        UpdateHandItem(); // Update the hand after dropping the item
    }
}
