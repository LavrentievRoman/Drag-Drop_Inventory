using UnityEngine;

public abstract class GameEvent : ScriptableObject
{
    public abstract void Raise();
}
