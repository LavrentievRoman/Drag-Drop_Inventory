using UnityEngine.Events;

public abstract class VoidEvent : GameEvent
{
    protected UnityEvent unityEvent = new();

    // Подписка на событее
    public void RegisterListener(UnityAction listener)
    {
        unityEvent.AddListener(listener);
    }

    // Отписка от события
    public void UnRegisterListener(UnityAction listener)
    {
        unityEvent.RemoveListener(listener);
    }

    // Инициализация события
    public override void Raise()
    {
        unityEvent?.Invoke();
    }
}
