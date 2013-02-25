using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Pipes;
using System.IO;
using System.Diagnostics;

namespace PipeClientSide
{
    class Program
    {
        static void Main(string[] args)
        {

        /*    using (var s = new NamedPipeClientStream ("pipedream"))
            {
                s.Connect(); 
                Console.WriteLine (s.ReadByte()); 
                s.WriteByte (200);                 // Send the value 200 back.
                s.WriteByte(10);
            }*/
            using (var s = new NamedPipeClientStream("pipedream2")) 
            { 
                s.Connect(); 
                s.ReadMode = PipeTransmissionMode.Message; 
                Console.WriteLine(Encoding.UTF8.GetString(ReadMessage(s)));
                byte[] msg = Encoding.UTF8.GetBytes("Hello right back!"); 
                s.Write(msg, 0, msg.Length); 
            }

          


        }
        static byte[] ReadMessage(PipeStream s)
        {
            MemoryStream ms = new MemoryStream();
            byte[] buffer = new byte[0x1000];      // Read in 4 KB blocks  
            do
            {
                ms.Write(buffer, 0, s.Read(buffer, 0, buffer.Length));
            } while (!s.IsMessageComplete);
            return ms.ToArray();
        }
    }
}
