using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Net.Mail;

namespace Networking
{
    class Program
    {
        static void Main(string[] args)
        {
            IPAddress a1 = new IPAddress(new byte[] { 101, 102, 103, 104 });
            IPAddress a2 = IPAddress.Parse("101.102.103.104"); 
            Console.WriteLine(a1.Equals(a2));                     // True
            Console.WriteLine (a1.AddressFamily);                   // InterNetwork

            IPAddress a3 = IPAddress.Parse("[3EA0:FFFF:198A:E4A3:4FF2:54fA:41BC:8D31]");
            Console.WriteLine (a3.AddressFamily);   // InterNetworkV6

            IPAddress a = IPAddress.Parse("101.102.103.104");
            IPEndPoint ep = new IPEndPoint(a, 222);           // Port 222
            Console.WriteLine (ep.ToString());                 // 101.102.103.104:222

            Uri info = new Uri("http://www.domain.com:80/info/");
            Uri page = new Uri("http://www.domain.com/info/page.html");
            Console.WriteLine(info.Host);     // www.domain.com
           
            Console.WriteLine(info.Port);     // 80
            Console.WriteLine (page.Port);     // 80  (Uri knows the default HTTP port)
            Console.WriteLine (info.IsBaseOf (page));         // True
            Uri relative = info.MakeRelativeUri (page);
            Console.WriteLine (relative.IsAbsoluteUri);       // False
            Console.WriteLine (relative.ToString());          // page.html

            WebClient wc = new WebClient(); 
            wc.Proxy = null; 
            wc.DownloadFile("http://www.albahari.com/nutshell/code.aspx", "code.htm");
        //    System.Diagnostics.Process.Start("code.htm");
            

           // var wc = new WebClient();
            
            wc.DownloadProgressChanged += (sender, argss) => 
              Console.WriteLine (argss.ProgressPercentage + "% complete");

           // Task.Delay (5000).ContinueWith (ant => wc.CancelAsync());
  
            
         //   wc.DownloadFileAsync ("http://oreilly.com", "webpage.htm");

            WebRequest req = WebRequest.Create
                ("http://www.albahari.com/nutshell/code.html");
            req.Proxy = null;
            using (WebResponse res = req.GetResponse())
            using (Stream rs = res.GetResponseStream())
            using (FileStream fs = File.Create("code.html"))
              rs.CopyTo(fs);
            
         // Create a WebProxy with the proxy's IP address and port. You can
            // optionally set Credentials if the proxy needs a username/password.

     //       WebProxy p = new WebProxy ("192.178.10.49", 808);
      //      p.Credentials = new NetworkCredential ("username", "password");
            // or:
     //       p.Credentials = new NetworkCredential ("username", "password", "domain");

     //       WebClient wc2 = new WebClient();
     //       wc2.Proxy = p;
              

    //        // Same procedure with a WebRequest object:
    //        WebRequest req2 = WebRequest.Create ("...");
   //         req2.Proxy = p;
           
            //auth
            WebClient wc3 = new WebClient();
            wc3.Proxy = null;
            wc3.BaseAddress = "ftp://ftp.albahari.com";

            // Authenticate, then upload and download a file to the FTP server.
            // The same approach also works for HTTP and HTTPS.

            string username = "nutshell";
            string password = "oreilly";
            wc3.Credentials = new NetworkCredential(username, password);

            wc3.DownloadFile("guestbook.txt", "guestbook.txt");

            string data = "Hello from " + Environment.UserName + "!\r\n";
            File.AppendAllText("guestbook.txt", data);

            wc3.UploadFile("guestbook.txt", "guestbook.txt");


            //
            CredentialCache cache = new CredentialCache();
            Uri prefix = new Uri("http://exchange.mydomain.com");
            cache.Add(prefix, "Digest", new NetworkCredential("joe", "passwd"));
            cache.Add(prefix, "Negotiate", new NetworkCredential("joe", "passwd"));

           // WebClient wc = new WebClient();
           // wc.Credentials = cache;

            //exception handling
            WebClient wc4 = new WebClient();
            try
            {
                wc4.Proxy = null;
                string s = wc4.DownloadString("http://www.albahari.com/notthere");
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.NameResolutionFailure)
                    Console.WriteLine("Bad domain name");
                else if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    HttpWebResponse response = (HttpWebResponse)ex.Response;
                    Console.WriteLine(response.StatusDescription);      // "Not Found"
                    if (response.StatusCode == HttpStatusCode.NotFound)
                        Console.WriteLine("Not there!");                  // "Not there!"
                }
                else throw;
            }

            //http headers
            WebClient wc5 = new WebClient();
            wc5.Proxy = null;
            wc5.Headers.Add("CustomHeader", "JustPlaying/1.0");
            wc5.DownloadString("http://www.oreilly.com");

            foreach (string name in wc.ResponseHeaders.Keys)
                Console.WriteLine(name + "=" + wc.ResponseHeaders[name]);

            //query string
            WebClient wc6 = new WebClient();
            wc6.Proxy = null;
            wc6.QueryString.Add("q", "WebClient");     // Search for "WebClient"
            wc6.QueryString.Add("hl", "fr");           // Display page in French
            wc6.DownloadFile("http://www.google.com/search", "results.html");
         //   System.Diagnostics.Process.Start("results.html");
            //same asstring 
        //  string search = Uri.EscapeDataString ("(WebClient OR HttpClient)");
          //  string language = Uri.EscapeDataString("fr");
         //   string requestURI = "http://www.google.com/search?q=" + search +
                          //      "&hl=" + language;

            //uploading data with WebClient
            WebClient wc7 = new WebClient();
            wc7.Proxy = null;

            var data2 = new System.Collections.Specialized.NameValueCollection();
            data2.Add("Name", "Joe Albahari");
            data2.Add("Company", "O'Reilly");
            
            byte[] result = wc7.UploadValues("http://www.albahari.com/EchoPost.aspx",
                                             "POST", data2);

            Console.WriteLine(Encoding.UTF8.GetString(result));

            //uploading with WebRequest
           /* var req2 = WebRequest.Create("http://www.albahari.com/EchoPost.aspx");
            req2.Proxy = null;
            req2.Method = "POST";
            req2.ContentType = "application/x-www-form-urlencoded";

            string reqString = "Name=Joe+Albahari&Company=O'Reilly";
            byte[] reqData = Encoding.UTF8.GetBytes(reqString);
            req2.ContentLength = reqData.Length;

            using (Stream reqStream = req.GetRequestStream())
                reqStream.Write(reqData, 0, reqData.Length);

            using (WebResponse res = req.GetResponse())
            using (Stream resSteam = res.GetResponseStream())
            using (StreamReader sr = new StreamReader(resSteam))
                Console.WriteLine(sr.ReadToEnd());*/

            //cookies
            CookieContainer cc = new CookieContainer();

            var request = (HttpWebRequest)WebRequest.Create("http://www.google.com");
            request.Proxy = null;
            request.CookieContainer = cc;
            using (var response = (HttpWebResponse)request.GetResponse())
            {
                foreach (Cookie c in response.Cookies)
                {
                    Console.WriteLine(" Name:   " + c.Name);
                    Console.WriteLine(" Value:  " + c.Value);
                    Console.WriteLine(" Path:   " + c.Path);
                    Console.WriteLine(" Domain: " + c.Domain);
                }
                // Read response stream...
            }

            //FORM auth
            //ex website: 
            //<form action="http://www.somesite.com/login" method="post"> 
          //  <input type="text" id="user" name="username">  
           // <input type="password" id="pass" name="password"> 
          //  <button type="submit" id="login-btn">Log In</button>
          //      </form>

            string loginUri = "http://www.somesite.com/login";
           // string 
                username = "username";   // (Your username)
         //   string 
                password = "password";   // (Your password)
            string reqString = "username=" + username + "&password=" + password;
            byte[] requestData = Encoding.UTF8.GetBytes(reqString);

            CookieContainer cc2 = new CookieContainer();
            var request2 = (HttpWebRequest)WebRequest.Create(loginUri);
            request2.Proxy = null;
            request2.CookieContainer = cc2;
            request2.Method = "POST";

            request2.ContentType = "application/x-www-form-urlencoded";
            request2.ContentLength = requestData.Length;

            using (Stream s = request2.GetRequestStream())
                s.Write(requestData, 0, requestData.Length);

            using (var response = (HttpWebResponse)request2.GetResponse())
                foreach (Cookie c in response.Cookies)
                    Console.WriteLine(c.Name + " = " + c.Value);

            // We're now logged in. As long as we assign cc to subsequent WebRequest
            // objects, we’ll be treated as an authenticated user.

            //using FTP
            var req3 = (FtpWebRequest)WebRequest.Create("ftp://ftp.albahari.com");
            req3.Proxy = null;
            req3.Credentials = new NetworkCredential("nutshell", "oreilly");
            req3.Method = WebRequestMethods.Ftp.ListDirectory;

            using (WebResponse resp = req3.GetResponse())
            using (StreamReader reader = new StreamReader(resp.GetResponseStream()))
                Console.WriteLine(reader.ReadToEnd());
            
            foreach (IPAddress aa in Dns.GetHostAddresses("albahari.com"))
                Console.WriteLine(aa.ToString());     // 205.210.42.167


            //send email
            SmtpClient client = new SmtpClient();
            client.Host = "mail.myisp.net";
            MailMessage mm = new MailMessage();
            
            mm.Sender = new MailAddress("kay@domain.com", "Kay");
            mm.From = new MailAddress("kay@domain.com", "Kay");
            mm.To.Add(new MailAddress("bob@domain.com", "Bob"));
            mm.CC.Add(new MailAddress("dan@domain.com", "Dan"));
            mm.Subject = "Hello!";
            mm.Body = "Hi there. Here's the photo!";
            mm.IsBodyHtml = false;
            mm.Priority = MailPriority.High;

            Attachment att = new Attachment("photo.jpg",
                                           System.Net.Mime.MediaTypeNames.Image.Jpeg);
            mm.Attachments.Add(att);

            client.Send(mm);
        }
    }
}
