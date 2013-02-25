using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iterators
{
    class Program
    {
        static void Main()
        {
            foreach (int fib in Fibs(6,true))
                Console.Write(fib + "  ");
        }

        static IEnumerable<int> Fibs(int fibCount, bool breakEarly)
        {
           
                for (int i = 0, prevFib = 1, curFib = 1; i < fibCount; i++)
                {//Whereas a return statement expresses “Here’s the value you asked me to return fromthis method,”
                    //a yield return statement expresses “Here’s the next element you asked me to yield from
                    //this enumerator.” On each yield statement, control is returned to the caller, but the 
                    //callee’s state is maintained so that the method cancontinue executing as soon as the caller
                    //enumerates the next element. The lifetime of this state is bound to the enumerator, such that
                    //the state can be released when the caller has finished enumerating
                    yield return prevFib;
                    int newFib = prevFib + curFib;
                    prevFib = curFib;
                    curFib = newFib;

                    //A return statement is illegal in an iterator block—you must usea yield break instead.
                    if (breakEarly)
                        yield break;
                } 
            }
        }
    }

