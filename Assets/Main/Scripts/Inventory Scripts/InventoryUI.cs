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

    // Этот метод вызывается при необходимости обновить UI.
    // Вызывайте его вместо прямого обращения к локальному InventoryHolder.
    public void RequestInventoryUpdate()
    {
        // Вызывается у владельца объекта,
        // чтобы сервер подготовил актуальные данные.
        if (IsOwner)
        {
            RequestInventorySlotsFromServerRpc();
        }
    }

    // RPC от клиента к серверу. Запрашиваем список слотов инвентаря.
    [ServerRpc(RequireOwnership = false)]
    private void RequestInventorySlotsFromServerRpc()
    {
        // Здесь сервер обращается к своему экземпляру InventoryHolder
        // и получает актуальный список слотов.
        var slots = InventoryHolder.InventorySystem.InventorySlots;

        // Подготавливаем массив строк для передачи: берём название предмета или заменитель.
        string[] slotNames = new string[3]; // Предположим, что нас интересуют первые 3 слота
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

        // Определяем активный слот.
        // Если HandHolder.activeSlot указывает на слот, можно передать индекс активного слота.
        int activeIndex = GetActiveSlotIndex(slots);

        // Вызываем RPC на клиентах для обновления UI.
        UpdateInventoryUIClientRpc(slotNames, activeIndex);
    }

    // Помогательный метод для определения индекса активного слота.
    private int GetActiveSlotIndex(List<InventorySlot> slots)
    {
        // Предположим, что HandHolder.activeSlot ссылается на объект типа InventorySlot,
        // сравниваем его с элементами списка.
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i] == HandHolder.activeSlot)
            {
                return i;
            }
        }
        return -1; // Если активный слот не найден
    }

    // RPC от сервера к наблюдателям (все клиенты), обновляем UI.
    [ObserversRpc]
    private void UpdateInventoryUIClientRpc(string[] slotNames, int activeIndex)
    {
        // Обновляем текстовые поля UI.
        // Выделяем активный слот (например, желтым цветом).
        if (slotNames.Length > 0)
            item1.text = activeIndex == 0 ? $"<color=yellow>{slotNames[0]}</color>" : slotNames[0];
        if (slotNames.Length > 1)
            item2.text = activeIndex == 1 ? $"<color=yellow>{slotNames[1]}</color>" : slotNames[1];
        if (slotNames.Length > 2)
            item3.text = activeIndex == 2 ? $"<color=yellow>{slotNames[2]}</color>" : slotNames[2];
    }

    // Пример подписки на событие изменения предмета в руке (если требуется обновление UI сразу)
    private void OnEnable()
    {
        HandHolder.OnHandItemChanged += HandleHandItemChanged;
        // Вместо прямого обновления UI по локальному инстансу,
        // обращаемся к серверу за актуальными данными.
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
