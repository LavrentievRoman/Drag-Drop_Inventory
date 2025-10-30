using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    // ������� �� ��������� ���������� ��������
    public static event Action<PickableItem> SetSelectedItem;
    // ������� �� ������� ����� ������ � ���������
    public static event Action HideInventoryMessages;

    [SerializeField] private Camera mainCamera;

    [SerializeField] private GameObject inventory;

    [SerializeField] private ItemDataBase itemDataBase;

    [Header("Inventory Parts")]
    [SerializeField] private GameObject inventorySlotsPanel; // ������ �� ������� ���������
    [SerializeField] private GameObject selectedItemGO;

    [Header("Events")]
    [SerializeField] private UseItemEvent useItemEvent;
    [SerializeField] private ShowPlayerMessageEvent InventoryFullMessage;

    private InventorySlot selectedSlot;
    private (PickableItem, int) selectedSlotItem;

    private Image selectedItemImage;
    private bool isSelect = false;

    private List<InventorySlot> slots;

    private void Awake()
    {
        InventorySlot.SelectItem += SelectItem;
        InventorySlot.PutItem += PutItemInSlot;
        InventorySlot.SwapItems += SwapItems;

        InteractiveObjectController.Pickup += AddItem;

        useItemEvent.RegisterListener(ItemUsed);

        InputManager.OpenInventory += OpenInventory;
        InputManager.DragItem += DragItem;
        InputManager.DeleteItem += DeleteItem;
        InputManager.SaveInventory += SaveInventory;
        InputManager.LoadInventory += LoadInventory;
        InputManager.SortInventory += SortInventory;

        PlayerController.UseItemOnPlayer += UseSelectedItem;
        PlayerController.ReturnSelectedItem += ReturnSelectItemInSlot;
        PlayerController.DropItem += DeleteItem;

        selectedItemImage = selectedItemGO.GetComponent<Image>();

        slots = new();
        for (int i = 0; i < inventorySlotsPanel.transform.childCount; i++)
        {
            if (inventorySlotsPanel.transform.GetChild(i).TryGetComponent<InventorySlot>(out var slot))
            {
                slots.Add(slot);
            }
        }
    }

    private void LateUpdate()
    {
        // ���� ���� ��������� ������� ���������� ��� ��������� �� ������
        if (isSelect)
        {
            MouseFollow();
        }
    }

    // ���������� ���������� �������� �� ������
    private void MouseFollow()
    {
        // ��������� ������� ������� � ��������� ������
        Vector3 pos = Input.mousePosition;
        pos.z = 99;

        // ����������� ������� �������
        selectedItemGO.transform.position = mainCamera.ScreenToWorldPoint(pos);
    }

    // ������ �������������� ��������
    private void DragItem(bool isDrag)
    {
        isSelect = isDrag;
    }

    // ����� ��������
    private void SelectItem(InventorySlot slot)
    {
        // ���������� ����
        selectedSlot = slot;

        // ���������� ���������� � �������� � �����
        selectedSlotItem.Item1 = slot.Item.Clone<PickableItem>();
        selectedSlotItem.Item2 = slot.Amount;

        // ���������� ���������� ������� ��������� �� ������
        selectedItemImage.enabled = true;
        selectedItemImage.sprite = selectedSlotItem.Item1.ItemSprite;

        // ��������� selectedItem � PlayerController
        SetSelectedItem?.Invoke(selectedSlotItem.Item1);

        // ������� ������� �� �����
        slot.Item = null;
        slot.Amount = -1;
    }

    // ������������� ��������
    private void PutItemInSlot(InventorySlot slot)
    {
        // ������� ������� � ��� ���������� � ����
        slot.Item = selectedSlotItem.Item1.Clone<PickableItem>();
        slot.Amount = selectedSlotItem.Item2;

        // ��������� �������� ���������� ��������
        selectedItemImage.enabled = false;
        isSelect = false;

        // ��������� selectedItem � PlayerController
        SetSelectedItem?.Invoke(null);
    }

    // ����� ��������� �������
    private void SwapItems(InventorySlot slot)
    {
        if (selectedSlot.Item != slot.Item) // ���� �������� ������
        {
            // ������ �������� �������
            selectedSlot.Item = slot.Item.Clone<PickableItem>();
            selectedSlot.Amount = slot.Amount;

            slot.Item = selectedSlotItem.Item1.Clone<PickableItem>();
            slot.Amount = selectedSlotItem.Item2;
        }
        else // ���� ����������
        {
            // ��� ������������ �����
            if (slot.Amount + selectedSlotItem.Item2 > slot.Item.MaximumAmount)
            {
                selectedSlotItem.Item2 -= slot.Item.MaximumAmount - slot.Amount;
                slot.Amount = slot.Item.MaximumAmount;

                selectedSlot.Item = selectedSlotItem.Item1.Clone<PickableItem>();
                selectedSlot.Amount = selectedSlotItem.Item2;
            }
            else
            {
                slot.Amount += selectedSlotItem.Item2;
            }
        }

        // ��������� �������� ���������� ��������
        selectedItemImage.enabled = false;
        isSelect = false;

        // ��������� selectedItem � PlayerController
        SetSelectedItem?.Invoke(null);
    }

    // ���������� �������� � ���������
    private void AddItem(PickableItem item, int amount, Action<int> returnRemains)
    {
        // ������� ���� ������� �������
        foreach (var slot in slots)
        {
            if (slot.Item == item && amount > 0)
            {
                // ������� ����� �������� ���������� �� ��������� ������������
                if (slot.Amount + amount > slot.Item.MaximumAmount)
                {
                    amount -= slot.Item.MaximumAmount - slot.Amount;
                    slot.Amount = slot.Item.MaximumAmount;
                }
                else
                {
                    slot.Amount += amount;
                    amount = 0;
                }
            }
        }

        // ���� ������� ������� �� ����� ��� ��� ����� ���������, �� ���� ������ ����
        foreach (var slot in slots)
        {
            if (slot.Item == null && amount > 0)
            {
                slot.Item = item.Clone<PickableItem>();
                slot.Amount = amount;
                amount = 0;
            }
        }

        if (amount > 0)
            InventoryFullMessage?.Raise("��������� ��������");

        returnRemains?.Invoke(amount);
    }

    // ���������� ���������
    private void SortInventory()
    {
        for (int i = 0; i < slots.Count - 1; i++)
        {
            for (int j = 0; j < slots.Count - i - 1; j++)
            {
                if (slots[j] < slots[j + 1])
                {
                    PickableItem item = slots[j].Item != null ? slots[j].Item.Clone<PickableItem>() : null;
                    int amount = slots[j].Item != null ? slots[j].Amount : 0;

                    slots[j].Item = slots[j + 1].Item != null ? slots[j + 1].Item.Clone<PickableItem>() : null;
                    slots[j].Amount = slots[j + 1].Item != null ? slots[j + 1].Amount : 0;

                    slots[j + 1].Item = item != null ? item.Clone<PickableItem>() : null;
                    slots[j + 1].Amount = item != null ? amount : 0;
                }
            }
        }
    }

    // ���� ��������� ������� �����������
    private void ItemUsed()
    {
        // ��������� selectedItem � PlayerController
        SetSelectedItem?.Invoke(null);

        // ��������� �������� ���������� ��������
        selectedItemImage.enabled = false;
        isSelect = false;
    }

    // ������������� ��������� �� ���������
    private void UseSelectedItem()
    {
        // ���������� ��������� ������� �� �����
        ReturnSelectItemInSlot();

        switch (selectedSlot.Item.ItemType)
        {
            case EItemType.Key:
                break;
            default:
                selectedSlot.UseItemInSlot();
                break;
        }
    }

    // ����������� ���������� �������� � ����
    private void ReturnSelectItemInSlot()
    {
        // ���������� ������� �� �����
        selectedSlot.Item = selectedSlotItem.Item1;
        selectedSlot.Amount = selectedSlotItem.Item2;

        // ��������� �������� ���������� ��������
        selectedItemImage.enabled = false;
        isSelect = false;

        // ��������� selectedItem � PlayerController
        SetSelectedItem?.Invoke(null);
    }

    // �������� �������� ��� ������ ��� �� ������� ���������
    private void DeleteItem(List<RaycastResult> uiElems)
    {
        foreach (RaycastResult elem in uiElems)
        {
            if (elem.gameObject.TryGetComponent<InventorySlot>(out var slot))
            {
                // ���� ������� ��������, �� ������ ���������� ��� �� �����
                if (slot.Item.ItemType == EItemType.Key)
                {
                    ReturnSelectItemInSlot();
                    return;
                }

                slot.DeleteItemInSlot();
            }
        }
    }

    // �������� �������� ����� ������� ������
    private void DeleteItem()
    {
        // ���������, ����� ������� ��� �� ��������
        if (selectedSlotItem.Item1.ItemType == EItemType.Key)
            return;      

        // ������� �������
        selectedSlot.DeleteItemInSlot();

        // ��������� �������� ���������� ��������
        selectedItemImage.enabled = false;
        isSelect = false;

        // ��������� selectedItem � PlayerController
        SetSelectedItem?.Invoke(null);
    }

    // ��������� ��������/�������� ���������
    private void OpenInventory(bool open)
    {
        if (!open)
        {
            // ���� ���� ��������� �������, ���� ��� ������� � ����
            if (isSelect)
            {
                ReturnSelectItemInSlot();
            }

            // �������� ��� ��������� ���������
            HideInventoryMessages?.Invoke();
        }

        inventory.SetActive(open);
    }

    // ����������� ���������
    private void SaveInventory()
    {
        InventoryData inventory = new()
        {
            items = new()
        };

        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].Item != null)
            {
                inventory.items.Add(new InventoryItem(slots[i].Item.ItemID, slots[i].Amount, i));
            }
        }

        SaveManager.SaveInventory(inventory);
    }

    // �������� ���������
    private void LoadInventory()
    {
        InventoryData inventory = SaveManager.LoadInventory();

        if (inventory != null)
        {
            foreach (var slot in slots)
            {
                slot.Item = null;
                slot.Amount = 0;
            }

            foreach (var item in inventory.items)
            {
                slots[item.index].Item = itemDataBase.GetByID(item.id).Clone<PickableItem>();
                slots[item.index].Amount = item.amount;
            }
        }
    }
}
