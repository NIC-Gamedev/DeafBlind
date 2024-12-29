using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Inventory System/Inventory Item")]
public class InventoryItemData : ScriptableObject
{
    public int ID;
    public string DisplayName;
    [TextArea(4, 4)]
    public string Description;
    public Sprite Icon;
    public int MaxStackSize;
    public bool isUsable;
    public bool isQuestItem;
    public bool isOneUse;
    public GameObject ItemPrefab;
    public List<ItemField> Fields = new List<ItemField>();

}
[System.Serializable]
public class ItemField
{
    public string FieldName; // Название поля
    public string FieldType; // Тип поля (например, "int", "float", "bool")
    public string FieldValue; // Значение поля в виде строки
}