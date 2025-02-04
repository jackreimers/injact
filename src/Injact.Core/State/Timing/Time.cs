namespace Injact.Core.State.Timing;

public static class Time
{
    public static float Delta { get; set; }

    public static event Action? OnUpdateEvent;
    public static event Action? OnLateUpdateEvent;

    public static void TriggerUpdate()
    {
        OnUpdateEvent?.Invoke();
    }

    public static void TriggerLateUpdate()
    {
        OnLateUpdateEvent?.Invoke();
    }
}