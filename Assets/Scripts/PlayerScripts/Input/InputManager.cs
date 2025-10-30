using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    // События на дейтвия при нажатии на кнопку
    public static event Action<Vector2> Move;
    public static event Action<Collider2D> InteractWithObject;
    public static event Action<List<RaycastResult>> InteractWithInventory;
    public static event Action<List<RaycastResult>> UseItemInSlot;
    public static event Action<List<RaycastResult>> DeleteItem;
    public static event Action<bool> OpenInventory;
    public static event Action<bool> DragItem;
    public static event Action SaveInventory;
    public static event Action LoadInventory;
    public static event Action SortInventory;

    [SerializeField] private Camera mainCamera;
    [SerializeField] private GraphicRaycaster raycaster;
    [SerializeField] private EventSystem eventSystem;

    // Схемы ввода инпутов
    [Header("Input Schemes")]
    [SerializeField] private InputScheme defaultScheme; // Обычный режим
    [SerializeField] private InputScheme openInventoryScheme; // Режим открытого инвентаря

    private InputScheme currentOverrideScheme = null; // Текущая схема

    // Возможные контекстные состояния 
    private enum EInputContextState
    {
        Default,
        OpenInventory
    }
    private EInputContextState currentState = EInputContextState.Default; // Текущий контекст

    // Инпуты для переключения состояний
    private InputBinding openInventoryInput;

    private void Awake()
    {
        openInventoryInput = defaultScheme.Actions.Find(a => a.Action == EPlayerActions.OpenInventory).Binding;
    }

    void Update()
    {
        ChangeCurrentState();
        HandleInput();
    }

    // Метод для смены текущего состояния
    private void ChangeCurrentState()
    {
        // Изменение на OpenInventory
        if (IsInputTriggered(openInventoryInput))
        {
            if (currentState == EInputContextState.Default)
            {
                // Открываем инвентарь
                OpenInventory?.Invoke(true);

                currentState = EInputContextState.OpenInventory;
                currentOverrideScheme = openInventoryScheme;
            }
            else if (currentState == EInputContextState.OpenInventory)
            {
                // Закрываем инвентарь
                OpenInventory?.Invoke(false);

                currentState = EInputContextState.Default;
                currentOverrideScheme = null;
            }
        }
    }

    // Метод обработки нажатия на инпут из схемы
    private void HandleInput()
    {
        // Проверяем инпут в текущей схеме
        if (currentOverrideScheme != null)
        {
            foreach (var action in currentOverrideScheme.Actions)
            {
                if (IsInputTriggered(action.Binding))
                {
                    ExecuteAction(action);
                }
            }
        }

        // Если нет, смотрим в базовой
        if (defaultScheme != null)
        {
            foreach (var action in defaultScheme.Actions)
            {
                // Если необходимо перетащить предмет
                if (action.Action == EPlayerActions.Interact)
                {                   
                    if (IsInputTriggered(action.Binding)) // Нажатие
                    {                     
                        ExecuteAction(action);
                        
                    }
                    else if (IsInputHeld(action.Binding)) // Удержание кнопки мыши
                    {
                        DragItem?.Invoke(true);
                    }
                    else if (IsInputReleased(action.Binding)) // Отжатие
                    {
                        ExecuteAction(action);
                        DragItem?.Invoke(false);
                    }
                }
                else 
                {
                    if ((IsInputTriggered(action.Binding)))
                        ExecuteAction(action);
                }
            }
        }

    }

    // Проверка удержания соответсвующего инпута
    private bool IsInputHeld(InputBinding binding)
    {
        switch (binding.Kind)
        {
            case EInputKind.Key:
                return Input.GetKey(binding.Key);

            case EInputKind.MouseButton:
                return Input.GetMouseButton(binding.MouseButton);

            case EInputKind.Axis:
                float axis = Input.GetAxisRaw(binding.AxisName);

                if (binding.Direction == EDirection.Right)
                    return axis > binding.AxisThreshold;
                else
                    return axis < -binding.AxisThreshold;

            default:
                return false;
        }
    }

    // Проверка нажатия на соответсвующий инпут
    private bool IsInputTriggered(InputBinding binding)
    {
        switch (binding.Kind)
        {
            case EInputKind.Key:
                return Input.GetKeyDown(binding.Key);
            case EInputKind.MouseButton:
                return Input.GetMouseButtonDown(binding.MouseButton);
            case EInputKind.Axis:
                float axis = Input.GetAxisRaw(binding.AxisName);

                if (binding.Direction == EDirection.Right)
                    return axis > binding.AxisThreshold;
                else
                    return axis < -binding.AxisThreshold;
            default:
                return false;
        }
    }

    // Проверка отжатия соотвествующего инпута
    private bool IsInputReleased(InputBinding binding)
    {
        switch (binding.Kind)
        {
            case EInputKind.Key:
                return Input.GetKeyUp(binding.Key);

            case EInputKind.MouseButton:
                return Input.GetMouseButtonUp(binding.MouseButton);

            default:
                return false;
        }
    }

    // Метод вызова событий на исполнение инпутов
    private void ExecuteAction(InputAction action)
    {
        switch (action.Action)
        {
            case EPlayerActions.MoveLeft: // Передвижение персонажа         
            case EPlayerActions.MoveRight:
                Vector2 moveVector = new()
                {
                    x = Input.GetAxisRaw(action.Binding.AxisName)
                };

                Move?.Invoke(moveVector);
                break;

            case EPlayerActions.Interact: // Взаимодействие
                // Взаимодействие с объектом
                Collider2D hit = Physics2D.OverlapPoint((Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition));
                if (hit != null)
                {
                    InteractWithObject?.Invoke(hit);
                    break;
                }

                // Взаимодействие с инвентарём
                PointerEventData interactPointerData = new(eventSystem)
                {
                    position = Input.mousePosition
                };

                List<RaycastResult> interactResults = new();
                raycaster.Raycast(interactPointerData, interactResults);

                InteractWithInventory?.Invoke(interactResults);

                break;

            case EPlayerActions.Use:
                // Взаимодействие с инвентарём
                PointerEventData usePointerData = new(eventSystem)
                {
                    position = Input.mousePosition
                };

                List<RaycastResult> useResults = new();
                raycaster.Raycast(usePointerData, useResults);

                UseItemInSlot?.Invoke(useResults);
                break;

            case EPlayerActions.Delete:
                // Взаимодействие с инвентарём
                PointerEventData deletePointerData = new(eventSystem)
                {
                    position = Input.mousePosition
                };

                List<RaycastResult> deleteResults = new();
                raycaster.Raycast(deletePointerData, deleteResults);

                DeleteItem?.Invoke(deleteResults);
                break;

            case EPlayerActions.SaveInventory:
                SaveInventory?.Invoke();
                break;

            case EPlayerActions.LoadInventory:
                LoadInventory?.Invoke();
                break;

            case EPlayerActions.Sort:
                SortInventory?.Invoke(); 
                break;
        }
    }
}
