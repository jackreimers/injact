namespace Injact.Core.State.Spatial.Interfaces;

/// <summary>
/// Wrapper class that provides access to the physical properties of a scene object.
/// </summary>
public interface IPhysicalProvider
{
    public Vector3 Position { get; set; }

    public void Translate(Vector3 translation);

    public void Translate(float x, float y, float z);

    public void TranslateLocal(Vector3 translation);

    public void TranslateLocal(float x, float y, float z);

    public void Rotate(Vector3 rotation);

    public void Rotate(float x, float y, float z);

    public Vector3 LookAt(Vector3 target);

    public void Enable();

    public void Disable();

    public void SetEnabled(bool value);

    public void Destroy();
}