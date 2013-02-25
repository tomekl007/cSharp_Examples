using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.MemoryMappedFiles;

namespace Memory_mapped_file_client_side
{
    class Program
    {
        static void Main(string[] args)
        {
            using (MemoryMappedFile mmFile = MemoryMappedFile.OpenExisting("Demo"))
            using (MemoryMappedViewAccessor accessor = mmFile.CreateViewAccessor())
            {
                Console.WriteLine(accessor.ReadInt32(0));   // 12345

                byte[] data = new byte[accessor.ReadInt32(0)];
                accessor.ReadArray(4, data, 0, data.Length);
                Console.WriteLine(Encoding.UTF8.GetString(data));   // This is a test
            }
        }
    }
}
