using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Text;


namespace WebPageServer
{
    class Program
    {
        static void Main(string[] args)
        {
            // Listen on port 51111, serving files in e:\webroot:
            var server = new WebServer("http://localhost:51111/", @"e:\webroot");
            try
            {
                server.Start();
                Console.WriteLine("Server running... press Enter to stop");
                Console.ReadLine();
            }
            finally { server.Stop(); }
        }


            class WebServer
            {
              HttpListener _listener;
              string _baseFolder;      // Your web page folder.

              public WebServer (string uriPrefix, string baseFolder)
              {
                _listener = new HttpListener();
                _listener.Prefixes.Add (uriPrefix);
                _baseFolder = baseFolder;
              }

              public void Start()
              {
                _listener.Start();
                while (true)
                  try 
                  {
                    var context = _listener.GetContext();
                    Task.Factory.StartNew (() => ProcessRequestAsync (context));
                  }
                  catch (HttpListenerException)     { break; }   // Listener stopped.
                  catch (InvalidOperationException) { break; }   // Listener stopped.
              }

              public void Stop() { _listener.Stop(); }

             // async
                  void ProcessRequestAsync (HttpListenerContext context)
              {
                try
                {
                  string filename = Path.GetFileName (context.Request.RawUrl);
                  string path = Path.Combine (_baseFolder, filename);
                  byte[] msg;
                  if (!File.Exists (path))
                  {
                    Console.WriteLine ("Resource not found: " + path);
                    context.Response.StatusCode = (int) HttpStatusCode.NotFound;
                    msg = Encoding.UTF8.GetBytes ("Sorry, that page does not exist");
                  }
                  else
                  {
                    context.Response.StatusCode = (int) HttpStatusCode.OK;
                    msg = File.ReadAllBytes (path);
                  }
                  context.Response.ContentLength64 = msg.Length;
                  using (Stream s = context.Response.OutputStream)
                 //   await 
                 s.Write(msg, 0, msg.Length);
                }
                catch (Exception ex) { Console.WriteLine ("Request error: " + ex); }
              }
            }
    }
}
