using UnityEngine;

[CreateAssetMenu(fileName = "New Heal Item", menuName = "Items/New Heal Item")]
public class HealItem : PickableItem
{
    // Количество здоровья воснатавливаемого предметом   
    [SerializeField] private float healAmount;

    public override void UseItem()
    {
        Debug.Log($"Захилили {healAmount} хп");
    }
}
