using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Process_and_redireciton
{
    class Program
    {
        static void Main(string[] args)
        {
            System.IO.TextWriter oldOut = Console.Out;

            using (System.IO.TextWriter w = System.IO.File.CreateText("e:\\output.txt")) {
            Console.SetOut(w);
            Console.WriteLine("hello word");
            }
            Console.SetOut(oldOut);
            System.Diagnostics.Process.Start("notepad.exe","e:\\output.txt");
        }
    }
}
