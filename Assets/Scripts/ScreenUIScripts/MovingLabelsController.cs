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

    // Начинаем/заканчиваем движение текста за персонажем
    private void StartMovePlayerTextLabel(bool isMove)
    {
        isPlayerTextLabelMove = isMove;
    }

    // Начинаем/заканчиваем движение текста имени объетка за курсором
    private void StartMoveObjectNameLabel(bool isMove)
    {
        isObjectNameLabelMove = isMove;
    }

    private void StartMoveTooltip(bool isMove)
    {
        isTooltipMove = isMove;
    }

    // Движение текста имени объетка за курсором
    private void MoveObjectNameLabel()
    {
        // Преобразуем позицию курсора в координаты канваса
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            Input.mousePosition,
            canvas.worldCamera,
            out Vector2 labelPos
            );

        // Прикрепляем позицию к верхнему левому углу текста
        Vector2 labelSize = objectNameLabel.rect.size;
        labelPos.x += labelSize.x * objectNameLabel.pivot.x;
        labelPos.y += labelSize.y * (1 - objectNameLabel.pivot.y);

        // Меняем позицию текста
        objectNameLabel.anchoredPosition = labelPos;
    }

    private void MoveTooltip()
    {
        // Преобразуем позицию курсора в координаты канваса
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            Input.mousePosition,
            canvas.worldCamera,
            out Vector2 labelPos
            );

        // Прикрепляем позицию к верхнему левому углу текста
        Vector2 labelSize = tooltip.rect.size;
        labelPos.x += labelSize.x * tooltip.pivot.x;
        labelPos.y += labelSize.y * (1 - tooltip.pivot.y);

        float offsetX = 15f;
        float offsetY = -20f;
        labelPos.x += offsetX;
        labelPos.y += offsetY;

        // Меняем позицию текста
        tooltip.anchoredPosition = labelPos;
    }

    // Движение текста за персонажем
    private IEnumerator MovePlayerTextLabel()
    {
        yield return new WaitForEndOfFrame();

        // Определяем позицию над головой пероснажа
        Vector3 offset = new(0, 1.5f, 0);
        Vector3 pos = Camera.main.WorldToScreenPoint(playerPosition.position + offset);

        // Преобразуем позицию в координаты канваса
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            pos,
            canvas.worldCamera,
            out Vector2 labelPos
            );

        // Меняем позицию текста 
        playerTextLabel.anchoredPosition = labelPos;
    }
}
