using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Text;
using System.Diagnostics;

namespace Threads1
{
    class Program
    {

        static bool _done;
        static readonly object _locker = new object();

        public static void StartF()
        {
            new Thread(Go).Start();
            Go();
        }

        static void Go()
        {
            lock (_locker)
            {
                if (!_done) { Console.WriteLine("Done"); _done = true; }
            }
        }





        static void Main(string[] args)
        {

            Threads1.Program.StartF();
            Thread t = new Thread((() => Console.WriteLine("in Thread")));
            t.IsBackground = true;//thead t dies because is background
            t.Start();

            Process p = Process.GetCurrentProcess();
            Console.WriteLine(p.PriorityClass);
          //  p.PriorityClass = ProcessPriorityClass.Normal;


            var signal = new ManualResetEvent(false);

            new Thread(() =>
            {
                Console.WriteLine("Waiting for signal...");
                signal.WaitOne();
                signal.Dispose();
                Console.WriteLine("Got signal!");
            }).Start();

            Thread.Sleep(2000);
            signal.Set();        // “Open” the signal


        }

       
    }
}
