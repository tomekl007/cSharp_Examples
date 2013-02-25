using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GenericDelegate
{
    public delegate T Transformer<T>(T arg);
    // With this definition, we can write a generalized Transform utility method that works on any type:
    public class Util
    {
        public static void Transform<T>(T[] values, Transformer<T> t)
        {
            for (int i = 0; i < values.Length; i++)
                values[i] = t(values[i]);
            
        }
    }
    

    class Program
    {
        static void Main()
        {
            int[] values = { 1, 2, 3 };
            Util.Transform(values, Square);      // Dynamically hook in Square
            Console.WriteLine(values[1]);
        }
        static int Square(int x) { return x * x; }
    }
   
}
