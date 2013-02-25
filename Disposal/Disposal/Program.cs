using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;


namespace Disposal
{
    class Program
    {
        static void Main(string[] args)
        {
            // These types are in System.Diagnostics:
            string procName = Process.GetCurrentProcess().ProcessName;
            using (PerformanceCounter pc = new PerformanceCounter   
                ("Process", "Private Bytes", procName))
                Console.WriteLine (pc.NextValue());
            TempFileRef tfr = new TempFileRef("c:/badDest");
            GC.Collect(); 
            GC.WaitForPendingFinalizers();
            GC.Collect();
           // Console.WriteLine(tfr.DeletionError);
           
            using (var tmr = new System.Threading.Timer 
                (TimerTick, null, 1000, 1000)){ 
                GC.Collect(); 
               // System.Threading.Thread.Sleep (10000);    // Wait 10 seconds
            }

            Widget one = new Widget("one"); 
            Widget two = new Widget("two");
            Widget.ListAllWidgets();
            
        }
        static void TimerTick(object notUsed) { Console.WriteLine("tick"); }
    }

    

    public class TempFileRef
    { 
        static ConcurrentQueue<TempFileRef> _failedDeletions    = 
            new ConcurrentQueue<TempFileRef>(); 
        public readonly string FilePath;
        int _deleteAttempt;
        public Exception DeletionError { get; private set; }  
        
        public TempFileRef (string filePath) { FilePath = filePath; }  
       
        ~TempFileRef()
        {
            Console.WriteLine("destructor of tempFileRef);
            try { File.Delete (FilePath); 
            } catch (Exception ex)   
            {
                if (_deleteAttempt++ < 3) GC.ReRegisterForFinalize(this);
                else
                {
                    DeletionError = ex;
                    _failedDeletions.Enqueue(this);   // Resurrection    
                }
            }  
        }
    }

    class Widget{  
        static List<WeakReference> _allWidgets = new List<WeakReference>();  
        public readonly string Name; 
        public Widget (string name)  {   
            Name = name;    
            _allWidgets.Add (new WeakReference (this));  
        }  
        public static void ListAllWidgets()  {
            foreach (WeakReference weak in _allWidgets) { 
                Widget w = (Widget)weak.Target;
                if (w != null) Console.WriteLine(w.Name); 
            }
        }
    }
}
