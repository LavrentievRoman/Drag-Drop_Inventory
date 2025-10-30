using System;
using UnityEngine.Events;

public abstract class GameEvent<T> : GameEvent
{
    protected UnityEvent<T> unityEvent = new();

    // �������� �� �������
    public void RegisterListener(UnityAction<T> listener)
    {
        unityEvent.AddListener(listener);
    }

    // ������� �� �������
    public void UnRegisterListener(UnityAction<T> listener)
    {
        unityEvent.RemoveListener(listener);
    }

    // ������������� �������
    public void Raise(T param)
    {
        unityEvent?.Invoke(param);
    }

    // ��������� ����������
    public override void Raise()
    {
        throw new InvalidOperationException($"{name} requires parameters. Use Raise(T param) instead.");
    }
}
