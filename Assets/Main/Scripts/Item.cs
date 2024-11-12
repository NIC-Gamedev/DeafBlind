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
    //≈сли хотем четко создать какой то обьект используем его, но или если чего то нехватает то просто его наследуем и добавл€ем туда что то нужное 
    //[CreateAssetMenu(fileName = "New Potion", menuName = "Inventory/Potion")]

    [Header("ќсновные данные предмета")]
    public string itemName;            
    public Sprite itemIcon;            
    public ItemType itemType;          
    [TextArea] public string description; 

    [Header("ƒополнительные параметры")]
    public int maxStack = 1;           
    public bool isQuestItem;           

   
    public abstract void UseItem(GameObject user);
}