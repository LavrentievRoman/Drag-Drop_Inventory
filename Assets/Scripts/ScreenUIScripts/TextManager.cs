using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class TextManager : MonoBehaviour
{
    // Событее на движение лейбла с текстом персонажа
    public static event Action<bool> MovePlayerTextLabel;
    // Событее на движение лейбла с именем объета
    public static event Action<bool> MoveObjectNameTextLabel;
    // Событее на движение подсказки
    public static event Action<bool> MoveTooltip;

    [Header("Events")]
    [SerializeField] private ShowPlayerMessageEvent playerMessageEvent;

    [Header("Text Labels")]
    [SerializeField] private TMP_Text playerTextLabel;
    [SerializeField] private TMP_Text objectNameTextLabel;

    [Header("Item Tooltip")]
    [SerializeField] private GameObject tooltip;
    [SerializeField] private TMP_Text itemNameLabel;
    [SerializeField] private TMP_Text itemDescriptionLabel;

    private Coroutine showPlayerMessage;

    private void Awake()
    {
        playerMessageEvent.RegisterListener(ShowPlayerMessage);

        PlayerController.ShowObjectName += ShowObjectName;

        InventorySlot.ShowTooltip += ShowTooltip;
        InventorySlot.HideTooltip += HideTooltip;

        InventoryManager.HideInventoryMessages += HideTooltip;
    }

    // Вывод сообщений персонажа
    private void ShowPlayerMessage(string message)
    {
        // Останавливаем показ предыдущего сообщения
        if (showPlayerMessage != null)
        {
            StopCoroutine(showPlayerMessage);
            MovePlayerTextLabel?.Invoke(false);
        }

        MovePlayerTextLabel?.Invoke(true);

        showPlayerMessage = StartCoroutine(ShowingPlayerMessage(message));
    }
    private IEnumerator ShowingPlayerMessage(string message) // Показ сообщения
    {
        playerTextLabel.text = message;

        yield return new WaitForSeconds(3f);

        playerTextLabel.text = "";
        MovePlayerTextLabel?.Invoke(false);
    }

    private void ShowTooltip(string name, string description)
    {
        tooltip.SetActive(true);

        itemNameLabel.text = name;
        itemDescriptionLabel.text = description;

        MoveTooltip?.Invoke(true);
    }

    private void HideTooltip()
    {
        MoveTooltip?.Invoke(false);

        itemNameLabel.text = "";
        itemDescriptionLabel.text = "";

        tooltip.SetActive(false);
    }

    // Показ имени обекта
    private void ShowObjectName(string _name)
    {
        if (_name != null)
            MoveObjectNameTextLabel?.Invoke(true);
        else MoveObjectNameTextLabel?.Invoke(false);

        objectNameTextLabel.text = _name;
    }
}
