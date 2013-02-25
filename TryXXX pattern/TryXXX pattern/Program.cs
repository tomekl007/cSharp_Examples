using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TryXXX_pattern
{
    class Program
    {
        static void Main()
        {
            bool result;
            TryToBoolean("Bad", out result);
            Console.WriteLine(result);

            result = ToBoolean("Bad");		// throws Exception
        }

        public static bool ToBoolean(string text)
        {
            bool returnValue;
            if (!TryToBoolean(text, out returnValue))
                throw new FormatException("Cannot parse to Boolean");
            return returnValue;
        }

        public static bool TryToBoolean(string text, out bool result)
        {
            text = text.Trim().ToUpperInvariant();
            if (text == "TRUE" || text == "YES" || text == "Y")
            {
                result = true;
                return true;
            }
            if (text == "FALSE" || text == "NO" || text == "N")
            {
                result = false;
                return true;
            }
            result = false;
            return false;
        }
    }
}
