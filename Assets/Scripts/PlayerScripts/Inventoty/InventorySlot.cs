using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // ������� �� ����� �������� � �����
    public static event Action<InventorySlot> SelectItem;
    // ������� �� ������������� �������� � ����
    public static event Action<InventorySlot> PutItem;
    // ������� �� ����� �������� � �����
    public static event Action<InventorySlot> SwapItems;

    // ������� �� ����� ���������� � �������� � �����
    public static event Action<string, string> ShowTooltip;
    public static event Action HideTooltip;

    [Header("Slot Parts")]
    [SerializeField] private Image itemImage; // ����������� ��������
    [SerializeField] private TMP_Text itemAmountText; // ����� ����������

    // �������, ������� ����� � �����
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

    // ���������� ��������� � �����
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

    // ��� ��������� ������� �� ����
    public void OnPointerEnter(PointerEventData eventData)
    {
        // ���������� ��� �������� � �����
        if (Item != null)
            ShowTooltip?.Invoke(Item.Name, Item.Description);
    }

    // ��� ��������� ������� �� �����
    public void OnPointerExit(PointerEventData eventData)
    {
        // �������� ��� �������� � �����
        HideTooltip?.Invoke();
    }

    // ����� �������������� �� ������
    public void InteractWithSlot(PickableItem selectItem)
    {
        if (selectItem == null && Item != null) // ���� ����� ����� �������
        {
            SelectItem?.Invoke(this);
        }
        else if (selectItem != null && Item == null) // ���� ����� �������� �������
        {
            PutItem?.Invoke(this);
        }
        else if (selectItem != null && Item != null) // ���� ����� �������� �������� �������
        {
            SwapItems?.Invoke(this);
        }
    }

    // ������������� �������� � �����
    public void UseItemInSlot()
    {
        if (Item != null)
        {
            Item.UseItem();

            // ���� ��� ��������� ��������� ����������
            if (Item.ItemType == EItemType.Heal)
            {
                Amount--;
                // ���� ���������� �������
                if (Amount <= 0)
                {
                    Item = null;
                }
            }
        }
    }

    // �������� �������� �� �����
    public void DeleteItemInSlot()
    {
        Item = null;
        Amount = 0;
    }
}
