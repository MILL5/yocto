using System;
using System.Diagnostics;

namespace sample.wpf.library
{
    internal class SimpleLogger : ILogger
    {
        public void Log(string message)
        {
            Debug.WriteLine(message);
        }

        public SimpleLogger()
        {
        }
    }
}
