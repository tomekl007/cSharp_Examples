using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace WritingHTTPServer
{
    class Program
    {
        static void Main(string[] args)
        {

            ListenAsync();                           // Start server
            WebClient wc = new WebClient();          // Make a client request.
            Console.WriteLine(wc.DownloadString
              ("http://localhost:51111/MyApp/Request.txt"));
        }

        //async
        static void ListenAsync()
        {
          HttpListener listener = new HttpListener();
          listener.Prefixes.Add ("http://localhost:51111/MyApp/");  // Listen on
          listener.Start();                                         // port 51111.

          // Await a client request:
          HttpListenerContext context = listener.GetContext();
              //await listener.GetContextAsync();

          // Respond to the request:
          string msg = "You asked for: " + context.Request.RawUrl;
          context.Response.ContentLength64 = Encoding.UTF8.GetByteCount (msg);
          context.Response.StatusCode = (int) HttpStatusCode.OK;

          using (Stream s = context.Response.OutputStream)
          using (StreamWriter writer = new StreamWriter(s))
              writer.Write(msg);
              //await writer.WriteAsync (msg);

          listener.Stop();
        }
    }
}
