using System;
using static mill5.yocto.Preconditions;

namespace mill5.yocto
{
    public class Cleanup
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