using UnityEngine;

[CreateAssetMenu(fileName = "New Key Item", menuName = "Items/New Key Item")]
public class KeyItem : PickableItem
{
    [Header("Key Item Params")]
    [SerializeField] private UseItemEvent useItemEvent;

    public override void UseItem()
    {
        useItemEvent?.Raise();
    }
}
