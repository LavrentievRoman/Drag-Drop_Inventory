using System;
using UnityEngine.Events;

public abstract class GameEvent<T> : GameEvent
{
    protected UnityEvent<T> unityEvent = new();

    // Подписка на событее
    public void RegisterListener(UnityAction<T> listener)
    {
        unityEvent.AddListener(listener);
    }

    // Отписка от события
    public void UnRegisterListener(UnityAction<T> listener)
    {
        unityEvent.RemoveListener(listener);
    }

    // Инициализация события
    public void Raise(T param)
    {
        unityEvent?.Invoke(param);
    }

    // Обработка исключения
    public override void Raise()
    {
        throw new InvalidOperationException($"{name} requires parameters. Use Raise(T param) instead.");
    }
}
