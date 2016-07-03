using System;

namespace yocto
{
    internal class WeakReference<T> where T : class
    {
        private readonly WeakReference _reference;

        public WeakReference(T target)
        {
            _reference = new WeakReference(target);
        }

        public bool TryGetTarget(out T target)
        {
            target = null;

            try
            {
                target = (T)_reference.Target;
            }
            catch (InvalidOperationException)
            {
            }

            return target != null;
        }
    }
}
