using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon Item", menuName = "Items/New Weapon Item")]

public class WeaponItem : PickableItem
{
    // Экипировка оружия
    public override void UseItem()
    {
        Debug.Log($"{Name} экипирован");
    }
}
