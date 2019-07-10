using Microsoft.SqlServer.Dac;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace TestBackup
{
    class Program
    {
        private static Timer timer;

        static void Main(string[] args)
        {            

            var timerState = new TimerState { Counter = 0 };

            timer = new Timer(
                callback: new TimerCallback(TimerTask),
                state: timerState,
                dueTime: 1000,
                period: 60000);

            while (timerState.Counter <= 10)
            {
                Task.Delay(60000).Wait();
               
            }

            timer.Dispose();
            Console.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: done.");          
        }

        private static void TimerTask(object timerState)
        {
            DateTime now = DateTime.Now;

            var minute = now.Minute;

            if(minute == 30 || minute == 0 )
            {
                string data = now.ToString("dd-MM-yyyy-HH-mm-ss");
                Console.WriteLine(data);

                var connectionString = "Server=DESKTOP-08EQ49B; " +
                    "Database=bober; User Id=test; Password=123456; " +
                    "MultipleActiveResultSets=true";
                var services = new DacServices(connectionString);
                services.Message += (sender, e) =>
                {
                    // If you use a lock file,
                    // this would be a good location to extend the lease
                };


                services.ExportBacpac("D://" + data + ".bacpac", "bober");
                var state = timerState as TimerState;
                Interlocked.Increment(ref state.Counter);
            }
           
        }

        class TimerState
        {
            public int Counter;
        }
    }
}
