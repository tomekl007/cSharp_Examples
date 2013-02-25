using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;


namespace ConcurentPatterns
{
    class Program
    {
        static void Main(string[] args)
        {

            //	var token = new CancellationToken();
            var tokenC = new CancellationTokenSource();


            Program p = new Program();
            //   Task t = Task.Factory.StartNew(p.Foo(token));

            //      .ContinueWith (ant => token.Cancel());   // Tell it to cancel in two seconds.
            //     p.Foo(tokenC.Token);
             tokenC.Cancel();
            try
            {
                p.Foo(tokenC.Token);
            }
            catch (OperationCanceledException ex)
            { Console.WriteLine("Cancelled"); }

            Action<int> progress = i => Console.WriteLine(i + " %");
            p.FooProgress(progress);
            Console.ReadLine();
            //or use Progress class
 
     
        }


        // This is a simplified version of the CancellationToken type in System.Threading:
        /*    class CancellationToken
            {
                public bool IsCancellationRequested { get; private set; }
                public void Cancel() { IsCancellationRequested = true; }
                public void ThrowIfCancellationRequested()
                {
                    if (IsCancellationRequested) throw new OperationCanceledException();
                }
            }*/

        void Foo(CancellationToken cancellationToken)
        {
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine(i);
                Thread.Sleep(2);
                cancellationToken.ThrowIfCancellationRequested();//or i could ignore
            }
        }




        Task FooProgress(Action<int> onProgressPercentChanged)
        {
            return Task.Factory.StartNew(() =>
            {
                for (int i = 0; i < 1000; i++)
                {
                    if (i % 10 == 0) onProgressPercentChanged(i / 10);
                    // Do something compute-bound...
                }
            });
        }

    }
   
}
