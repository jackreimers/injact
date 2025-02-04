namespace Injact.Core.State.Spatial;

public struct Vector2 : IEquatable<Vector2>, IFormattable
{
    public float X { get; set; }
    public float Y { get; set; }

    public Vector2()
    {
        X = 0f;
        Y = 0f;
    }

    public Vector2(float value)
    {
        X = value;
        Y = value;
    }

    public Vector2(float x, float y)
    {
        X = x;
        Y = y;
    }

    public bool Equals(Vector2 other)
    {
        return X.Equals(other.X) && Y.Equals(other.Y);
    }

    public override bool Equals(object? obj)
    {
        return obj is Vector2 other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y);
    }

    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        if (string.IsNullOrEmpty(format))
        {
            format = "G";
        }

        return format switch
        {
            "G" => $"({X}, {Y})",
            _ => throw new FormatException($"The {format} format string is not supported.")
        };
    }

    public static Vector2 operator +(Vector2 first, Vector2 second)
    {
        return new Vector2(first.X + second.X, first.Y + second.Y);
    }

    public static Vector2 operator -(Vector2 first, Vector2 second)
    {
        return new Vector2(first.X - second.X, first.Y - second.Y);
    }

    public static Vector2 operator -(Vector2 vector)
    {
        return new Vector2(-vector.X, -vector.Y);
    }

    public static Vector2 operator *(Vector2 vector, float amount)
    {
        return new Vector2(vector.X * amount, vector.Y * amount);
    }

    public static Vector2 operator *(float amount, Vector2 vector)
    {
        return new Vector2(vector.X * amount, vector.Y * amount);
    }

    public static Vector2 operator /(Vector2 vector, float amount)
    {
        return new Vector2(vector.X / amount, vector.Y / amount);
    }

    public static Vector2 operator *(Vector2 first, Vector2 second)
    {
        return new Vector2(first.X * second.X, first.Y * second.Y);
    }

    public static bool operator ==(Vector2 first, Vector2 second)
    {
        var num1 = first.X - second.X;
        var num2 = first.Y - second.Y;

        return num1 * (double)num1 + num2 * (double)num2 < 9.999999439624929E-11;
    }

    public static bool operator !=(Vector2 first, Vector2 second)
    {
        return !(first == second);
    }

    public static readonly Vector2 Zero = new(0f, 0f);
    public static readonly Vector2 One = new(1f, 1f);
}