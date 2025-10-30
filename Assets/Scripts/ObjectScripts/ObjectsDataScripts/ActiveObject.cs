using System.Collections.Generic;
using UnityEngine;

public abstract class ActiveObject : ScriptableObject
{
    [Header("Main Params")]
    [SerializeField] private ShowPlayerMessageEvent messageEvent;

    [field: SerializeField] public string Name { get; protected set; }

    [field: SerializeField] public EActiveObjectType ObjectType { get; protected set; }

    [field: SerializeField] public string Description { get; protected set; }

    [field: SerializeField] public bool IsActive { get; protected set; } = true;

    public void ShowDescription()
    {
        if (IsActive) 
        {
            RaiseMessage(Description);
        }
    }

    // Вызов события на показ сообщения
    protected void RaiseMessage(string message)
    {
        messageEvent?.Raise(message);
    }
}
