using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml;
using System.IO;

namespace linqToXml
{
    class Program
    {
        static void Main(string[] args)
        {
            string xml = @"<customer id='123' status='archived'>                
                            <firstname>Joe</firstname>                 
                            <lastname>Bloggs<!--nice name--></lastname>       
                            </customer>"; 
            XElement customer = XElement.Parse(xml);

            //XDocument fromWeb = XDocument.Load("http://albahari.com/sample.xml");
            XElement fromFile = XElement.Load(@"F:\UJ\PMD\GpsApiLow - Copy\res\layout\activity_gps.xml");
            Console.WriteLine(fromFile);

            XElement config = XElement.Parse(@"<configuration>    
                                            <client enabled='true'>      
                                            <timeout>30</timeout>    
                                            </client> 
                                            </configuration>");

            foreach (XElement child in config.Elements()) 
                Console.WriteLine(child.Name);                     // client

            XElement client = config.Element ("client");

            bool enabled = (bool) client.Attribute ("enabled");   // Read attribute

            Console.WriteLine(enabled);                          // True

            client.Attribute ("enabled").SetValue (!enabled);     // Update attribute
            int timeout = (int) client.Element ("timeout");       // Read element
            
            Console.WriteLine (timeout);                          // 30
            client.Element ("timeout").SetValue (timeout * 2);    // Update element
            client.Add (new XElement ("retries", 3));             // Add new elememt
            
            Console.WriteLine (config);         // Implicitly call config.ToString()

            //constructing xml 
            XElement lastName = new XElement("lastname", "Bloggs");
            lastName.Add(new XComment("nice name")); 
            XElement customer2 = new XElement("customer"); 
            customer2.Add(new XAttribute("id", 123)); 
            customer2.Add(new XElement("firstname", "Joe")); 
            customer2.Add(lastName); 
            Console.WriteLine(customer2.ToString());

            //functional construction
            XElement customer3 = new XElement("customer", 
                                    new XAttribute("id", 123),
                                     new XElement("firstname", "joe"), 
                                      new XElement("lastname", "bloggs",
                                        new XComment("nice name")));

            //with linq
          /*  XElement query = new XElement("customers", from c in dataContext.Customers 
                                                       select new XElement("customer",
                                                           new XAttribute("id", c.ID),
                                                           new XElement("firstname", c.FirstName), 
                                                           new XElement("lastname", c.LastName, 
                                                               new XComment("nice name"))));
           */
            
            //automatic deep cloning
            var address = new XElement("address", 
                new XElement("street", "Lawley St"), 
                new XElement("town", "North Beach")); 
            
            var customer1 = new XElement("customer1", address);
            var customer2a = new XElement("customer2", address); 
            customer1.Element("address").Element("street").Value = "Another St"; 
            Console.WriteLine(customer2a.Element("address").Element("street").Value);   // Lawley St


            //traversing

            var bench = new XElement("bench", 
                new XElement("toolbox", 
                    new XElement("handtool", "Hammer"),
                    new XElement("handtool", "Rasp")),
                new XElement("toolbox", 
                        new XElement("handtool", "Saw"),
                        new XElement("powertool", "Nailgun")),
                        new XComment("Be careful with the nailgun"));
            
            foreach (XNode node in bench.Nodes()) 
                Console.WriteLine(node.ToString(SaveOptions.DisableFormatting) + ".");

            foreach (XElement e in bench.Elements())
                Console.WriteLine(e.Name + "=" + e.Value);


            IEnumerable<string> query = from toolbox in bench.Elements()
                                        where toolbox.Elements().Any(tool => tool.Value == "Nailgun")
                                        select toolbox.Value;
            
            IEnumerable<string> query2 = from toolbox in bench.Elements() 
                                         from tool in toolbox.Elements()
                                         where tool.Name == "handtool"
                                         select tool.Value;
          
            int x = bench.Elements("toolbox").Count();

            //XElement settings = XElement.Load("databaseSettings.xml");
            //string cx = fromFile.Element("RelativeLayout").Element("textView").Value;
            string cx = (string) fromFile.Element("TextView").Value;
            Console.WriteLine(cx);

            Console.WriteLine(bench.Descendants("handtool").Count());  // 3
       

            foreach (XNode node in bench.DescendantNodes()) 
                Console.WriteLine(node.ToString(SaveOptions.DisableFormatting));
           //The next query extracts all comments anywhere within the
            //X-DOM that contain theword “careful”:
            IEnumerable<string> queryC =  from c in bench.DescendantNodes().OfType<XComment>() 
                                         where c.Value.Contains ("careful")
                                         orderby c.Value  select c.Value;
            foreach (string node in queryC)
                Console.WriteLine(node);

            foreach (XNode child in bench.Nodes()) 
                Console.WriteLine(child.Parent == bench);

            XElement settings = new XElement("settings", 
                new XElement("timeout", 30)); 
            settings.SetValue("blah");
            
            Console.WriteLine(settings.ToString());  // <settings>blah</settings>

            XElement settings2 = new XElement("settings"); 
            settings2.SetElementValue("timeout", 30);     // Adds child node
            settings2.SetElementValue ("timeout", 60);     // Update it to 60

            XElement items = new XElement("items", 
                                new XElement("one"), 
                                  new XElement("three")); 
            
            items.FirstNode.AddAfterSelf(new XElement("two"));

            XElement items2 = XElement.Parse("<items><one/><two/><three/></items>"); 
            items2.FirstNode.ReplaceWith(new XComment("One was here"));

            XElement contacts = XElement.Parse(@"<contacts>    
                                                <customer name='Mary'/>    
                                                <customer name='Chris' archived='true'/>    
                                                <supplier name='Susan'>      
                                                <phone archived='true'>012345678<!--confidential--></phone> 
                                                </supplier> 
                                                </contacts>");
            //The following removes all customers:
            contacts.Elements ("customer").Remove();
            //The next statement removes all archived contacts (so Chris disappears):
            contacts.Elements().Where (e => (bool?) e.Attribute ("archived") == true).Remove();

            contacts.Elements().Where(e => e.DescendantNodes().OfType<XComment>()
                .Any(c => c.Value == "confidential")).Remove();
            //delete all comments
            contacts.DescendantNodes().OfType<XComment>().Remove();


            var ee = new XElement("date", DateTime.Now);
            ee.SetValue(DateTime.Now.AddDays(1)); 
                Console.Write(ee.Value);

            XElement ex = new XElement("now", DateTime.Now); 
            DateTime dt = (DateTime)ex; 

            XAttribute a = new XAttribute("resolution", 1.234);
            double res = (double)a;

           // int? timeout2 = (int)config.Element("timeout");      // Errorint? 
           int? timeout2 = (int?) config.Element ("timeout");    // OK; timeout is null.

            var data = XElement.Parse (  @"<data>      
                            <customer id='1' name='Mary' credit='100' />      
                            <customer id='2' name='John' credit='150' />
                            <customer id='3' name='Anne' />    </data>");

            IEnumerable<string> query5 = from cust in data.Elements() 
                                        where (int?)cust.Attribute("credit") > 100 
                                        select cust.Attribute("name").Value;
            foreach (string child in query5)
                Console.WriteLine(child);

            //xhtml file
            var styleInstruction = new XProcessingInstruction (  "xml-stylesheet", 
                "href='styles.css' type='text/css'");
            
            var docType = new XDocumentType ("html",  "-//W3C//DTD XHTML 1.0 Strict//EN", 
                "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd", null);
            
            XNamespace ns = "http://www.w3.org/1999/xhtml";
            
            var root =  new XElement (ns + "html",   
                new XElement (ns + "head",      
                    new XElement (ns + "title", "An XHTML page")),    
                    new XElement (ns + "body",
                        new XElement(ns + "p", "This is the content")));
            
            var doc = new XDocument(new XDeclaration("1.0", "utf-8", "no"),
                new XComment("Reference a stylesheet"),
                styleInstruction,
                docType,
                root); 
            
            doc.Save("test.html");

            Console.WriteLine(doc.Root.Name.LocalName);          // html
            XElement bodyNode = doc.Root.Element (ns + "body");
            Console.WriteLine (bodyNode.Document == doc); // TrueRecall that a document’s children have no Parent:
            Console.WriteLine (doc.Root.Parent == null);          // True
            
            foreach (XNode node in doc.Nodes()) 
                Console.Write (node.Parent == null);                // TrueTrueTrueTrue

            var doc2 = new XDocument(new XDeclaration("1.0", "utf-8", "yes"),
                new XElement("test", "data")); 
            
            var output = new StringBuilder();
            var settings3 = new XmlWriterSettings { Indent = true };
            using (XmlWriter xw = XmlWriter.Create(output, settings3))
                doc2.Save(xw);
            Console.WriteLine(output.ToString());
            //write xml to a file
            File.WriteAllText("data.xml", doc2.ToString());

            String toExclude = (@"<Project><ItemGroup>    
                <Compile Include='ObjectGraph.cs' />    
                  <Compile Include='Program.cs' />    
                  <Compile Include='Properties\AssemblyInfo.cs' />  
                   <Compile Include='Tests\Aggregation.cs' />   
                  <Compile Include='Tests\Advanced\RecursiveXml.cs' />  
                      </ItemGroup>
                    <ItemGroup>
                    </ItemGroup>
                    </Project>"
               );
            XElement project = XElement.Parse(toExclude);
            Console.WriteLine(project);

           // XNamespace ns = project.Name.Namespace;
            var query7 = new XElement("ProjectReport", 
                from compileItem in project.Elements("ItemGroup")
                    .Elements("Compile") 
                let include = compileItem.Attribute("Include")
                where include != null 
                select new XElement("File", include.Value));

            Console.WriteLine(query7);

              //xml reader
      
        XmlReaderSettings settings7 = new XmlReaderSettings();
        
        settings7.IgnoreWhitespace = true;
        settings7.ProhibitDtd = false;      // Must set this to read DTDs
        
        using (XmlReader r = XmlReader.Create ("customer.xml", settings7)) 
            while (r.Read())  {
                //r.MoveToContent();
                Console.Write (r.NodeType.ToString().PadRight (17, '-'));  
                Console.Write ("> ".PadRight (r.Depth * 3));
                //read attribute
                //Console.WriteLine(r["id"]);              // 123
               // Console.WriteLine (r ["status"]);          // archived
              //  Console.WriteLine (r ["bogus"] == null);   // True
                //or
                      // Console.WriteLine(reader[0]);            // 123
                      //  Console.WriteLine (reader [1]);            // archived
                
                switch (r.NodeType)    
                {     
                    case XmlNodeType.Element:      
                    case XmlNodeType.EndElement:      
                    Console.WriteLine (r.Name); break;     
                    
                    case XmlNodeType.Text:      
                    case XmlNodeType.CDATA:     
                    case XmlNodeType.Comment:    
                    case XmlNodeType.XmlDeclaration:     
                    Console.WriteLine (r.Value); break;     
                    
                    case XmlNodeType.DocumentType:     
                        Console.WriteLine (r.Name + " - " + r.Value); break;

                    default: break;
                }
            }

       
            using (XmlReader reader = XmlReader.Create("customer.xml")) 
        if (reader.MoveToFirstAttribute()) 
            do { 
                Console.WriteLine(reader.Name + "=" + reader.Value); 
            } while (reader.MoveToNextAttribute());
            // OUTPUT:id=123status=archived


        
        XmlWriterSettings settingsW = new XmlWriterSettings();
        settingsW.Indent = true;
        using (XmlWriter writer = XmlWriter.Create ("..\\..\\foo.xml", settingsW))
         {  
        writer.WriteStartElement ("customer");  
            writer.WriteElementString ("firstname", "Jim");
            writer.WriteElementString ("lastname"," Bo");
            writer.WriteElementString("lastname", " Bo"); 
         //   writer.WriteEndElement();
            writer.WriteStartElement("customer"); 
            writer.WriteAttributeString("id", "1");
            writer.WriteAttributeString("status", "archived");
            writer.WriteEndElement();
            
            writer.WriteStartElement("o", "customer", "http://oreilly.com"); 
            writer.WriteElementString("o", "firstname", "http://oreilly.com", "Jim");
            writer.WriteElementString("o", "lastname", "http://oreilly.com", "Bo");
            writer.WriteEndElement();
         }
        }

        static IEnumerable<XElement> ExpandPaths(IEnumerable<string> paths) 
        { 
            var brokenUp = from path in paths 
                           let split = path.Split(new char[] { '\\' }, 2) 
                           orderby split[0] 
                           select new { name = split[0],
                               remainder = split.ElementAtOrDefault(1) };
            
            IEnumerable<XElement> files = from b in brokenUp 
                                          where b.remainder == null 
                                          select new XElement("file", b.name); 
            
            IEnumerable<XElement> folders = from b in brokenUp 
                                            where b.remainder != null 
                                            group b.remainder by b.name 
                                            into grp 
                                            select new XElement("folder",
                                                new XAttribute("name", grp.Key), 
                                                ExpandPaths(grp)); 
            return files.Concat(folders); 
        }
        
          

        }

    }

  

