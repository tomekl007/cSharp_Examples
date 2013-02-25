using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Pipes;
using System.Diagnostics;
using System.IO.Compression;
using System.IO.MemoryMappedFiles;
using System.IO.IsolatedStorage;


namespace StreamsAndIO
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create a file called test.txt in the current directory:
            using (Stream s = new FileStream("test.txt", FileMode.Create) )
            {
                Console.WriteLine(s.CanRead);       // True     
                Console.WriteLine(s.CanWrite);      // True    
                Console.WriteLine(s.CanSeek);       // True   
                s.WriteByte(101);
                s.WriteByte(102);
                byte[] block = { 1, 2, 3, 4, 5 };
                s.Write(block, 0, block.Length);     // Write block of 5 bytes    
                Console.WriteLine(s.Length);         // 7   
                Console.WriteLine(s.Position);       // 7    
                s.Position = 0;                       // Move back to the start   
                Console.WriteLine(s.ReadByte());     // 101    
                Console.WriteLine(s.ReadByte());     // 102     
                // Read from the stream back into the block array:     
                Console.WriteLine(s.Read(block, 0, block.Length));   // 5  

                // Assuming the last Read returned 5, we'll be at     
                // the end of the file, so Read will now return 0:   
                Console.WriteLine(s.Read(block, 0, block.Length));   // 0

                //wraper to synchronizer stream
                //Stream.Synchronized(s);


                //calculate how much lines is longer than 80
            
            }
            int longLines = File.ReadLines("test.txt")
                .Count(l => l.Length > 80);
            Console.WriteLine(longLines);

            //named pipe server side read one byte
       /*     using (var s = new NamedPipeServerStream("pipedream")) 
            { 
                s.WaitForConnection(); 
               s.WriteByte(100);
              
               Console.WriteLine(s.ReadByte()); 
            }*/

            //named pipe read message
          /*  using (var s = new NamedPipeServerStream("pipedream2", PipeDirection.InOut, 1,
                PipeTransmissionMode.Message))
                {
                s.WaitForConnection(); 
                byte[] msg = Encoding.UTF8.GetBytes("Hello"); 
                s.Write(msg, 0, msg.Length); 
                Console.WriteLine(Encoding.UTF8.GetString(ReadMessage(s)));
                }*/

            //unnamed pipe parent process(server)
            string clientExe = @"C:\Users\Tomek\Documents\Visual Studio 2010\Projects\PRACOWNIA PROGRAMOWNIE 2\UnnamedPipeChild\UnnamedPipeChild\bin\Debug\UnnamedPipeChild.exe";
            HandleInheritability inherit = HandleInheritability.Inheritable;
            using (var tx = new AnonymousPipeServerStream(PipeDirection.Out, inherit))
            using (var rx = new AnonymousPipeServerStream(PipeDirection.In, inherit))
            {
                string txID = tx.GetClientHandleAsString();
                string rxID = rx.GetClientHandleAsString();

                var startInfo = new ProcessStartInfo(clientExe, txID + " " + rxID);//passes like args
                startInfo.UseShellExecute = false;      // Required for child process  
                Process p = Process.Start(startInfo);

                tx.DisposeLocalCopyOfClientHandle();    // Release unmanaged 
                rx.DisposeLocalCopyOfClientHandle();    // handle resources.  

                tx.WriteByte(100);
                Console.WriteLine("Server received: " + rx.ReadByte());

                p.WaitForExit();
            }

            // Write 100K to a 
            File.WriteAllBytes ("myFile.bin", new byte [100000]);
            using (FileStream fs = File.OpenRead ("myFile.bin"))
            using (BufferedStream bs = new BufferedStream (fs, 20000))  //20K buffer
            {
                bs.ReadByte();  
                Console.WriteLine (fs.Position);         // 20000
            }

            using (FileStream fs = File.Create ("test.txt"))
            using (TextWriter writer = new StreamWriter (fs))
            {  
                writer.WriteLine ("Line1"); 
                writer.WriteLine ("Line2");
            }
            using (FileStream fs = File.OpenRead ("test.txt"))
            using (TextReader reader = new StreamReader (fs))
            { 
                Console.WriteLine (reader.ReadLine());       // Line1
                Console.WriteLine (reader.ReadLine());       // Line2
            }

          using (TextWriter w = File.CreateText ("data.txt"))
          { 
              w.WriteLine (123);          // Writes "123"
              w.WriteLine (true);         // Writes the word "true"
          }
            using (TextReader r = File.OpenText ("data.txt"))
            { 
                int myInt = int.Parse (r.ReadLine());     // myInt == 123 
                bool yes = bool.Parse (r.ReadLine());     // yes == true
            }
                
            //compresion and decompression
            string[] words = "The quick brown fox jumps over the lazy dog".Split();
            Random rand = new Random();
            
            using (Stream s = File.Create ("compressed.bin"))
            using (Stream ds = new DeflateStream (s, CompressionMode.Compress))
            using (TextWriter w = new StreamWriter (ds))  
                for (int i = 0; i < 1000; i++)  
                   w.Write(words [rand.Next (words.Length)] + " ");

            Console.WriteLine (new FileInfo ("compressed.bin").Length);      // 1073
         
            using (Stream s = File.OpenRead ("compressed.bin"))
            using (Stream ds = new DeflateStream (s, CompressionMode.Decompress))
            using (TextReader r = new StreamReader (ds))  
                Console.Write ( r.ReadToEnd());

            Console.WriteLine(Environment.SpecialFolder.MyDocuments);


           // Watch(@"L:\Example", "*.txt", true); 

            //memory mapped file
        using (MemoryMappedFile mmFile = MemoryMappedFile.CreateNew ("Demo", 500))
            using (MemoryMappedViewAccessor accessor = mmFile.CreateViewAccessor())
            {  
                accessor.Write (0, 12345);

                byte[] data = Encoding.UTF8.GetBytes("This is a test");
                accessor.Write(0, data.Length); 
                accessor.WriteArray(4, data, 0, data.Length);
               // Console.ReadLine();   // Keep shared memory alive until user hits Enter.
                
               
             
                var  data2 = new Data { X = 123, Y = 456 };
                accessor.Write (0, ref data2);
                accessor.Read (0, out data2);
                Console.WriteLine (data2.X + " " + data2.Y);   // 123 456

            }

            //isoalated storage
            // IsolatedStorage classes live in System.IO.IsolatedStorage
            using (IsolatedStorageFile f =       
                IsolatedStorageFile.GetMachineStoreForDomain())
            using (var s = new IsolatedStorageFileStream ("hi.txt",FileMode.Create,f))
            using (var writer = new StreamWriter (s)) 
                writer.WriteLine ("Hello, World");

            // Read it back:
            using (IsolatedStorageFile f =     
                IsolatedStorageFile.GetMachineStoreForDomain())
            using (var s = new IsolatedStorageFileStream ("hi.txt", FileMode.Open, f))
            using (var reader = new StreamReader (s))  
                Console.WriteLine (reader.ReadToEnd());        // Hello, world


        }


        //watching for events
        static void Watch(string path, string filter, bool includeSubDirs)
        {
            using (var watcher = new FileSystemWatcher(path, filter))
            {
                watcher.Created += FileCreatedChangedDeleted;
                watcher.Changed += FileCreatedChangedDeleted;
                watcher.Deleted += FileCreatedChangedDeleted;
                watcher.Renamed += FileRenamed;
                watcher.Error += FileError;
                watcher.IncludeSubdirectories = includeSubDirs;
                watcher.EnableRaisingEvents = true;
                Console.WriteLine("Listening for events - press <enter> to end");
                Console.ReadLine();
            }  // Disposing the FileSystemWatcher stops further events from firing.
        }

            static void FileCreatedChangedDeleted (object o, FileSystemEventArgs e)
            {  
                Console.WriteLine ("File {0} has been {1}", e.FullPath, e.ChangeType);
            }
        static void FileRenamed (object o, RenamedEventArgs e)
        {
            Console.WriteLine ("Renamed: {0}->{1}", e.OldFullPath, e.FullPath);
        }
        
        static void FileError (object o, ErrorEventArgs e)
        {  
            Console.WriteLine ("Error: " + e.GetException().Message);
        }

        static byte[] ReadMessage (PipeStream s)
        {  
            MemoryStream ms = new MemoryStream();  
            byte[] buffer = new byte [0x1000];      // Read in 4 KB blocks  
            do    
            { 
                ms.Write (buffer, 0, s.Read (buffer, 0, buffer.Length)); 
            }  while (!s.IsMessageComplete);  
            return ms.ToArray();
        }
    }
    struct Data { public int X, Y; }
}
