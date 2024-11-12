using UnityEngine;

public enum ItemType
{
    Weapon,
    Armor,
    Potion,
    QuestItem,
    Miscellaneous
}

public abstract class ItemData : ScriptableObject
{
    //���� ����� ����� ������� ����� �� ������ ���������� ���, �� ��� ���� ���� �� ��������� �� ������ ��� ��������� � ��������� ���� ��� �� ������ 
    //[CreateAssetMenu(fileName = "New Potion", menuName = "Inventory/Potion")]

    [Header("�������� ������ ��������")]
    public string itemName;            
    public Sprite itemIcon;            
    public ItemType itemType;          
    [TextArea] public string description; 

    [Header("�������������� ���������")]
    public int maxStack = 1;           
    public bool isQuestItem;           

   
    public abstract void UseItem(GameObject user);
}