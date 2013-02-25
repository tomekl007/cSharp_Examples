using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Implicit_and_explitit_conversion
{
    class Program
    {
        // Implicit and explicit conversions are overloadable operators:

        public struct Note
        {
            int value;
            public int SemitonesFromA { get { return value; } }

            public Note(int semitonesFromA) { value = semitonesFromA; }

            // Convert to hertz
            public static implicit operator double(Note x)
            {
                return 440 * Math.Pow(2, (double)x.value / 12);
            }

            // Convert from hertz (accurate to the nearest semitone)
            public static explicit operator Note(double x)
            {
                return new Note((int)(0.5 + 12 * (Math.Log(x / 440) / Math.Log(2))));
            }
        }

        static void Main()
        {
            Note n = (Note)554.37;  // explicit conversion
            double x = n;           // implicit conversion
            Console.WriteLine(x);
            //Custom conversions are ignored by the as and is operators:
        }
    }
}
