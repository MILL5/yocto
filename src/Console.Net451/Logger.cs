using System;

namespace Console.Net451
{
    public interface ILogger
    {
        void WriteLine(string line);
    }

    public class Logger : ILogger
    {
        public void WriteLine(string line)
        {
            System.Console.WriteLine(line);
        }
    }
}
