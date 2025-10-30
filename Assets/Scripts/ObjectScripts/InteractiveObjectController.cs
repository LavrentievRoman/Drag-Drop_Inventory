using System;
using UnityEngine;

public class InteractiveObjectController : MonoBehaviour
{
    // Событее на движение к этому обекту
    public static event Action<Vector2, Action<PickableItem>> MoveTo;
    // Событее на подбор этого предмета
    public static event Action<PickableItem, int, Action<int>> Pickup;
    // Событее на показ имени этого объекта
    public static event Action<string, int> LookAt;

    [SerializeField] private ActiveObject activeObject;

    [Header("PickableItem Params")]
    [SerializeField] private int amount;

    // При наведении курсора на объект
    private void OnMouseEnter()
    {
        // Если с объектом ещё можно проивзоимодействовать
        if (activeObject.IsActive)
        {     
            // Показываем имя объекта
            LookAt?.Invoke(activeObject.Name, amount);
        }
    }

    // При отведении курсора от объекта
    private void OnMouseExit()
    {
        LookAt?.Invoke(null, 0);
    }

    // Обработка взаимодействия с объектом
    public void Interact(PickableItem selectItem)
    {
        // Если с объектом ещё можно проивзоимодействовать
        if (activeObject.IsActive)
        {
            // Смотрим на наличее выбранного предмета
            if (selectItem == null)
            {
                MoveTo?.Invoke((Vector2)transform.position, InteractWithObject);
            }
            else
            {
                // Смотрим на то, а можно ли вообще применять предмет к данному типу объекта
                switch (activeObject.ObjectType)
                {
                    case EActiveObjectType.Target:
                        MoveTo?.Invoke((Vector2)transform.position, InteractWithObject);
                        break;
                }
            }
        }
    }

    // Запуск взаимодействия с объектом
    private void InteractWithObject(PickableItem selectItem)
    {
        // Выбироаем взаимодействие с объектом в зависимости от его типа и наличия выбранного предмета
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

    // Подбор предмета
    private void PickUp()
    {
        Pickup?.Invoke((PickableItem)activeObject, amount, ReturnRemains);
    }

    // Возвращение остатков после подбора
    private void ReturnRemains(int reamains)
    {
        if (reamains <= 0)
            Destroy(gameObject);
        else amount = reamains;
    }
}
