using System.Collections.Generic;
using UnityEngine;

public abstract class PickableItem : ActiveObject
{
    [field: Header("Pickable Item Params")]
    [field: SerializeField] public string ItemID { get; private set; }

    [field: SerializeField] public EItemType ItemType { get; set; } // Тип предмета

    [field: SerializeField] public int MaximumAmount { get; set; } // Максимальное количество в стаке 

    [field: SerializeField] public Sprite ItemSprite { get; set; } // Спрайт предмета

    // Использование предмета
    public abstract void UseItem();

    public virtual T Clone<T>() where T : PickableItem
    {
        T clone = Instantiate(this) as T; 

        return clone;
    }

    public static bool operator ==(PickableItem left, PickableItem right)
    {
        if (ReferenceEquals(left, right)) return true;
        else if (left is null || right is null) return false;

        return (left.Name == right.Name && left.ItemSprite == right.ItemSprite);
    }

    public static bool operator !=(PickableItem left, PickableItem right)
    {
        return !(left == right);
    }

    public override bool Equals(object obj)
    {
        return obj is PickableItem other && this == other;
    }

    public override int GetHashCode()
    {
        return Name.GetHashCode();
    }
}
