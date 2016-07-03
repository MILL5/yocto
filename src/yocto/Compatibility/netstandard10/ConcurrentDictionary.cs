#if NETSTANDARD1_0
using System;
using System.Collections.Generic;

namespace System.Collections.Concurrent
{
    internal class ConcurrentDictionary<TKey, TValue> : Dictionary<TKey, TValue>
    {
        private readonly object _syncLock = new object();

        public ConcurrentDictionary() : base()
        {
        }

        public ConcurrentDictionary(IDictionary<TKey, TValue> collection)
            : base(collection)
        {
        }

        public TValue AddOrUpdate(TKey key, Func<TKey, TValue> addValueFactory, Func<TKey, TValue, TValue> updateValueFactory)
        {
            TValue newValue;
            bool found;

            lock (_syncLock)
            {
                found = ContainsKey(key);

                if (found)
                {
                    TValue oldValue = this[key];
                    newValue = updateValueFactory(key, oldValue);

                    this[key] = newValue;
                }
                else
                {
                    newValue = addValueFactory(key);

                    Add(key, newValue);
                }
            }

            return newValue;
        }

        public bool TryRemove(TKey key, out TValue value)
        {
            bool found = false;
            value = default(TValue);
            
            lock (_syncLock)
            {
                try
                {
                    if (ContainsKey(key))
                    {
                        value = this[key];
                        found = true;
                    }
                }
                catch
                {
                }
            }

            return found;
        }
    }
}
#endif