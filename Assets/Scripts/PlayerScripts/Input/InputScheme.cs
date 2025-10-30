using System.Collections.Generic;
using UnityEngine;

// ����� ������������ ����� ��������, ������� ����� ������� � ������� ������
[CreateAssetMenu(fileName = "New Input Scheme", menuName = "Input/Scheme")]
public class InputScheme : ScriptableObject
{
    [field: SerializeField] public List<InputAction> Actions { get; private set; }
}
