using System;
using static yocto.Preconditions;

namespace yocto
{
    internal class Cleanup
    {
        public static void SafeMethod(Action action)
        {
            CheckIsNotNull(nameof(action), action);

            try
            {
                action();
            }
            catch
            {
                // DO NOTHING
            }
        }
    }
}