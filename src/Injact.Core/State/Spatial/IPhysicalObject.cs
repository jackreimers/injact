namespace Injact.Core.State.Spatial;

public interface IPhysicalObject
{
    public IPhysicalProvider PhysicalProvider { get; set; }
}