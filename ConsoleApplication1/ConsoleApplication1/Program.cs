using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplication1
{
    public delegate void ProgressReporter(int percentComplete);

    class Program
    {
        static void Main(string[] args)
        {
            X x = new X();
           
            ProgressReporter p = x.InstanceProgress;
            p(99);                                 // 99
            Console.WriteLine(p.Target == x);     // True
            Console.WriteLine(p.Method);          // Void InstanceProgress(Int32)
        }
    }
    
    class X
    {
        public void InstanceProgress(int percentComplete)
        {
            Console.WriteLine(percentComplete);
        }
    }
}
