namespace Injact.Core.State.Lifecycle.Interfaces;

public interface ILifecycleObject
{
    public bool IsEnabled { get; set; }

    public event Action<LifecycleObject>? OnEnableEvent;
    public event Action<LifecycleObject>? OnDisableEvent;
    public event Action<LifecycleObject>? OnDestroyEvent;

    public void Awake();

    public void Start();

    public void Update();

    public void Destroy();

    public void Enable();

    public void Disable();
}