using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Pipes;

namespace UnnamedPipeChild
{
    class Program
    {
        static void Main(string[] args)
        {
            string rxID = args[0];    // Note we're reversing the
            string txID = args[1];    // receive and transmit roles.
            
            using (var rx = new AnonymousPipeClientStream(PipeDirection.In, rxID)) 
            using (var tx = new AnonymousPipeClientStream(PipeDirection.Out, txID)) 
            { 
                Console.WriteLine("Client received: " + rx.ReadByte());
                tx.WriteByte(200); 
            }
        }
    }
}
