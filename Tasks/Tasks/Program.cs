using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Tasks
{
    class Program
    {
        static void Main(string[] args)
        {

            Task.Factory.StartNew(() => Console.WriteLine("sth"));

            Task task = Task.Factory.StartNew(() =>
            {
                Console.WriteLine("Task started");
                Thread.Sleep(2000);
                Console.WriteLine("Foo");
            });
            Console.WriteLine(task.IsCompleted);  // False
            task.Wait();  // Blocks until task is complete

        /*   Task<int> primeNumberTask = Task.Factory.StartNew(() =>
            Enumerable.Range(2, 3000000).Count
            (n => Enumerable.Range(2, (int)Math.Sqrt(n) - 1).All(i => n % i > 0)));

            Console.WriteLine("Task running...");
            Console.WriteLine("The answer is " + primeNumberTask.Result);//return result after task 
            //finished execution, until this time it will block*/

            // Start a Task that throws a NullReferenceException:
            Task taskEx = Task.Factory.StartNew(() => { throw null; });
            try
            {
                taskEx.Wait();
            }
            catch (AggregateException aex)
            {
                if (aex.InnerException is NullReferenceException)
                    Console.WriteLine("Null!");
                else
                    throw;
            }

//
           // Console.WriteLine("here");
           Task<int> primeNumberTaskWithContinous = Task.Factory.StartNew(() =>
                Enumerable.Range(2, 3000000).Count(n => Enumerable.Range(2, (int)Math.Sqrt(n) - 1).All(i => n % i > 0)));
            
            primeNumberTaskWithContinous.ContinueWith(antecedent =>
            {
                int result = antecedent.Result;
                Console.WriteLine(result);          // Writes 123
            });

            Console.ReadLine();

            var tcs = new TaskCompletionSource<int>();

            new Thread(() => { Thread.Sleep(5000); tcs.SetResult(42); }).Start();

            Task<int> taskCS = tcs.Task;         // Our "slave" task.
            Console.WriteLine(taskCS.Result);   // 42

            

        }


        //own impl.
        //Task<TResult> Run<TResult>(Func<TResult> function)
      //  {
     //       var tcs = new TaskCompletionSource<TResult>();
     //       new Thread(() =>
     //       {
        //        try { tcs.SetResult(function()); }
     //           catch (Exception ex) { tcs.SetException(ex); }
     //       }).Start();
     //       return tcs.Task;
    //    }

        

    }
}
