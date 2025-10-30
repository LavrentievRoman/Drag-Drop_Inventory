using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    // Событее на возвращение предмета в слот
    public static event Action ReturnSelectedItem;
    // Событее использования предмета на персонаже
    public static event Action UseItemOnPlayer;
    // Событее удаления предмета из инвентаря
    public static event Action DropItem;

    // Событее на показ имени осматриваемого объекта
    public static event Action<string> ShowObjectName;

    [Header("Collision Parameters")]
    [SerializeField] private float collisionOffset = 0.05f; // Отступ при столкновении
    [SerializeField] private ContactFilter2D movementFilter; // Параметры колизий

    [Header("Player Speed")]
    [SerializeField] private float playerSpeed = 5f;

    private PickableItem selectedItem;

    private Rigidbody2D rb;

    private Coroutine moveToObjectCoroutine;

    private void Awake()
    {
        InputManager.Move += PlayerMoveByKeyboard;
        InputManager.InteractWithObject += InteractWithObject;
        InputManager.InteractWithInventory += InteractWithInventory;
        InputManager.UseItemInSlot += UseItemFromInventory;

        InteractiveObjectController.MoveTo += PlayerMoveToObject;
        InteractiveObjectController.LookAt += LookAtObject;

        InventoryManager.SetSelectedItem += SetSelectedItem;

        rb = GetComponent<Rigidbody2D>();
    }

    // Передвижение персонажа посредством клавиатуры
    private void PlayerMoveByKeyboard(Vector2 moveVector)
    {
        StopMoveToObject();

        // Информация об обхетках на пути перосонажа 
        List<RaycastHit2D> castCollisions = new();

        // Высчитываем количество препятсвий на пути игрока на заданном растоянии
        int count = rb.Cast(moveVector, movementFilter, castCollisions, playerSpeed * Time.fixedDeltaTime + collisionOffset);

        if (count == 0)
        {
            // Двигаем персонажа
            rb.MovePosition(rb.position + playerSpeed * Time.fixedDeltaTime * moveVector);
        }
    }

    // Движение к объекту
    private void PlayerMoveToObject(Vector2 objectCoord, Action<PickableItem> onArrived)
    {
        // Запускаем корутину 
        moveToObjectCoroutine = StartCoroutine(MovePlayer(objectCoord, onArrived));
    }
    private IEnumerator MovePlayer(Vector2 target, Action<PickableItem> onArrived)
    {
        // Двигаемся пока не сравняемся с координатами объекта
        while (Mathf.Abs(transform.position.x - target.x) > 0.01f)
        {
            // Определяем направление движения
            float direction = Mathf.Sign(target.x - rb.position.x);

            // Определяем текущий шаг
            float actualStep = Mathf.Min(playerSpeed * Time.fixedDeltaTime, Mathf.Abs(target.x - rb.position.x));

            // Двигаем персонажа
            Vector2 newPosition = rb.position + new Vector2(actualStep * direction, 0);
            rb.MovePosition(newPosition);

            yield return new WaitForFixedUpdate();
        }

        // Корректируем позицию персонажа 
        rb.MovePosition(new Vector2(target.x, rb.position.y));

        // Запускаем метод взаимодействия с объектом
        onArrived?.Invoke(selectedItem);

        // Завершаем корутину 
        moveToObjectCoroutine = null;
    }

    // Остановка движения к объекту
    private void StopMoveToObject()
    {
        if (moveToObjectCoroutine != null)
            StopCoroutine(moveToObjectCoroutine);
    }

    // Взаимодействие с объектом
    private void InteractWithObject(Collider2D hit)
    {
        if (hit != null)
        {
            // Если объект - игрок
            if (hit.CompareTag("Player"))
            {
                if (selectedItem != null)
                    UseItemOnPlayer?.Invoke();
            }

            // Вызываем метод взаимодействия c интерактивным объектом
            if (hit.TryGetComponent<InteractiveObjectController>(out var interactiveObject))
            {          
                StopMoveToObject();              

                interactiveObject.Interact(selectedItem);             
            }         
        }
    }

    // Взаимодействие с инвентарём
    private void InteractWithInventory(List<RaycastResult> uiElems)
    {
        foreach (RaycastResult elem in uiElems)
        {
            if (elem.gameObject.TryGetComponent<InventorySlot>(out var slot))
            {
                StopMoveToObject();

                slot.InteractWithSlot(selectedItem);

                return;
            }
        }

        if (selectedItem != null)
        {
            DropItem?.Invoke();
        }
    }

    // Использование предеметов из инвентаря
    private void UseItemFromInventory(List<RaycastResult> uiElems)
    {
        foreach (RaycastResult elem in uiElems)
        {
            if (elem.gameObject.TryGetComponent<InventorySlot>(out var slot))
            {
                if (selectedItem == null)
                    slot.UseItemInSlot();
            }
        }
    }

    // Взгляд на объект
    private void LookAtObject(string name, int amount)
    {
        // Показываем имя объекта и его количество, если не один 
        if (amount > 1)
            ShowObjectName?.Invoke($"{name} (x{amount})");
        else ShowObjectName?.Invoke(name);
    }

    // Установка выбранного предмета
    private void SetSelectedItem(PickableItem item)
    {
        if (item != null)
            selectedItem = item.Clone<PickableItem>();
        else selectedItem = null;
    }

}
