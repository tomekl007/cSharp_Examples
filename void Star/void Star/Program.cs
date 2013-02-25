using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace void_Star
{
    class Program
    {
        // A void pointer (void*) makes no assumptions about the type of the underlying data and is
        // useful for functions that deal with raw memory:

        unsafe static void Main()
        {
           

            short[] a = { 1, 1, 2, 3, 5, 8, 13, 21, 34, 55 };
            fixed (short* p = a)
            {
                //sizeof returns size of value-type in bytes
                Zap(p, a.Length * sizeof(short));
            }
            foreach (short x in a)
                System.Console.WriteLine(x);   // Prints all zeros
        }
        /// <summary>
        /// get void* and zeros memory to byteCount
        /// </summary>
        /// <param name="memory"></param>
        /// <param name="byteCount"></param>
        unsafe static void Zap(void* memory, int byteCount)
        {
            Console.WriteLine("byteCount = " + byteCount);
            byte* b = (byte*)memory;
            for (int i = 0; i < byteCount; i++)
            {
                *b++ = 0;
            }
        }
    }
}
