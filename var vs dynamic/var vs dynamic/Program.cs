using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace var_vs_dynamic
{
    class Program
    {
        static void Main(string[] args)
        {
            // var says, “let the compiler figure out the type”.
            // dynamic says, “let the runtime figure out the type”.

            dynamic x = "hello";  // Static type is dynamic, runtime type is string
            var y = "hello";      // Static type is string, runtime type is string
            int i = x;            // Run-time error
            int j = y;            // Compile-time error
        }
    }
}
