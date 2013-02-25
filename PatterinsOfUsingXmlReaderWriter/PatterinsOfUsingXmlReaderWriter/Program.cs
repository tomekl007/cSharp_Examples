using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace PatterinsOfUsingXmlReaderWriter
{
    public class Contacts {
        public IList<Customer> Customers = new List<Customer>();
        public IList<Supplier> Suppliers = new List<Supplier>();
        
        public void ReadXml (XmlReader r){  
            bool isEmpty = r.IsEmptyElement;           // This ensures we don't get  
            r.ReadStartElement();                      // snookered by an empty  
            if (isEmpty) return;                       // <contacts/> element! 
            while (r.NodeType == XmlNodeType.Element)  {    
                if (r.Name == Customer.XmlName)      
                    Customers.Add (new Customer (r));    
                else if (r.Name == Supplier.XmlName) 
                    Suppliers.Add (new Supplier (r));  
                else      
                    throw new XmlException ("Unexpected node: " + r.Name);  
            }  
            r.ReadEndElement();
        }
        
        public void WriteXml (XmlWriter w){ 
            foreach (Customer c in Customers)  {   
                w.WriteStartElement (Customer.XmlName); 
                c.WriteXml (w);   
                w.WriteEndElement();  
            }  
            foreach (Supplier s in Suppliers)  {  
                w.WriteStartElement (Supplier.XmlName); 
                s.WriteXml (w);   
                w.WriteEndElement(); 
            }
        }
    }
    public class Customer { 
        public const string XmlName = "customer"; 
        public int? ID; 
        public string FirstName, LastName; 
        public Customer() { } public Customer(XmlReader r) { ReadXml(r); }
        public void ReadXml(XmlReader r) { 
            if (r.MoveToAttribute("id")) 
                ID = r.ReadContentAsInt();
            r.ReadStartElement();
            FirstName = r.ReadElementContentAsString("firstname", ""); 
            LastName = r.ReadElementContentAsString("lastname", "");
            r.ReadEndElement(); } 
        
        public void WriteXml(XmlWriter w) { 
            if (ID.HasValue) 
            w.WriteAttributeString("id", "", ID.ToString()); 
            w.WriteElementString("firstname", FirstName); 
            w.WriteElementString("lastname", LastName); 
        }
    }

    public class Supplier {
        public const string XmlName = "supplier";
        public string Name; public Supplier() { } 
        public Supplier(XmlReader r) { ReadXml(r); }
        public void ReadXml(XmlReader r) {
            r.ReadStartElement(); 
            Name = r.ReadElementContentAsString("name", "");
            r.ReadEndElement(); }
        public void WriteXml (XmlWriter w)  {  
            w.WriteElementString ("name", Name); 
        }
    }
    


    class Program
    {
        static void Main(string[] args)
        {

            using (XmlWriter w = XmlWriter.Create ("log.xml"))
            { 
                w.WriteStartElement ("log");  
                for (int i = 0; i < 10; i++) 
                {  
                    XElement e = new XElement ("logentry",        
                        new XAttribute ("id", i),              
                        new XElement ("date", DateTime.Today.AddDays (-1)),
                        new XElement ("source", "test"));  
                    e.WriteTo (w);  
                }  
                w.WriteEndElement ();
            }

            XmlDocument doc2 = new XmlDocument();
            doc2.Load("customer.xml"); 
            Console.WriteLine(doc2.DocumentElement.ChildNodes[0].InnerText);   // Jim
            Console.WriteLine (doc2.DocumentElement.ChildNodes[1].InnerText);   // Bo
            Console.WriteLine (  doc2.DocumentElement.ChildNodes[1].ParentNode.Name);        // customer
            
            doc2.DocumentElement.ChildNodes[0].InnerText = "Jo";              // wrong
            doc2.DocumentElement.ChildNodes[0].FirstChild.InnerText = "Jo";   // right
        //creating xml doc
            XmlDocument doc = new XmlDocument(); 
            doc.AppendChild(doc.CreateXmlDeclaration("1.0", null, "yes"));
            XmlAttribute id = doc.CreateAttribute("id"); 
            XmlAttribute status = doc.CreateAttribute("status");
            id.Value = "123"; status.Value = "archived"; 
            XmlElement firstname = doc.CreateElement("firstname");
            XmlElement lastname = doc.CreateElement("lastname"); 
            firstname.AppendChild(doc.CreateTextNode("Jim")); 
            lastname.AppendChild(doc.CreateTextNode("Bo"));
            XmlElement customer = doc.CreateElement("customer");
            customer.Attributes.Append(id); 
            customer.Attributes.Append(status);
            customer.AppendChild(lastname); 
            customer.AppendChild(firstname);
            doc.AppendChild(customer);
//xpath nav
          //  XPathNavigator nav = doc.CreateNavigator(); 
           // XPathNavigator jim = nav.SelectSingleNode(
         //       "customers/customer[firstname='Jim']"
         //       ); 
         //   //Console.WriteLine(jim.Value);                    // JimBo

            XPathNavigator nav = doc2.CreateNavigator();
            string xPath = "./customer/firstname/text()";
            
            foreach (XPathNavigator navC in nav.Select(xPath)) 
                Console.WriteLine(navC.Value);

        
        }
    }
}
