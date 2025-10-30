using System.Collections.Generic;
using UnityEngine;

// Класс определяющий набор действий, которые можно сделать в текущем режиме
[CreateAssetMenu(fileName = "New Input Scheme", menuName = "Input/Scheme")]
public class InputScheme : ScriptableObject
{
    [field: SerializeField] public List<InputAction> Actions { get; private set; }
}
