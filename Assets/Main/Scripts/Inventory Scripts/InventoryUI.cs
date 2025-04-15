using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InventoryUI : NetworkBehaviour
{
    public TMP_Text item1, item2, item3;
    public HandHolder HandHolder;
    public InventoryHolder InventoryHolder;

    // ���� ����� ���������� ��� ������������� �������� UI.
    // ��������� ��� ������ ������� ��������� � ���������� InventoryHolder.
    public void RequestInventoryUpdate()
    {
        // ���������� � ��������� �������,
        // ����� ������ ���������� ���������� ������.
        if (IsOwner)
        {
            RequestInventorySlotsFromServerRpc();
        }
    }

    // RPC �� ������� � �������. ����������� ������ ������ ���������.
    [ServerRpc(RequireOwnership = false)]
    private void RequestInventorySlotsFromServerRpc()
    {
        // ����� ������ ���������� � ������ ���������� InventoryHolder
        // � �������� ���������� ������ ������.
        var slots = InventoryHolder.InventorySystem.InventorySlots;

        // �������������� ������ ����� ��� ��������: ���� �������� �������� ��� ����������.
        string[] slotNames = new string[3]; // �����������, ��� ��� ���������� ������ 3 �����
        for (int i = 0; i < slotNames.Length; i++)
        {
            if (i >= slots.Count || slots[i].ItemData == null)
            {
                slotNames[i] = "<empty>";
            }
            else
            {
                slotNames[i] = slots[i].ItemData.DisplayName;
            }
        }

        // ���������� �������� ����.
        // ���� HandHolder.activeSlot ��������� �� ����, ����� �������� ������ ��������� �����.
        int activeIndex = GetActiveSlotIndex(slots);

        // �������� RPC �� �������� ��� ���������� UI.
        UpdateInventoryUIClientRpc(slotNames, activeIndex);
    }

    // ������������� ����� ��� ����������� ������� ��������� �����.
    private int GetActiveSlotIndex(List<InventorySlot> slots)
    {
        // �����������, ��� HandHolder.activeSlot ��������� �� ������ ���� InventorySlot,
        // ���������� ��� � ���������� ������.
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i] == HandHolder.activeSlot)
            {
                return i;
            }
        }
        return -1; // ���� �������� ���� �� ������
    }

    // RPC �� ������� � ������������ (��� �������), ��������� UI.
    [ObserversRpc]
    private void UpdateInventoryUIClientRpc(string[] slotNames, int activeIndex)
    {
        // ��������� ��������� ���� UI.
        // �������� �������� ���� (��������, ������ ������).
        if (slotNames.Length > 0)
            item1.text = activeIndex == 0 ? $"<color=yellow>{slotNames[0]}</color>" : slotNames[0];
        if (slotNames.Length > 1)
            item2.text = activeIndex == 1 ? $"<color=yellow>{slotNames[1]}</color>" : slotNames[1];
        if (slotNames.Length > 2)
            item3.text = activeIndex == 2 ? $"<color=yellow>{slotNames[2]}</color>" : slotNames[2];
    }

    // ������ �������� �� ������� ��������� �������� � ���� (���� ��������� ���������� UI �����)
    private void OnEnable()
    {
        HandHolder.OnHandItemChanged += HandleHandItemChanged;
        // ������ ������� ���������� UI �� ���������� ��������,
        // ���������� � ������� �� ����������� �������.
        RequestInventoryUpdate();
    }

    private void OnDisable()
    {
        HandHolder.OnHandItemChanged -= HandleHandItemChanged;
    }

    private void HandleHandItemChanged(GameObject newItem)
    {
        RequestInventoryUpdate();
    }
}
