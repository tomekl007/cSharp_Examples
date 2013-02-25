using System;
using System.Collections.Generic;

using System.Text;

namespace Language_binding
{
    class Program
    {
        // Language binding occurs when a dynamic object does not implement IDynamicMetaObjectProvider:

        static dynamic Mean(dynamic x, dynamic y)
        {
            return (x + y) / 2;
        }

        static void Main()
        {
            int x = 3, y = 4;
            Console.WriteLine(Mean(x, y));
        }
    }
}
