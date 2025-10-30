using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // Событее на выбор предмета в слоте
    public static event Action<InventorySlot> SelectItem;
    // Событее на складирование предмета в слот
    public static event Action<InventorySlot> PutItem;
    // Событее на смену предмета в слоте
    public static event Action<InventorySlot> SwapItems;

    // Событмя на показ информации о предмете в слоте
    public static event Action<string, string> ShowTooltip;
    public static event Action HideTooltip;

    [Header("Slot Parts")]
    [SerializeField] private Image itemImage; // Изображение предмета
    [SerializeField] private TMP_Text itemAmountText; // Текст количества

    // Предмет, который лежит в слоте
    public PickableItem Item
    {
        get => _item;
        set
        {
            _item = value;

            if (value == null)
            {
                itemImage.sprite = null;
                itemImage.enabled = false;
            }
            else
            {
                itemImage.enabled = true;
                itemImage.sprite = value.ItemSprite;
            }
        }
    }
    private PickableItem _item = null;

    // Количество предметов в слоте
    public int Amount
    {
        get => _amount;
        set
        {
            _amount = value;

            if (_amount <= 1)
            {
                itemAmountText.text = "";
            }
            else
            {
                itemAmountText.text = _amount.ToString();
            }
        }
    }
    private int _amount = -1;

    public static bool operator >(InventorySlot left, InventorySlot right)
    {
        if (left.Item == null && right.Item == null) return false;
        else if (left.Item == null && right.Item != null) return false;
        else if (left.Item != null && right.Item == null) return true;
        else if (left.Item.ItemType == EItemType.Key && (right.Item.ItemType == EItemType.Weapon || right.Item.ItemType == EItemType.Heal))
            return true;
        else if (left.Item.ItemType == EItemType.Weapon && right.Item.ItemType == EItemType.Heal) return true;
        else if (left.Item.ItemType == right.Item.ItemType)
        {
            if (String.Compare(left.Item.Name, right.Item.Name) < 0) return true;
            else if (String.Compare(left.Item.Name, right.Item.Name) > 0) return false;
            else
            {
                if (left.Amount >= right.Amount) return true;
                else return false;
            }
        }

        return false;
    }

    public static bool operator <(InventorySlot left, InventorySlot right)
    {
        if (left.Item == null && right.Item == null) return false;
        else if (left.Item == null && right.Item != null) return true;
        else if (left.Item != null && right.Item == null) return false;
        else if (left.Item.ItemType == EItemType.Heal && (right.Item.ItemType == EItemType.Weapon || right.Item.ItemType == EItemType.Key))
            return true;
        else if (left.Item.ItemType == EItemType.Weapon && right.Item.ItemType == EItemType.Key) return true;
        else if (left.Item.ItemType == right.Item.ItemType)
        {
            if (String.Compare(left.Item.Name, right.Item.Name) < 0) return false;
            else if (String.Compare(left.Item.Name, right.Item.Name) > 0) return true;
            else
            {
                if (left.Amount >= right.Amount) return false;
                else return true;
            }
        }

        return false;
    }

    // При наведении курсора на слот
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Показываем имя предмета в слоте
        if (Item != null)
            ShowTooltip?.Invoke(Item.Name, Item.Description);
    }

    // При отведении курсора от слота
    public void OnPointerExit(PointerEventData eventData)
    {
        // Скрываем имя предмета в слоте
        HideTooltip?.Invoke();
    }

    // Метод взаимодействия со слотом
    public void InteractWithSlot(PickableItem selectItem)
    {
        if (selectItem == null && Item != null) // Если нужно взять предмет
        {
            SelectItem?.Invoke(this);
        }
        else if (selectItem != null && Item == null) // Если нужно положить предмет
        {
            PutItem?.Invoke(this);
        }
        else if (selectItem != null && Item != null) // Если нужно поменять предметы местами
        {
            SwapItems?.Invoke(this);
        }
    }

    // Использование предмета в слоте
    public void UseItemInSlot()
    {
        if (Item != null)
        {
            Item.UseItem();

            // Если это расходник уменьшаем количество
            if (Item.ItemType == EItemType.Heal)
            {
                Amount--;
                // Если закончился удаляем
                if (Amount <= 0)
                {
                    Item = null;
                }
            }
        }
    }

    // Удаление пердмета из слота
    public void DeleteItemInSlot()
    {
        Item = null;
        Amount = 0;
    }
}
