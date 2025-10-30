using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Inventory/Item Database", order = 1)]
public class ItemDataBase : ScriptableObject
{
    // База даннып предметов
    [field: SerializeField] private List<PickableItem> Items { get; set; } 

    public PickableItem GetByID(string id)
    {
        return Items.Find(item => item.ItemID == id);
    }
}

// Сериализуемы параметры предмета в инвенторе
[Serializable]
public class InventoryItem
{
    public string id;
    public int amount;
    public int index;

    public InventoryItem(string _id, int _amount, int _index)
    {
        id = _id;
        amount = _amount;
        index = _index;
    }
}

// Инвентарь
[Serializable]
public class InventoryData
{
    public List<InventoryItem> items;
}
