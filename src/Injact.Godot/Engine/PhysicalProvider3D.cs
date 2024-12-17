using NativeVector3 = Injact.Core.State.Vectors.Vector3;
using EngineVector3 = Godot.Vector3;

namespace Injact.Godot;

public class PhysicalProvider3D : IPhysicalProvider
{
    private readonly Node _parent;
    private readonly Node3D _node;

    public bool Enabled { get; private set; } = true;

    public PhysicalProvider3D(Node3D node)
    {
        _parent = node.GetParent();
        _node = node;
    }

    public NativeVector3 Position
    {
        get => _node.Position.ToNative();
        set => _node.Position = value.ToEngine();
    }

    public void Translate(NativeVector3 translation)
    {
        _node.Translate(translation.ToEngine());
    }

    public void Translate(float x, float y, float z)
    {
        _node.Translate(new EngineVector3(x, y, z));
    }

    public void TranslateLocal(NativeVector3 translation)
    {
        _node.TranslateObjectLocal(translation.ToEngine());
    }

    public void TranslateLocal(float x, float y, float z)
    {
        _node.TranslateObjectLocal(new EngineVector3(x, y, z));
    }

    public void Rotate(NativeVector3 rotation)
    {
        _node.RotateX(rotation.X);
        _node.RotateY(rotation.Y);
        _node.RotateZ(rotation.Z);
    }

    public void Rotate(float x, float y, float z)
    {
        _node.RotateX(x);
        _node.RotateY(y);
        _node.RotateZ(z);
    }

    public NativeVector3 LookAt(NativeVector3 target)
    {
        _node.LookAt(target.ToEngine());
        return _node.Rotation.ToNative();
    }

    public void Enable()
    {
        SetEnabled(true);
    }

    public void Disable()
    {
        SetEnabled(false);
    }

    public void SetEnabled(bool value)
    {
        if (Enabled == value)
        {
            return;
        }

        Enabled = value;

        if (value)
        {
            _parent.AddChild(_node);
        }

        else
        {
            _parent.RemoveChild(_node);
        }
    }

    public void Destroy()
    {
        _node.QueueFree();
    }
}