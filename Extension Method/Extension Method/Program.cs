using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Extension_Method
{
    public static class StringHelper
    {
        //An extension method is a static methodof a static class,
        //where the this modifier is applied to the first parameter.
        public static bool IsCapitalized(this string s)
        {
            if (string.IsNullOrEmpty(s)) return false;
            return char.IsUpper(s[0]);
        }

        public static T First<T>(this IEnumerable<T> sequence)
        {
            Console.WriteLine("IEnumerabnle");
            foreach (T element in sequence)
                return element;

            throw new InvalidOperationException("No elements!");
        }
    }

    class Program
    {
// Extension methods allow an existing type to be extended with new methods without altering
// the definition of the original type:

// (Note that these examples will not work in older versions of LINQPad)




    static void Main()
    {
    Console.WriteLine("Perth".IsCapitalized());
    // Equivalent to:
    Console.WriteLine(StringHelper.IsCapitalized("Perth"));

    // Interfaces can be extended, too:
    Console.WriteLine("Seattle".First());   // S
     }
    }
}
