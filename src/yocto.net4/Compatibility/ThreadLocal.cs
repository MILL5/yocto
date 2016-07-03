using System;
using System.Collections.Generic;

namespace yocto
{
    internal class ThreadLocal<T> where T : class
    {
        private readonly object _syncLock = new object();
        private readonly System.Threading.ThreadLocal<T> _threadLocal;
        private readonly IDictionary<int, T> _tracking;

        public ThreadLocal(Func<T> factory)
        {
            _threadLocal = new System.Threading.ThreadLocal<T>(factory);
            _tracking = new Dictionary<int, T>();
        }

        public T Value
        {
            get
            {
                T value = _threadLocal.Value;
                int hash = value.GetHashCode();

                lock (_syncLock)
                {
                    if (!_tracking.ContainsKey(hash))
                        _tracking.Add(hash, value);
                }

                return value;
            }
        }

        public IList<T> Values
        {
            get
            {
                List<T> values = new List<T>();

                lock (_syncLock)
                {
                    values.AddRange(_tracking.Values);
                }

                return values;
            }
        }
    }
}
