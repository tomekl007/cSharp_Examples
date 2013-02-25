using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;

namespace CodeContracts
{
    class Program
    {
        static void Main(string[] args)
        {
            
            IList<String> list = new List<String>();
            String item = "first";
            //String item2 = null;
            AddIfNotPresent<String>(list, item);
            AddIfNotPresent<String>(list, item);

        }

        public static bool AddIfNotPresent<T> (IList<T> list, T item){
            Contract.Requires (list != null);          // Precondition  
            Contract.Requires (!list.IsReadOnly);      // Precondition 
            Contract.Ensures (list.Contains (item));   // Postcondition  
            if (list.Contains(item)) return false; 
            list.Add (item);  return true;
        }
    }
}
