using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class TextManager : MonoBehaviour
{
    // ������� �� �������� ������ � ������� ���������
    public static event Action<bool> MovePlayerTextLabel;
    // ������� �� �������� ������ � ������ ������
    public static event Action<bool> MoveObjectNameTextLabel;
    // ������� �� �������� ���������
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

    // ����� ��������� ���������
    private void ShowPlayerMessage(string message)
    {
        // ������������� ����� ����������� ���������
        if (showPlayerMessage != null)
        {
            StopCoroutine(showPlayerMessage);
            MovePlayerTextLabel?.Invoke(false);
        }

        MovePlayerTextLabel?.Invoke(true);

        showPlayerMessage = StartCoroutine(ShowingPlayerMessage(message));
    }
    private IEnumerator ShowingPlayerMessage(string message) // ����� ���������
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

    // ����� ����� ������
    private void ShowObjectName(string _name)
    {
        if (_name != null)
            MoveObjectNameTextLabel?.Invoke(true);
        else MoveObjectNameTextLabel?.Invoke(false);

        objectNameTextLabel.text = _name;
    }
}
