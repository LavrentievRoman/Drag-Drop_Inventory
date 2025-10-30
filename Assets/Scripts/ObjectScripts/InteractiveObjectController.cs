using System;
using UnityEngine;

public class InteractiveObjectController : MonoBehaviour
{
    // ������� �� �������� � ����� ������
    public static event Action<Vector2, Action<PickableItem>> MoveTo;
    // ������� �� ������ ����� ��������
    public static event Action<PickableItem, int, Action<int>> Pickup;
    // ������� �� ����� ����� ����� �������
    public static event Action<string, int> LookAt;

    [SerializeField] private ActiveObject activeObject;

    [Header("PickableItem Params")]
    [SerializeField] private int amount;

    // ��� ��������� ������� �� ������
    private void OnMouseEnter()
    {
        // ���� � �������� ��� ����� ���������������������
        if (activeObject.IsActive)
        {     
            // ���������� ��� �������
            LookAt?.Invoke(activeObject.Name, amount);
        }
    }

    // ��� ��������� ������� �� �������
    private void OnMouseExit()
    {
        LookAt?.Invoke(null, 0);
    }

    // ��������� �������������� � ��������
    public void Interact(PickableItem selectItem)
    {
        // ���� � �������� ��� ����� ���������������������
        if (activeObject.IsActive)
        {
            // ������� �� ������� ���������� ��������
            if (selectItem == null)
            {
                MoveTo?.Invoke((Vector2)transform.position, InteractWithObject);
            }
            else
            {
                // ������� �� ��, � ����� �� ������ ��������� ������� � ������� ���� �������
                switch (activeObject.ObjectType)
                {
                    case EActiveObjectType.Target:
                        MoveTo?.Invoke((Vector2)transform.position, InteractWithObject);
                        break;
                }
            }
        }
    }

    // ������ �������������� � ��������
    private void InteractWithObject(PickableItem selectItem)
    {
        // ��������� �������������� � �������� � ����������� �� ��� ���� � ������� ���������� ��������
        if (selectItem == null)
        {
            switch (activeObject.ObjectType)
            {
                case EActiveObjectType.Pickable:
                    PickUp();
                    break;

                default:
                    activeObject.ShowDescription();
                    break;
            }
        }
        else
        {
            switch (activeObject.ObjectType)
            {
                case EActiveObjectType.Target:               
                    ((InteractiveObject)activeObject).UseKeyItem(selectItem);
                    break;
            }
        }
    }

    // ������ ��������
    private void PickUp()
    {
        Pickup?.Invoke((PickableItem)activeObject, amount, ReturnRemains);
    }

    // ����������� �������� ����� �������
    private void ReturnRemains(int reamains)
    {
        if (reamains <= 0)
            Destroy(gameObject);
        else amount = reamains;
    }
}
