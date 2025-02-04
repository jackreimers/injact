using NativeVector2 = Injact.Core.State.Spatial.Vector2;
using NativeVector3 = Injact.Core.State.Spatial.Vector3;
using EngineVector2 = Godot.Vector2;
using EngineVector3 = Godot.Vector3;

namespace Injact.Godot.Utilities.Extensions;

public static class VectorExtensions
{
    public static NativeVector2 ToNative(this EngineVector2 vector)
    {
        return new NativeVector2(vector.X, vector.Y);
    }

    public static EngineVector2 ToEngine(this NativeVector2 vector)
    {
        return new EngineVector2(vector.X, vector.Y);
    }

    public static NativeVector3 ToNative(this EngineVector3 vector)
    {
        return new NativeVector3(vector.X, vector.Y, vector.Z);
    }

    public static EngineVector3 ToEngine(this NativeVector3 vector)
    {
        return new EngineVector3(vector.X, vector.Y, vector.Z);
    }
}