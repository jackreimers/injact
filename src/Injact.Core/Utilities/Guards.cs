namespace Injact.Core.Utilities;

public static class Guard
{
    public static class Against
    {
        private const string ObjectNullMessage = "Object \"{0}\" cannot be null.";
        private const string ObjectNotNullMessage = "Object \"{0}\" must be null.";
        private const string StringNullOrEmptyMessage = "String cannot be null or empty.";
        private const string StringNullOrWhitespaceMessage = "String cannot be null or whitespace.";
        private const string ValueNegativeMessage = "Value must be greater than or equal to zero.";
        private const string ValueZeroOrNegativeMessage = "Value must be greater than zero.";

        public static void Condition(bool condition, string message)
        {
            if (condition)
            {
                throw new DependencyException(message);
            }
        }

        public static void Assignable<T1, T2>(string message)
        {
            if (typeof(T1).IsAssignableFrom(typeof(T2)))
            {
                throw new DependencyException(message);
            }
        }

        public static T Null<T>(T? value)
        {
            if (value == null || value == null!)
            {
                throw new NullReferenceException(string.Format(ObjectNullMessage, typeof(T).Name));
            }

            return value;
        }

        public static T? NotNull<T>(T? value)
        {
            if (value != null || value != null!)
            {
                throw new ArgumentException(string.Format(ObjectNotNullMessage, typeof(T).Name));
            }

            return value;
        }

        public static string NullOrEmpty(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new NullReferenceException(StringNullOrEmptyMessage);
            }

            return value;
        }

        public static string NullOrWhitespace(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new NullReferenceException(StringNullOrWhitespaceMessage);
            }

            return value;
        }

        public static int Negative(int value)
        {
            if (value < 0)
            {
                throw new ArithmeticException(ValueNegativeMessage);
            }

            return value;
        }

        public static int ZeroOrNegative(int value)
        {
            if (value <= 0)
            {
                throw new ArithmeticException(ValueZeroOrNegativeMessage);
            }

            return value;
        }
    }
}