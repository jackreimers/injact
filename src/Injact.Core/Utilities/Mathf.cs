namespace Injact.Core.Utilities;

public struct Mathf
{
    public const float Pi = 3.1415927f;
    public const float FloatComparisonError = 9.999999439624929E-11f;

    public static float SquareRoot(float value)
    {
        return (float)Math.Sqrt(value);
    }

    public static float Round(float value)
    {
        return (float)Math.Round(value);
    }

    public static float Round(float value, int digits)
    {
        return (float)Math.Round(value, digits);
    }

    public static int RoundToInt(float value)
    {
        return (int)Math.Round(value);
    }

    public static float Floor(float value)
    {
        return (float)Math.Floor(value);
    }

    public static int FloorToInt(float value)
    {
        return (int)Math.Floor(value);
    }

    public static float Ceiling(float value)
    {
        return (float)Math.Ceiling(value);
    }

    public static int CeilingToInt(float value)
    {
        return (int)Math.Ceiling(value);
    }

    public static float Minimum(float a, float b)
    {
        return a < b
            ? a
            : b;
    }

    public static float Maximum(float a, float b)
    {
        return a > b
            ? a
            : b;
    }

    public static int Absolute(float value)
    {
        return (int)Math.Abs(value);
    }

    public static float Clamp(float value, float minimum, float maximum)
    {
        if (value < minimum)
        {
            value = minimum;
        }

        else if (value > maximum)
        {
            value = maximum;
        }

        return value;
    }

    public static float Clamp01(float value)
    {
        return value switch
        {
            < 0f => 0f,
            > 1f => 1f,
            _ => value
        };
    }

    public static float Sin(float value)
    {
        return (float)Math.Sin(value);
    }

    public static float Cos(float value)
    {
        return (float)Math.Cos(value);
    }

    public static float Tan(float value)
    {
        return (float)Math.Tan(value);
    }

    public static float Asin(float value)
    {
        return (float)Math.Asin(value);
    }

    public static float Acos(float value)
    {
        return (float)Math.Acos(value);
    }

    public static float Atan(float value)
    {
        return (float)Math.Atan(value);
    }

    public static float Atan2(float y, float x)
    {
        return (float)Math.Atan2(y, x);
    }

    public static float Power(float value, float power)
    {
        return (float)Math.Pow(value, power);
    }
}