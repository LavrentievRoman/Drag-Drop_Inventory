using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    // Сохранение инвентаря
    public static void SaveInventory(InventoryData inventory)
    {
        string json = JsonUtility.ToJson(inventory, true);

        File.WriteAllText(Application.persistentDataPath + "/playerInventory.json", json);
    }

    // Загрузка инвентаря
    public static InventoryData LoadInventory()
    {
        string filePath = Application.persistentDataPath + "/playerInventory.json";

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);

            InventoryData data = JsonUtility.FromJson<InventoryData>(json);

            return data;
        }
        return null;
    }
}
