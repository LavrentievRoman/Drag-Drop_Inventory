using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;

[CreateAssetMenu(fileName = "New Target Object", menuName = "Objects/New Target Object")]
public class TargetObject : InteractiveObject
{
    [Header("Target Object Params")]
    [SerializeField] private TestEvent _event; // �������� ����������� ��� ������������� ������� ��������

    // ������������� �������� �� �������
    public override void UseKeyItem(PickableItem _keyItem)
    {
        if (_keyItem is KeyItem && IsActive)
        {
            // �������, ��� ������������ ������� ������������� ����������
            if (_keyItem == keyItem)
            {
                _keyItem.UseItem();

                ExecuteEvent();

                IsActive = false;
            }
            else
            {
                RaiseMessage($"������� {_keyItem.Name} �� ��������");
            }
        }
        else
        {
            RaiseMessage($"��� ������ ������������");
        }
    }

    // ���������� ��������
    private void ExecuteEvent()
    {
        _event?.Raise();
    }
}