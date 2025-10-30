using UnityEngine.Events;

public abstract class VoidEvent : GameEvent
{
    protected UnityEvent unityEvent = new();

    // �������� �� �������
    public void RegisterListener(UnityAction listener)
    {
        unityEvent.AddListener(listener);
    }

    // ������� �� �������
    public void UnRegisterListener(UnityAction listener)
    {
        unityEvent.RemoveListener(listener);
    }

    // ������������� �������
    public override void Raise()
    {
        unityEvent?.Invoke();
    }
}
