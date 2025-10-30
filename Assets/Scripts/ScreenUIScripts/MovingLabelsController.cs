using System.Collections;
using UnityEngine;

public class MovingLabelsController : MonoBehaviour
{
    [SerializeField] private Canvas canvas;

    [SerializeField] private Transform playerPosition;

    [Header("Moving Labels")]
    [SerializeField] private RectTransform playerTextLabel;
    [SerializeField] private RectTransform objectNameLabel;
    [SerializeField] private RectTransform tooltip;

    private bool isPlayerTextLabelMove = false;
    private bool isObjectNameLabelMove = false;
    private bool isTooltipMove = false;

    private void Awake()
    {
        TextManager.MoveObjectNameTextLabel += StartMoveObjectNameLabel;
        TextManager.MovePlayerTextLabel += StartMovePlayerTextLabel;
        TextManager.MoveTooltip += StartMoveTooltip;
    }

    void LateUpdate()
    {
        if (isPlayerTextLabelMove)
        {
            StartCoroutine(MovePlayerTextLabel());
        }

        if (isObjectNameLabelMove)
        {
            MoveObjectNameLabel();
        }

        if (isTooltipMove)
        {
            MoveTooltip();
        }
    }

    // ��������/����������� �������� ������ �� ����������
    private void StartMovePlayerTextLabel(bool isMove)
    {
        isPlayerTextLabelMove = isMove;
    }

    // ��������/����������� �������� ������ ����� ������� �� ��������
    private void StartMoveObjectNameLabel(bool isMove)
    {
        isObjectNameLabelMove = isMove;
    }

    private void StartMoveTooltip(bool isMove)
    {
        isTooltipMove = isMove;
    }

    // �������� ������ ����� ������� �� ��������
    private void MoveObjectNameLabel()
    {
        // ����������� ������� ������� � ���������� �������
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            Input.mousePosition,
            canvas.worldCamera,
            out Vector2 labelPos
            );

        // ����������� ������� � �������� ������ ���� ������
        Vector2 labelSize = objectNameLabel.rect.size;
        labelPos.x += labelSize.x * objectNameLabel.pivot.x;
        labelPos.y += labelSize.y * (1 - objectNameLabel.pivot.y);

        // ������ ������� ������
        objectNameLabel.anchoredPosition = labelPos;
    }

    private void MoveTooltip()
    {
        // ����������� ������� ������� � ���������� �������
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            Input.mousePosition,
            canvas.worldCamera,
            out Vector2 labelPos
            );

        // ����������� ������� � �������� ������ ���� ������
        Vector2 labelSize = tooltip.rect.size;
        labelPos.x += labelSize.x * tooltip.pivot.x;
        labelPos.y += labelSize.y * (1 - tooltip.pivot.y);

        float offsetX = 15f;
        float offsetY = -20f;
        labelPos.x += offsetX;
        labelPos.y += offsetY;

        // ������ ������� ������
        tooltip.anchoredPosition = labelPos;
    }

    // �������� ������ �� ����������
    private IEnumerator MovePlayerTextLabel()
    {
        yield return new WaitForEndOfFrame();

        // ���������� ������� ��� ������� ���������
        Vector3 offset = new(0, 1.5f, 0);
        Vector3 pos = Camera.main.WorldToScreenPoint(playerPosition.position + offset);

        // ����������� ������� � ���������� �������
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            pos,
            canvas.worldCamera,
            out Vector2 labelPos
            );

        // ������ ������� ������ 
        playerTextLabel.anchoredPosition = labelPos;
    }
}
