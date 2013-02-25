using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Dynamic;


namespace custom_binding
{
    class Program
    {
        // Custom binding occurs when a dynamic object implements IDynamicMetaObjectProvider:

        static void Main()
        {
            dynamic d = new Duck();
            d.Quack();                  // Quack method was called
            d.Waddle();                 // Waddle method was called
        }

        public class Duck : System.Dynamic.DynamicObject
        {
            public override bool TryInvokeMember(
                InvokeMemberBinder binder, object[] args, out object result)
            {
                Console.WriteLine(binder.Name + " method was called");
                result = null;
                return true;
            }
        }
    }
}
