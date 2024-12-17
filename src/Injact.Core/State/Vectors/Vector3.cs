namespace Injact.Core.State.Vectors;

public struct Vector3 : IEquatable<Vector3>, IFormattable
{
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }

    public float Magnitude => GetMagnitude(this);
    public float SquaredMagnitude => GetSquaredMagnitude(this);

    public Vector3 Normalised => GetNormalised(this);

    public Vector3()
    {
        X = 0f;
        Y = 0f;
        Z = 0f;
    }

    public Vector3(float value)
    {
        X = value;
        Y = value;
        Z = value;
    }

    public Vector3(float x, float y, float z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public bool Equals(Vector3 other)
    {
        return X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z);
    }

    public override bool Equals(object? obj)
    {
        return obj is Vector3 other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y, Z);
    }

    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        if (string.IsNullOrEmpty(format))
        {
            format = "G";
        }

        return format switch
        {
            "G" => $"({X}, {Y}, {Z})",
            _ => throw new FormatException($"The {format} format string is not supported.")
        };
    }

    public static Vector3 operator +(Vector3 first, Vector3 second)
    {
        return new Vector3(first.X + second.X, first.Y + second.Y, first.Z + second.Z);
    }

    public static Vector3 operator -(Vector3 first, Vector3 second)
    {
        return new Vector3(first.X - second.X, first.Y - second.Y, first.Z - second.Z);
    }

    public static Vector3 operator -(Vector3 vector)
    {
        return new Vector3(-vector.X, -vector.Y, -vector.Z);
    }

    public static Vector3 operator *(Vector3 vector, float amount)
    {
        return new Vector3(vector.X * amount, vector.Y * amount, vector.Z * amount);
    }

    public static Vector3 operator *(float amount, Vector3 vector)
    {
        return new Vector3(vector.X * amount, vector.Y * amount, vector.Z * amount);
    }

    public static Vector3 operator *(Vector3 first, Vector3 second)
    {
        return new Vector3(first.X * second.X, first.Y * second.Y, first.Z * second.Z);
    }

    public static Vector3 operator /(Vector3 vector, float amount)
    {
        return new Vector3(vector.X / amount, vector.Y / amount, vector.Z / amount);
    }

    public static bool operator ==(Vector3 first, Vector3 second)
    {
        var number1 = first.X - second.X;
        var number2 = first.Y - second.Y;
        var number3 = first.Z - second.Z;

        return number1 * (double)number1 + number2 * (double)number2 + number3 * (double)number3 < Mathf.FloatComparisonError;
    }

    public static bool operator !=(Vector3 first, Vector3 second)
    {
        return !(first == second);
    }

    private static float GetMagnitude(Vector3 vector)
    {
        return Mathf.SquareRoot(vector.X * vector.X + vector.Y * vector.Y + vector.Z * vector.Z);
    }

    private static float GetSquaredMagnitude(Vector3 vector)
    {
        return vector.X * vector.X + vector.Y * vector.Y + vector.Z * vector.Z;
    }

    private static Vector3 GetNormalised(Vector3 vector)
    {
        var magnitude = GetMagnitude(vector);
        return vector / magnitude;
    }

    public static float Distance(Vector3 first, Vector3 second)
    {
        return GetMagnitude(first - second);
    }

    public static float DistanceSquared(Vector3 first, Vector3 second)
    {
        return GetSquaredMagnitude(first - second);
    }

    public static float Dot(Vector3 first, Vector3 second)
    {
        return (float)(first.X * (double)second.X + first.Y * (double)second.Y + first.Z * (double)second.Z);
    }

    public static Vector3 Cross(Vector3 first, Vector3 second)
    {
        return new Vector3(
            (float)(first.Y * (double)second.Z - first.Z * (double)second.Y),
            (float)(first.Z * (double)second.X - first.X * (double)second.Z),
            (float)(first.X * (double)second.Y - first.Y * (double)second.X));
    }

    public static Vector3 Minimum(Vector3 first, Vector3 second)
    {
        return new Vector3(
            Mathf.Minimum(first.X, second.X),
            Mathf.Minimum(first.Y, second.Y),
            Mathf.Minimum(first.Z, second.Z));
    }

    public static Vector3 Maximum(Vector3 first, Vector3 second)
    {
        return new Vector3(
            Mathf.Maximum(first.X, second.X),
            Mathf.Maximum(first.Y, second.Y),
            Mathf.Maximum(first.Z, second.Z));
    }

    public static Vector3 Translate(Vector3 point, Vector3 reference, float distance)
    {
        return point + (point - reference).Normalised * distance;
    }

    public static Vector3 Rotate(Vector3 point, Vector3 axis, float angle)
    {
        angle = angle.ToRadians();
        return point * Mathf.Cos(angle) + Cross(axis, point) * Mathf.Sin(angle) + axis * Dot(axis, point) * (1 - Mathf.Cos(angle));
    }

    public static Vector3 Pivot(Vector3 point, Vector3 pivot, Vector3 axis, float angle)
    {
        return pivot + Rotate(point - pivot, axis, angle);
    }

    public static Vector3 Round(Vector3 vector)
    {
        return new Vector3(
            Mathf.Round(vector.X),
            Mathf.Round(vector.Y),
            Mathf.Round(vector.Z));
    }

    public static Vector3 Round(Vector3 vector, float step)
    {
        var factor = step / 1;

        return new Vector3(
            Mathf.Round(vector.X / factor) * factor,
            Mathf.Round(vector.Y / factor) * factor,
            Mathf.Round(vector.Z / factor) * factor);
    }

    public static float Angle(Vector3 first, Vector3 second)
    {
        var cross = Cross(first, second);
        var dot = Dot(first, second);

        return Mathf.Atan2(cross.Magnitude, dot);
    }

    public static float SignedAngle(Vector3 first, Vector3 second, Vector3 axis)
    {
        var cross = Cross(first, second);
        var dot = Dot(first, second);

        var angle = Mathf.Atan2(cross.Magnitude, dot);

        var check = Dot(axis, cross);
        if (check < 0f)
        {
            angle = -angle;
        }

        return angle;
    }

    public static readonly Vector3 Zero = new(0f, 0f, 0f);
    public static readonly Vector3 One = new(1f, 1f, 1f);
    public static readonly Vector3 Forward = new(0f, 0f, 1f);
    public static readonly Vector3 Backward = new(0f, 0f, -1f);
    public static readonly Vector3 Up = new(0f, 1f, 0f);
    public static readonly Vector3 Down = new(0f, -1f, 0f);
    public static readonly Vector3 Left = new(-1f, 0f, 0f);
    public static readonly Vector3 Right = new(1f, 0f, 0f);
}