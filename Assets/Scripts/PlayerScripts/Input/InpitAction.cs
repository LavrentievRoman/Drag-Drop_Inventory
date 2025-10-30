using UnityEngine;

// ����� ��� �������� ������ ���������� � �������� ������
[CreateAssetMenu(fileName = "New Input", menuName = "Input/Action Input")]
public class InputAction : ScriptableObject
{
    // �������� ������
    [field: SerializeField] public EPlayerActions Action { get; private set; }

    // ������ �� ����������
    [field: SerializeField] public InputBinding Binding { get; private set; }
}
