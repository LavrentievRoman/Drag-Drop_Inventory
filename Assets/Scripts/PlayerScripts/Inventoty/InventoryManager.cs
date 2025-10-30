using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    // Событее на установку выбранного предмета
    public static event Action<PickableItem> SetSelectedItem;
    // Событее на скрытее всего текста в инвентаре
    public static event Action HideInventoryMessages;

    [SerializeField] private Camera mainCamera;

    [SerializeField] private GameObject inventory;

    [SerializeField] private ItemDataBase itemDataBase;

    [Header("Inventory Parts")]
    [SerializeField] private GameObject inventorySlotsPanel; // Панель со слотами инвентаря
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
        // Если есть выбранный предмет заставляем его следовать за мышкой
        if (isSelect)
        {
            MouseFollow();
        }
    }

    // Следование выбранного предмета за мышкой
    private void MouseFollow()
    {
        // Вычисляем позицию курсора и добавляем отступ
        Vector3 pos = Input.mousePosition;
        pos.z = 99;

        // Присваиваем позицию объекту
        selectedItemGO.transform.position = mainCamera.ScreenToWorldPoint(pos);
    }

    // Запуск перетаскивания предмета
    private void DragItem(bool isDrag)
    {
        isSelect = isDrag;
    }

    // Выбор предмета
    private void SelectItem(InventorySlot slot)
    {
        // Запоминаем слот
        selectedSlot = slot;

        // Запоминаем информацию о предмете в слоте
        selectedSlotItem.Item1 = slot.Item.Clone<PickableItem>();
        selectedSlotItem.Item2 = slot.Amount;

        // Заставляем выбравнный пердмет следовать за мышкой
        selectedItemImage.enabled = true;
        selectedItemImage.sprite = selectedSlotItem.Item1.ItemSprite;

        // Обновляем selectedItem в PlayerController
        SetSelectedItem?.Invoke(selectedSlotItem.Item1);

        // Убираем предмет из слота
        slot.Item = null;
        slot.Amount = -1;
    }

    // Складирование предмета
    private void PutItemInSlot(InventorySlot slot)
    {
        // Передаём предмет и его количество в слот
        slot.Item = selectedSlotItem.Item1.Clone<PickableItem>();
        slot.Amount = selectedSlotItem.Item2;

        // Отключаем движение выбранного предмета
        selectedItemImage.enabled = false;
        isSelect = false;

        // Обновляем selectedItem в PlayerController
        SetSelectedItem?.Invoke(null);
    }

    // Смена предметов местами
    private void SwapItems(InventorySlot slot)
    {
        if (selectedSlot.Item != slot.Item) // Если предметы разные
        {
            // Меняем предметы местами
            selectedSlot.Item = slot.Item.Clone<PickableItem>();
            selectedSlot.Amount = slot.Amount;

            slot.Item = selectedSlotItem.Item1.Clone<PickableItem>();
            slot.Amount = selectedSlotItem.Item2;
        }
        else // Если одинаковые
        {
            // При переполнении стака
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

        // Отключаем движение выбранного предмета
        selectedItemImage.enabled = false;
        isSelect = false;

        // Обновляем selectedItem в PlayerController
        SetSelectedItem?.Invoke(null);
    }

    // Добавление предмета в инвентарь
    private void AddItem(PickableItem item, int amount, Action<int> returnRemains)
    {
        // Сначала ищем похожий предмет
        foreach (var slot in slots)
        {
            if (slot.Item == item && amount > 0)
            {
                // Смотрим чтобы сумарное количество не привышало маклимальное
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

        // Если похожий предмет не нашли или все стаки заполнены, то ищем пустой слот
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
            InventoryFullMessage?.Raise("Инвентарь заполнен");

        returnRemains?.Invoke(amount);
    }

    // Сортировка предметов
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

    // Если выбранный предмет использован
    private void ItemUsed()
    {
        // Обновляем selectedItem в PlayerController
        SetSelectedItem?.Invoke(null);

        // Отключаем движение выбранного предмета
        selectedItemImage.enabled = false;
        isSelect = false;
    }

    // Использование предметов на персонаже
    private void UseSelectedItem()
    {
        // Возвращаем выбранный предмет на место
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

    // Возвращение выбранного предмета в слот
    private void ReturnSelectItemInSlot()
    {
        // Возвращаем предмет на место
        selectedSlot.Item = selectedSlotItem.Item1;
        selectedSlot.Amount = selectedSlotItem.Item2;

        // Отключаем движение выбранного предмета
        selectedItemImage.enabled = false;
        isSelect = false;

        // Обновляем selectedItem в PlayerController
        SetSelectedItem?.Invoke(null);
    }

    // Удаление предмета при выносе его за пределы инвентаря
    private void DeleteItem(List<RaycastResult> uiElems)
    {
        foreach (RaycastResult elem in uiElems)
        {
            if (elem.gameObject.TryGetComponent<InventorySlot>(out var slot))
            {
                // Если предмет ключевой, то просто возвращаем его на место
                if (slot.Item.ItemType == EItemType.Key)
                {
                    ReturnSelectItemInSlot();
                    return;
                }

                slot.DeleteItemInSlot();
            }
        }
    }

    // Удаление предмета через нажатие кнопки
    private void DeleteItem()
    {
        // Проверяем, чтобы предмет был не ключевым
        if (selectedSlotItem.Item1.ItemType == EItemType.Key)
            return;      

        // Удаляем предмет
        selectedSlot.DeleteItemInSlot();

        // Отключаем движение выбранного предмета
        selectedItemImage.enabled = false;
        isSelect = false;

        // Обновляем selectedItem в PlayerController
        SetSelectedItem?.Invoke(null);
    }

    // Обработка открытия/закрытия инвентаря
    private void OpenInventory(bool open)
    {
        if (!open)
        {
            // Если есть выбранный предмет, надо его вернуть в слот
            if (isSelect)
            {
                ReturnSelectItemInSlot();
            }

            // Скрываем все сообщения инвентаря
            HideInventoryMessages?.Invoke();
        }

        inventory.SetActive(open);
    }

    // Сохраннение инвентаря
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

    // Загрузка инвентаря
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
