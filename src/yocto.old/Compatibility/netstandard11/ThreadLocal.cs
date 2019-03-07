using System;
using System.Collections.Generic;

#if NETSTANDARD1_1
namespace yocto
{
    internal class ThreadLocal<T> where T : class
    {
        private readonly System.Threading.ThreadLocal<T> _threadLocal;

        public ThreadLocal(Func<T> factory)
        {
            _threadLocal = new System.Threading.ThreadLocal<T>(factory, true);
        }

        public T Value => _threadLocal.Value;

        public IList<T> Values => _threadLocal.Values;
    }
}
#endif
