namespace Injact.Core.State.Spatial;

//TODO: Operators for Vector2 and Vector2Int
public struct Vector2Int : IEquatable<Vector2Int>, IFormattable
{
    public int X { get; set; }
    public int Y { get; set; }

    public Vector2Int()
    {
        X = 0;
        Y = 0;
    }

    public Vector2Int(int value)
    {
        X = value;
        Y = value;
    }

    public Vector2Int(int x, int y)
    {
        X = x;
        Y = y;
    }

    public bool Equals(Vector2Int other)
    {
        return X.Equals(other.X) && Y.Equals(other.Y);
    }

    public override bool Equals(object? obj)
    {
        return obj is Vector2Int other && Equals(other);
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

    public static Vector2 operator +(Vector2Int first, Vector2Int second)
    {
        return new Vector2(first.X + second.X, first.Y + second.Y);
    }

    public static Vector2 operator -(Vector2Int first, Vector2Int second)
    {
        return new Vector2(first.X - second.X, first.Y - second.Y);
    }

    public static Vector2 operator -(Vector2Int vector)
    {
        return new Vector2(-vector.X, -vector.Y);
    }

    public static Vector2 operator *(Vector2Int vector, float amount)
    {
        return new Vector2(vector.X * amount, vector.Y * amount);
    }

    public static Vector2 operator *(float amount, Vector2Int vector)
    {
        return new Vector2(vector.X * amount, vector.Y * amount);
    }

    public static Vector2 operator /(Vector2Int vector, float amount)
    {
        return new Vector2(vector.X / amount, vector.Y / amount);
    }

    public static bool operator ==(Vector2Int first, Vector2Int second)
    {
        var num1 = first.X - second.X;
        var num2 = first.Y - second.Y;

        return num1 * (double)num1 + num2 * (double)num2 < 9.999999439624929E-11;
    }

    public static bool operator !=(Vector2Int first, Vector2Int second)
    {
        return !(first == second);
    }

    public static readonly Vector2Int Zero = new(0, 0);
    public static readonly Vector2Int One = new(1, 1);
}