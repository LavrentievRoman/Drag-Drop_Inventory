using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    // ������� �� ����������� �������� � ����
    public static event Action ReturnSelectedItem;
    // ������� ������������� �������� �� ���������
    public static event Action UseItemOnPlayer;
    // ������� �������� �������� �� ���������
    public static event Action DropItem;

    // ������� �� ����� ����� �������������� �������
    public static event Action<string> ShowObjectName;

    [Header("Collision Parameters")]
    [SerializeField] private float collisionOffset = 0.05f; // ������ ��� ������������
    [SerializeField] private ContactFilter2D movementFilter; // ��������� �������

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

    // ������������ ��������� ����������� ����������
    private void PlayerMoveByKeyboard(Vector2 moveVector)
    {
        StopMoveToObject();

        // ���������� �� �������� �� ���� ���������� 
        List<RaycastHit2D> castCollisions = new();

        // ����������� ���������� ���������� �� ���� ������ �� �������� ���������
        int count = rb.Cast(moveVector, movementFilter, castCollisions, playerSpeed * Time.fixedDeltaTime + collisionOffset);

        if (count == 0)
        {
            // ������� ���������
            rb.MovePosition(rb.position + playerSpeed * Time.fixedDeltaTime * moveVector);
        }
    }

    // �������� � �������
    private void PlayerMoveToObject(Vector2 objectCoord, Action<PickableItem> onArrived)
    {
        // ��������� �������� 
        moveToObjectCoroutine = StartCoroutine(MovePlayer(objectCoord, onArrived));
    }
    private IEnumerator MovePlayer(Vector2 target, Action<PickableItem> onArrived)
    {
        // ��������� ���� �� ���������� � ������������ �������
        while (Mathf.Abs(transform.position.x - target.x) > 0.01f)
        {
            // ���������� ����������� ��������
            float direction = Mathf.Sign(target.x - rb.position.x);

            // ���������� ������� ���
            float actualStep = Mathf.Min(playerSpeed * Time.fixedDeltaTime, Mathf.Abs(target.x - rb.position.x));

            // ������� ���������
            Vector2 newPosition = rb.position + new Vector2(actualStep * direction, 0);
            rb.MovePosition(newPosition);

            yield return new WaitForFixedUpdate();
        }

        // ������������ ������� ��������� 
        rb.MovePosition(new Vector2(target.x, rb.position.y));

        // ��������� ����� �������������� � ��������
        onArrived?.Invoke(selectedItem);

        // ��������� �������� 
        moveToObjectCoroutine = null;
    }

    // ��������� �������� � �������
    private void StopMoveToObject()
    {
        if (moveToObjectCoroutine != null)
            StopCoroutine(moveToObjectCoroutine);
    }

    // �������������� � ��������
    private void InteractWithObject(Collider2D hit)
    {
        if (hit != null)
        {
            // ���� ������ - �����
            if (hit.CompareTag("Player"))
            {
                if (selectedItem != null)
                    UseItemOnPlayer?.Invoke();
            }

            // �������� ����� �������������� c ������������� ��������
            if (hit.TryGetComponent<InteractiveObjectController>(out var interactiveObject))
            {          
                StopMoveToObject();              

                interactiveObject.Interact(selectedItem);             
            }         
        }
    }

    // �������������� � ���������
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

    // ������������� ���������� �� ���������
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

    // ������ �� ������
    private void LookAtObject(string name, int amount)
    {
        // ���������� ��� ������� � ��� ����������, ���� �� ���� 
        if (amount > 1)
            ShowObjectName?.Invoke($"{name} (x{amount})");
        else ShowObjectName?.Invoke(name);
    }

    // ��������� ���������� ��������
    private void SetSelectedItem(PickableItem item)
    {
        if (item != null)
            selectedItem = item.Clone<PickableItem>();
        else selectedItem = null;
    }

}
