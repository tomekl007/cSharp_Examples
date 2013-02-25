using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace POP3
{
    class Program
    {
        static void Main(string[] args)
        {
        }

        static void Receive()
        {
            using (TcpClient client = new TcpClient("mail.isp.com ", 110))
            using (NetworkStream n = client.GetStream())
            {
                ReadLine(n);                             // Read the welcome message.
                SendCommand(n, "USER username");
                SendCommand(n, "PASS password");
                SendCommand(n, "LIST");                  // Retrieve message IDs
                List<int> messageIDs = new List<int>();
                while (true)
                {
                    string line = ReadLine(n);             // e.g.  "1 1876"
                    if (line == ".") break;
                    messageIDs.Add(int.Parse(line.Split(' ')[0]));   // Message ID
                }

                foreach (int id in messageIDs)         // Retrieve each message.
                {
                    SendCommand(n, "RETR " + id);
                    string randomFile = Guid.NewGuid().ToString() + ".eml";
                    using (StreamWriter writer = File.CreateText(randomFile))
                        while (true)
                        {
                            string line = ReadLine(n);      // Read next line of message.
                            if (line == ".") break;          // Single dot = end of message.
                            if (line == "..") line = ".";    // "Escape out" double dot.
                            writer.WriteLine(line);         // Write to output file.
                        }
                    SendCommand(n, "DELE " + id);       // Delete message off server.
                }
                SendCommand(n, "QUIT");
            }
        }

        static string ReadLine(Stream s)
        {
            List<byte> lineBuffer = new List<byte>();
            while (true)
            {
                int b = s.ReadByte();
                if (b == 10 || b < 0) break;
                if (b != 13) lineBuffer.Add((byte)b);
            }
            return Encoding.UTF8.GetString(lineBuffer.ToArray());
        }

        static void SendCommand(Stream stream, string line)
        {
            byte[] data = Encoding.UTF8.GetBytes(line + "\r\n");
            stream.Write(data, 0, data.Length);
            string response = ReadLine(stream);
            if (!response.StartsWith("+OK"))
                throw new Exception("POP Error: " + response);
        }
    }
}
