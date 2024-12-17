namespace Injact.Core.Utilities.Extensions;

public static class MathExtensions
{
    public static float ToRadians(this float value)
    {
        return value * 0.017453292f;
    }

    public static float ToDegrees(this float value)
    {
        return value * 57.29578f;
    }
}