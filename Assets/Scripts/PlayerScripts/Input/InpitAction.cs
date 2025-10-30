using UnityEngine;

// Класс для привязки кнопки клавиатура к действию игрока
[CreateAssetMenu(fileName = "New Input", menuName = "Input/Action Input")]
public class InputAction : ScriptableObject
{
    // Действие игрока
    [field: SerializeField] public EPlayerActions Action { get; private set; }

    // Кнопка на клавиатуре
    [field: SerializeField] public InputBinding Binding { get; private set; }
}
