using UnityEngine;

[System.Serializable]
// Привязка кнопок
public class InputBinding
{
    // Тип ввода
    [field: SerializeField] public EInputKind Kind { get; private set; }

    // Ввод кнопкой на клавиатуре
    [field: Header("Key Input")]
    [field: SerializeField] public KeyCode Key { get; private set; }

    // Ввод нажатием на мышку
    [field: Header("Mouse Button Input")]
    [field: SerializeField] public int MouseButton { get; private set; }

    // Ввод осью
    [field: Header("Axis Input")]
    [field: SerializeField] public string AxisName { get; private set; } // Название
    [field: SerializeField] public float AxisThreshold { get; private set; } = 0.1f; // Порог активации
    [field: SerializeField] public EDirection Direction { get; private set; } // Направление
}
