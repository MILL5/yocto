using System;
using System.Threading;

namespace Console.Net451
{
    public class TenSecondJob : IJob
    {
        private readonly CountdownEvent _countdown = new CountdownEvent(10);
        private readonly ILogger _logger;

        public TenSecondJob(ILogger logger)
        {
            _logger = logger;
        }

        public void Run()
        {
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = 1000;
            timer.Elapsed += Timer_Elapsed;
            timer.Start();

            _countdown.Wait();

            timer.Stop();
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            _logger.WriteLine(DateTime.Now.ToLongTimeString());
            _countdown.AddCount();
        }
    }
}
