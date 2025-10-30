using System;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;

public abstract class InteractiveObject : ActiveObject
{
    [Header("Interactive Object Params")]
    // Требуемый предмет
    [SerializeField] protected KeyItem keyItem;

    public abstract void UseKeyItem(PickableItem _keyItem);
}
