using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon Item", menuName = "Items/New Weapon Item")]

public class WeaponItem : PickableItem
{
    // ���������� ������
    public override void UseItem()
    {
        Debug.Log($"{Name} ����������");
    }
}
