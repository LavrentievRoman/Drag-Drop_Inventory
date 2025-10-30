using UnityEngine;

[CreateAssetMenu(fileName = "New Heal Item", menuName = "Items/New Heal Item")]
public class HealItem : PickableItem
{
    // ���������� �������� ����������������� ���������   
    [SerializeField] private float healAmount;

    public override void UseItem()
    {
        Debug.Log($"�������� {healAmount} ��");
    }
}
