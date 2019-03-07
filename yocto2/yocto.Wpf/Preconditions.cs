using System;
// ReSharper disable UseNullPropagation

namespace yocto
{
    internal static class Preconditions
    {
        private static void ThrowException(Exception ex)
        {
            throw ex;
        }

        private static void CheckParamName(string paramName)
        {
            if (string.IsNullOrWhiteSpace(paramName))
                ThrowException(new ArgumentException($@"{nameof(paramName)} cannot be null, empty or whitespace.", nameof(paramName)));
        }

        public static void CheckIsNotNull(string paramName, object value)
        {
            CheckParamName(paramName);

            if (value == null)
                ThrowException(new ArgumentNullException(paramName, $@"{paramName} cannot be null."));
        }

        public static void CheckIfLengthLessThanOrEqual(string paramName, object[] values, int thisValue)
        {
            CheckParamName(paramName);

            if (values == null)
                return;

            if (thisValue < 0)
                ThrowException(new ArgumentException(nameof(thisValue), $@"{nameof(thisValue)} must be greater than or equal to 0."));

            if (values.Length > thisValue)
                ThrowException(new ArgumentOutOfRangeException(paramName, $@"{paramName} length must be less than or equal to {thisValue}."));
        }

        public static void CheckIsGreaterThanOrEqual(string paramName, int value, int thisValue)
        {
            CheckParamName(paramName);

            if (value < thisValue)
                ThrowException(new ArgumentOutOfRangeException(paramName, $@"{paramName} must be greater than or equal to {thisValue}."));
        }

        public static void CheckIsNotNullEmptyOrWhitespace(string paramName, string value)
        {
            CheckParamName(paramName);

            if (string.IsNullOrWhiteSpace(value))
                ThrowException(new ArgumentNullException(paramName, $@"{paramName} cannot be null, empty or whitespace."));
        }

        public static void CheckIsNotNull<T>(string paramName, object value) where T : Exception, new()
        {
            CheckParamName(paramName);

            if (value == null)
            {
                ThrowException(new T());
            }
        }

        public static T CheckIsType<T>(string paramName, object value) where T : class
        {
            CheckParamName(paramName);

            CheckIsNotNull(paramName, value);

            if (!(value is T))
            {
                ThrowException(new ArgumentException(paramName, $@"{paramName} is not of type [{typeof(T).Name}]."));
            }

            return (T)value;
        }
    }
}