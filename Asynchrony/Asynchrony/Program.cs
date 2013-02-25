using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Asynchrony
{
    class Program
    {
        static void Main(string[] args)
        {
            //coarse grained
          //  Task.Factory.StartNew(() => DisplayPrimeCounts());
            //Console.ReadLine();

            //fine grained
            Program p = new Program();
         //   p.DisplayPrimeCountsF();
         //   Console.ReadLine();


          
            p.DisplayPrimeCountsAsync();
            Console.ReadLine();
        }

        Task DisplayPrimeCountsAsync()
        {
            var machine = new PrimesStateMachine();
            machine.DisplayPrimeCountsFrom(0);
            return machine.Task;
        }


       static void DisplayPrimeCounts()
        {
            for (int i = 0; i < 10; i++)
                Console.WriteLine(GetPrimesCount(i * 1000000 + 2, 1000000) +
                    " primes between " + (i * 1000000) + " and " + ((i + 1) * 1000000 - 1));

            Console.WriteLine("Done!");
        }

       static int GetPrimesCount(int start, int count)
        {
            return
                ParallelEnumerable.Range(start, count).Count(n =>
                    Enumerable.Range(2, (int)Math.Sqrt(n) - 1).All(i => n % i > 0));
        }
        
        //finer
       void DisplayPrimeCountsF()
       {
           DisplayPrimeCountsFrom(0);
       }

     


        Task<int> GetPrimesCountAsync (int start, int count)
{
    Console.WriteLine("getPrimesAsync" + Thread.CurrentThread.ManagedThreadId);
	return Task.Factory.StartNew (() => 
		ParallelEnumerable.Range (start, count).Count (n => 
			Enumerable.Range (2, (int) Math.Sqrt(n)-1).All (i => n % i > 0)));
}

          void DisplayPrimeCountsFrom(int i)		// This is starting to get awkward!
       {
         //  var awaiter = GetPrimesCountAsync(i * 1000000 + 2, 1000000).GetAwaiter();
         //  awaiter.OnCompleted

            GetPrimesCountAsync(i * 1000000 + 2, 1000000).ContinueWith
              (awaiter =>
           {
               Console.WriteLine(awaiter.Result + " primes between " +
                   (i * 1000000) + " and " + ((i + 1) * 1000000 - 1));
               if (i++ < 10) DisplayPrimeCountsFrom(i);
               else Console.WriteLine("Done");
           });
       }
       
    }

    class PrimesStateMachine		// Even more awkward!!
    {
        TaskCompletionSource<object> _tcs = new TaskCompletionSource<object>();
        public Task Task { get { return _tcs.Task; } }

        public void DisplayPrimeCountsFrom(int i)
        {
            
            GetPrimesCountAsync(i * 1000000 + 2, 1000000).ContinueWith((awaiter) =>
            {
                Console.WriteLine(awaiter.Result);
                if (i++ < 10) DisplayPrimeCountsFrom(i);
                else { Console.WriteLine("Done"); _tcs.SetResult(awaiter.Result); }
            });
        }

        Task<int> GetPrimesCountAsync(int start, int count)
        {
            return Task.Factory.StartNew(() =>
                ParallelEnumerable.Range(start, count).Count(n =>
                    Enumerable.Range(2, (int)Math.Sqrt(n) - 1).All(i => n % i > 0)));
        }
    }
}
  
