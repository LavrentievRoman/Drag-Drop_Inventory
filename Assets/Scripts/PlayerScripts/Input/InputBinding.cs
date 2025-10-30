using UnityEngine;

[System.Serializable]
// �������� ������
public class InputBinding
{
    // ��� �����
    [field: SerializeField] public EInputKind Kind { get; private set; }

    // ���� ������� �� ����������
    [field: Header("Key Input")]
    [field: SerializeField] public KeyCode Key { get; private set; }

    // ���� �������� �� �����
    [field: Header("Mouse Button Input")]
    [field: SerializeField] public int MouseButton { get; private set; }

    // ���� ����
    [field: Header("Axis Input")]
    [field: SerializeField] public string AxisName { get; private set; } // ��������
    [field: SerializeField] public float AxisThreshold { get; private set; } = 0.1f; // ����� ���������
    [field: SerializeField] public EDirection Direction { get; private set; } // �����������
}
