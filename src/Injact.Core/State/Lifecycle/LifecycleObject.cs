namespace Injact.Core.State.Lifecycle;

public abstract class LifecycleObject : ILifecycleObject
{
    //ReSharper disable once ConvertToConstant.Local
    //This will be set to true by the dependency injection container
    private readonly bool _shouldRunUpdate = false;
    private bool isEnabled;

    public bool IsEnabled
    {
        get => isEnabled;
        set => OnEnabledChanged(value);
    }

    public event Action<LifecycleObject>? OnEnableEvent;
    public event Action<LifecycleObject>? OnDisableEvent;
    public event Action<LifecycleObject>? OnDestroyEvent;

    public virtual void Awake() { }

    public virtual void Start() { }

    public virtual void Update() { }

    public virtual void Destroy()
    {
        Time.OnUpdateEvent -= Update;
        OnDestroyEvent?.Invoke(this);
    }

    public void Enable()
    {
        IsEnabled = true;
    }

    public void Disable()
    {
        IsEnabled = false;
    }

    protected virtual void OnEnabledChanged(bool value)
    {
        if (isEnabled == value)
        {
            return;
        }

        isEnabled = value;

        if (isEnabled)
        {
            OnEnableEvent?.Invoke(this);

            if (_shouldRunUpdate)
            {
                Time.OnUpdateEvent += Update;
            }
        }

        else
        {
            OnDisableEvent?.Invoke(this);
            Time.OnUpdateEvent -= Update;
        }
    }
}