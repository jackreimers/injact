namespace Injact.Core.State.Vectors;

public struct Bounds : IEquatable<Bounds>
{
    public Vector3 Center { get; private set; }
    public Vector3 Extents { get; set; }

    public Vector3 Size
    {
        get => Extents * 2;
        set => Extents = value * 0.5f;
    }

    public Vector3 Minimum
    {
        get => Center - Extents;
        set => SetExtents(value, Maximum);
    }

    public Vector3 Maximum
    {
        get => Center + Extents;
        set => SetExtents(Minimum, value);
    }

    public Bounds(Vector3 center, Vector3 size)
    {
        Center = center;
        Extents = size * 0.5f;
    }

    public bool Equals(Bounds other)
    {
        return Center.Equals(other.Center) && Extents.Equals(other.Extents);
    }

    public override bool Equals(object? obj)
    {
        return obj is Bounds other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Center, Extents);
    }

    public static bool operator ==(Bounds first, Bounds second)
    {
        return first.Center == second.Center && first.Extents == second.Extents;
    }

    public static bool operator !=(Bounds first, Bounds second)
    {
        return !(first == second);
    }

    public bool Intersects(Vector3 target)
    {
        throw new NotImplementedException();
    }

    public bool Intersects(Bounds target)
    {
        return
            Minimum.X <= target.Maximum.X &&
            Maximum.X >= target.Minimum.X &&
            Minimum.Y <= target.Maximum.Y &&
            Maximum.Y >= target.Minimum.Y &&
            Minimum.Z <= target.Maximum.Z &&
            Maximum.Z >= target.Minimum.Z;
    }

    public void Expand(float value)
    {
        Extents += new Vector3(value, value, value) * 0.5f;
    }

    public void Expand(Vector3 value)
    {
        Extents += value * 0.5f;
    }

    public void Encapsulate(Vector3 target)
    {
        SetExtents(Vector3.Minimum(Minimum, target), Vector3.Maximum(Maximum, target));
    }

    public void Encapsulate(Bounds target)
    {
        Encapsulate(target.Center - target.Extents);
        Encapsulate(target.Center + target.Extents);
    }

    private void SetExtents(Vector3 minimum, Vector3 maximum)
    {
        Center = minimum + Extents;
        Extents = (maximum - minimum) * 0.5f;
    }
}
