#if NET4
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
            target = (T)_reference.Target;

            return target != null;
        }
    }
}
#endif