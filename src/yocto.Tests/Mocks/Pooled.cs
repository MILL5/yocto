using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace yocto.tests.Mocks
{
    interface IPooled
    {
        long Count { get; }
    }

    public class Pooled : IPooled
    {
        private static long _count;

        public Pooled()
        {
            Interlocked.Increment(ref _count);
        }

        public long Count => Interlocked.Read(ref _count);

        public static void Reset()
        {
            _count = 0;
        }
    }
}
