using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public TMP_Text item1, item2, item3;

    public HandHolder HandHolder;
    public InventoryHolder InventoryHolder;

    private void OnEnable()
    {
        HandHolder.OnHandItemChanged += HandleHandItemChanged;
        UpdateInventoryTexts(); // Обновляем UI при активации
    }

    private void OnDisable()
    {
        HandHolder.OnHandItemChanged -= HandleHandItemChanged;
    }

    private void HandleHandItemChanged(GameObject newItem)
    {
        UpdateInventoryTexts();
    }

    private void UpdateInventoryTexts()
    {
        var slots = InventoryHolder.InventorySystem.InventorySlots;

        UpdateSlotText(item1, slots, 0);
        UpdateSlotText(item2, slots, 1);
        UpdateSlotText(item3, slots, 2);
    }

    private void UpdateSlotText(TMP_Text textMesh, List<InventorySlot> slots, int index)
    {
        if (index >= slots.Count || slots[index].ItemData == null)
        {
            textMesh.text = "<empty>";
            return;
        }

        var slot = slots[index];
        string name = slot.ItemData.DisplayName;

        bool isActive = HandHolder.activeSlot == slot;
        textMesh.text = isActive ? $"<color=yellow>{name}</color>" : name;
    }
}
