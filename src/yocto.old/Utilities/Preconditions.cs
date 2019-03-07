using System;
// ReSharper disable UseNullPropagation

namespace yocto
{
    public static class Preconditions
    {
        private static void CheckParamName(string paramName)
        {
            if (string.IsNullOrWhiteSpace(paramName))
                throw new ArgumentException($@"{nameof(paramName)} cannot be null, empty or whitespace.", nameof(paramName));
        }

        public static void CheckIsNotNull(string paramName, object value)
        {
            CheckParamName(paramName);

            if (value == null)
                throw new ArgumentNullException(paramName, $"{paramName} cannot be null.");
        }

        public static void CheckIfLengthLessThanOrEqual(string paramName, object[] values, int thisValue)
        {
            CheckParamName(paramName);

            if (values == null)
                return;

            if (thisValue < 0)
                throw new ArgumentException(nameof(thisValue), $"{nameof(thisValue)} must be greater than or equal to 0.");

            if (values.Length > thisValue)
                throw new ArgumentOutOfRangeException(paramName, $"{paramName} length must be less than or equal to {thisValue}.");
        }

        public static void CheckIsGreaterThanOrEqual(string paramName, int value, int thisValue)
        {
            CheckParamName(paramName);

            if (value < thisValue)
                throw new ArgumentOutOfRangeException(paramName, $"{paramName} must be greater than or equal to {thisValue}.");
        }

        public static void CheckIsNotNullEmptyOrWhitespace(string paramName, string value)
        {
            CheckParamName(paramName);

            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException(paramName, $"{paramName} cannot be null, empty or whitespace.");
        }
    }
}