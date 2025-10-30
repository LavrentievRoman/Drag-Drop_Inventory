using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;

[CreateAssetMenu(fileName = "New Target Object", menuName = "Objects/New Target Object")]
public class TargetObject : InteractiveObject
{
    [Header("Target Object Params")]
    [SerializeField] private TestEvent _event; // Действие выполняемое при использовании нужного пердмета

    // Использование предмета на объекте
    public override void UseKeyItem(PickableItem _keyItem)
    {
        if (_keyItem is KeyItem && IsActive)
        {
            // Смотрим, что используемый предмет соответствует ожидаемому
            if (_keyItem == keyItem)
            {
                _keyItem.UseItem();

                ExecuteEvent();

                IsActive = false;
            }
            else
            {
                RaiseMessage($"Предмет {_keyItem.Name} не подходит");
            }
        }
        else
        {
            RaiseMessage($"Это нельзя использовать");
        }
    }

    // Производим действие
    private void ExecuteEvent()
    {
        _event?.Raise();
    }
}