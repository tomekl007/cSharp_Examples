using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Data.Objects;
using System.Linq.Expressions;
using System.Resources;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Collections;
using System.Xml.Linq;




namespace Associations
{
    
    class NutshellContext : ObjectContext // For Entity Framework
{
        public NutshellContext(String conStr) : base(conStr)
    {}

    public ObjectSet<Customer> Customers
    {
        get { return CreateObjectSet<Customer>(); }
    }
    public ObjectSet<Purchase> Purchases
    {
        get { return CreateObjectSet<Purchase>(); }
    }
}

    public partial class Customer
    {
        public static Expression<Func<Customer, bool>> isTomek()
        {
            return p => p.Name == "Tomek";
        }

    }


    class Program
    {
        static void Main(string[] args)
        {

         //   DataContext dataContext = new DataContext("LocalDatabase1.mdf");
           // Table<Customer> customers = dataContext.GetTable<Customer>();

            var context = new NutshellContext("name=LocalDatabase1Entities");
            
           // context.DefaultContainerName = "LocalDatabase1Model";
          //  ObjectSet<Customer> customers = context.CreateObjectSet<Customer>();


            //IQueryable<string> query = from c in customers
          //                             where c.Name.Contains("a")
          //                             orderby c.Name.Length
           //                            select c.Name.ToUpper();
            Customer cust = context.Customers.Single(c => c.Id == 1);
            Console.WriteLine(cust.Name);

            //foreach (Customer p in cust)
               // Console.WriteLine(cust.Purchases.First());

            Customer cust1 = context.Customers.OrderBy(c => c.Name).First();
            foreach (Purchase p in cust1.Purchases)
                Console.WriteLine(p);

          //  Purchase  purch1 = context.Purchases.Single(c => c.Id == 1);
           // Console.WriteLine(purch1.Customer.Id);

            Console.WriteLine("here");
            IQueryable<Customer> query = context.Customers.Where(c => c.Name.Contains("o"));
            //Console.WriteLine(query.Name);
            foreach (Customer p in query)
                Console.WriteLine(p.Name);

            Customer c1 = context.Customers.Single(c => c.Id == 2);
            IQueryable<Purchase> queryPur = context.Purchases;
            foreach (Purchase p in queryPur)
                Console.WriteLine(p.Name);

             Console.WriteLine();
            //--------------------------------
           // var context = new NutshellContext("connection string"); 
            var queryIO = from c in context.Customers 
                          select 
                            from p in c.Purchases 
                            select new { c.Name, p.Id };
            
            foreach (var customerPurchaseResults in queryIO) 
                foreach (var namePrice in customerPurchaseResults)
                    Console.WriteLine(namePrice.Name + " id of purchase  " + namePrice.Id);

            var queryNoRound = from c in context.Customers            
                        select new { c.Name, c.Purchases };
            
            foreach (var row in queryNoRound)  
                foreach (Purchase p in row.Purchases)   // No extra round-tripping    
                    Console.WriteLine (row.Name + " buy " + p.Name);

            //enumerate without projected
          //  context.ContextOptions.DeferredLoadingEnabled = true;  // For EF only.
            foreach (Customer c in context.Customers) 
                foreach (Purchase p in c.Purchases)    // Another SQL round-trip
                    Console.WriteLine (c.Name + " spent " + p.Name);

            //it will always filter customer with given predicate
            //DataLoadOptions options = new DataLoadOptions();
            //options.AssociateWith<Customer>(c => c.Purchases.Where(p => p.Price > 1000));
            //context.LoadOptions = options;

            //add delete modify data in database
            Customer custToModyf = context.Customers.Single(c => c.Id == 1); 
            custToModyf.Name = "Bloggs2";
            context.SaveChanges();   // Updates the customer

            //context.Customers.DeleteObject(custToModyf);  // DeleteObject with EF
            //context.SaveChanges();                  // Deletes the customer

            //add rows 
         //   Purchase p1 = new Purchase();
         //   p1.Name = "someWater";
         //   p1.Id  = 75;
         //   p1.CustomerId = 1;//{ ID = 100, Description = "Bike", Price = 500 };
            //Purchase p2 = new Purchase { ID = 101, Description = "Tools", Price = 100 };
            Customer customer1 = context.Customers.Single(c => c.Id == 1); 
        //    cust.Purchases.Add(p1); 
            //cust.Purchases.Add(p2); 
        //    context.SaveChanges();

            //to remove(it set CustomerId field to Null for this Purchase )
            //customer1.Purchases.Remove(customer1.Purchases.Single(p => p.Id == 75));
        //    context.SaveChanges();
            //to delete entity enirely from table
          
            
            context.Purchases.DeleteObject(context.Purchases.Single(p => p.Id == 1));
        
            context.SaveChanges();         // Submit SQL to database
           
            foreach (Customer c in context.Customers)
                foreach (Purchase p in c.Purchases)    // Another SQL round-trip
                    Console.WriteLine(c.Name + " spent " + p.Name);


            IQueryable<Customer> sqlQuery = context.Customers.Where(Customer.isTomek());

            
            foreach (Customer p in sqlQuery)
                Console.WriteLine(p.Name);
            //chapter 9 
            string[] names = { "Tom", "Dick", "Harry", "Marry", "Jay" };
            IEnumerable<String> queryA = names.Where(n => n.Contains("y"));

            int[] numbers = { 1, 2, 4, 100, 23, 1 };
            var takeWhileLess = numbers.TakeWhile(n => n < 100);
            //1,2,4
          
            var takeWhileGreater = numbers.SkipWhile(n => n < 100);
            //100,23,1      

            char[] distinctLetters = "HelloWorld".Distinct().ToArray();
            string s = new string(distinctLetters);                     // HeloWrd
            Console.WriteLine(s);

            IEnumerable<string> queryB = from f in FontFamily.Families
                                         select f.Name;
          //  foreach (string name in queryB) 
          //      Console.WriteLine (name);
            
            //as lambda
            IEnumerable<string> queryC = FontFamily.Families.Select(f => f.Name);

            var queryD = from f in FontFamily.Families
                         select new { f.Name, LineSpacing = f.GetLineSpacing(FontStyle.Bold) };

            IEnumerable<FontFamily> queryE = from f in FontFamily.Families
                                             where f.IsStyleAvailable(FontStyle.Strikeout) 
                                             select f; 
            
           // foreach (FontFamily ff in queryE) Console.WriteLine(ff.Name);

            IEnumerable<string> queryF = names.Select((st, i) => i + "=" + st); 
            //  { "0=Tom", "1=Dick", ... }

            //enumarating directory
            DirectoryInfo[] dirs = new DirectoryInfo(@"L:\Example").GetDirectories(); 
            var queryG = from d in dirs 
                        where (d.Attributes & FileAttributes.System) == 0 
                        select new { DirectoryName = d.FullName, Created = d.CreationTime, 
                            Files = from f in d.GetFiles()
                                    where (f.Attributes & FileAttributes.Hidden) == 0 
                                    select new { FileName = f.Name, f.Length,f.LastWriteTime } }; 
            
            foreach (var dirFiles in queryG) { 
                Console.WriteLine("Directory: " + dirFiles.DirectoryName); 
                foreach (var file in dirFiles.Files)    
                    Console.WriteLine("  " + file.FileName + " Len: " + file.Length); }

            //from EF
            var queryI =  from c in context.Customers  select new {
                c.Name,
                Purchases = from p in context.Purchases 
                            where p.CustomerId == c.Id 
                            select new { p.Name }}; 
            
            foreach (var namePurchases in query) 
            { Console.WriteLine("Customer: " + namePurchases.Name); 
                foreach (var purchaseDetail in namePurchases.Purchases)   
                    Console.WriteLine("  - $$$: " + purchaseDetail.Name); }

                    

            var q = from c in context.Customers
              let highValueP = from p in c.Purchases where p.CustomerId > 1 
                               select new { p.Name, p.Id }
              where highValueP.Any()
              select new { c.Name, Purchases = highValueP };
            
            foreach (var namePurchases in q)
                Console.WriteLine(namePurchases.Name);

            string textImput = "tomek lelek";
            string[] childSeq = textImput.Split();

            int[] numbers2 = { 1, 2, 3 }; 
            string[] letters = { "a", "b" };
            IEnumerable<string> queryK = from n in numbers
                                         from l in letters
                                         select n.ToString() + l;
            //RESULT: { "1a", "1b", "2a", "2b", "3a", "3b" }
            string[] players = { "Tom", "Jay", "Mary" };
            IEnumerable<string> queryQ = from name1 in players
                                        from name2 in players
                                        where name1.CompareTo(name2) < 0
                                        orderby name1, name2
                                        select name1 + " vs " + name2;
            foreach(string name in queryQ)
             Console.WriteLine(name);

            //cross join
            var queryCrossJoin = from c in context.Customers
                                 from p in context.Purchases
                                 select c.Name + " might have bought a " + p.Name;

            foreach (string name in queryCrossJoin)
                Console.WriteLine(name);
            //to get purchuse for concrete Customer add where clause
            var queryInnerJoin = from c in context.Customers
                                 from p in context.Purchases
                                 where c.Id == p.Id
                                 select c.Name + " bought a " + p.Name;
           
            foreach (var name in queryInnerJoin)
                Console.WriteLine(name);
            //to query bedzie miec taki sam result(sql inner join)
            var querryInnerJoin2 = from c in context.Customers
                                   from p in c.Purchases
                                   orderby c.Name descending
                                   select new { c.Name, PurchName = p.Name }; 
                                    
          
            foreach (var name in querryInnerJoin2)
               Console.WriteLine(name);//in EF rob tym sposobem

            var queryDiE = from c in context.Customers
                           from p in c.Purchases.DefaultIfEmpty()
                           select new { c.Name, PurchName=p.Name };
            foreach (var name in queryDiE)
                Console.WriteLine(name);

            //Joining ----------------------------------------------------------
            Customer[] customers = context.Customers.ToArray(); 
            Purchase[] purchases = context.Purchases.ToArray(); 
            var slowQuery = from c in customers 
                            from p in purchases 
                            where c.Id == p.Id
                            select c.Name + " bought a " + p.Name;
            foreach (var name in slowQuery)
                Console.WriteLine(name);
            //inner join like sql
            var fastQuery = from c in customers 
                            join p in purchases on c.Id equals p.Id
                            select c.Name + " bought a " + p.Name;
            foreach (var name in fastQuery)
                Console.WriteLine(name);
            //same result but a lot slower
            foreach (Customer c in customers) 
                foreach (Purchase p in purchases) 
                    if (c.Id == p.Id) 
                        Console.WriteLine(c.Name + "," + p.Name );

            //ILookup is like dictionary
            ILookup<int, Purchase> purchLookup = purchases.ToLookup(p => p.Id, p => p);
                foreach (Purchase p in purchLookup[1])
                    Console.WriteLine(p.Name);

                var fromLookup = from c in customers
                                 from p in purchLookup[c.Id]
                                 select new { c.Name, p.Id, purchName = p.Name };
                foreach (var name in fromLookup)
                    Console.WriteLine(name);
            //zip
            int[] numbers3 = { 3, 5, 7 };
            string[] words = { "three", "five", "seven", "ignored" };
            
            IEnumerable<string> zip = numbers3.Zip (words, (n, w) => n + "=" + w);
            foreach (string name in zip)
                Console.WriteLine(name);

            //group by 
            //Enumerable.GroupBy works by reading the input elements into
            //a temporary dictionaryof lists so that all elements with t
            //he same key end up in the same sublist. It thenemits a s
            //equence of groupings. A grouping is a sequence with a Key property.

            string[] files = Directory.GetFiles("L:\\Downloads");
            //IEnumerable<IGrouping<string,string>> query = 
            //files.GroupBy (file => Path.GetExtension (file));

            int counter = 0;
            var queryFiles = files.GroupBy(file => Path.GetExtension(file))//not sorted
                              .OrderBy(qrouping => qrouping.Key)
                              .Where(g => g.Count() > 6);

            var queryFilesq = from file in files
                              group file.ToUpper() by Path.GetExtension (file) into grouping
                              where grouping.Count() >= 5
                              select grouping;



            foreach (IGrouping<string, string> grouping in queryFilesq) 
            { 
                Console.WriteLine("Extension: " + grouping.Key);
                foreach (string filename in grouping)
                {
                    counter++;
                    Console.WriteLine("   - " + filename);
                }
                Console.WriteLine("number of elements: " + counter);
                counter = 0;
            }
            
            //---
            string[] votes = { "Bush", "Gore", "Gore", "Bush", "Bush" }; 
            IEnumerable<string> queryW = from vote in votes
                                         group vote by vote into g 
                                         orderby g.Count() descending 
                                         select g.Key; 
            string winner = queryW.First();    // Bush
            Console.WriteLine(winner);

            //nie moge zastosowac do tej bazy
            //from p in dataContext.Purchases
         //       group p.Price by p.Date.Year 
         //           into salesByYearselect 
        //    new {             Year       = salesByYear.Key,             
        //        TotalValue = salesByYear.Sum()           };

            MethodInfo[] methods = typeof(string).GetMethods(); 
            PropertyInfo[] props = typeof(string).GetProperties();
            IEnumerable<MemberInfo> both = methods.Concat<MemberInfo>(props);
            foreach (var name in both)
            {
                Console.WriteLine(name);
            }

            //filtering first
            var methods2 = typeof(string).GetMethods().Where(m => !m.IsSpecialName); 
            var props2 = typeof(string).GetProperties(); 
            var both2 = methods.Concat<MemberInfo>(props);
            foreach (var name in both2)
            {
                Console.WriteLine(name);
            }

            //conversion methods -------------------------------------
            ArrayList classicList = new ArrayList();          // in System.Collections
            classicList.AddRange ( new int[] { 3, 4, 5 } );
            IEnumerable<int> sequence1 = classicList.Cast<int>();

            DateTime offender = DateTime.Now; 
            classicList.Add(offender);

            IEnumerable<int>  sequence2 = classicList.OfType<int>(), // OK - ignores offending DateTime 
                sequence3 = classicList.Cast<int>();   // Throws exception


            int[] numbers4 = { 1, 2, 3, 4, 5 }; 
            int first = numbers4.First();                      // 1
            int last       = numbers4.Last();                       // 5
            int firstEven  = numbers4.First  (n => n % 2 == 0);     // 2
            int lastEven   = numbers4.Last   (n => n % 2 == 0);     // 4

            int firstBigError = numbers.First(n => n > 10);   // Exception while enumerate
            int firstBigNumber = numbers.FirstOrDefault (n => n > 10);   // 0
            
            //projecting  linq to sql query to xml (only Linq not EF)
         //   XElement queryFromDataToXml = new XElement("customers", from c in context.Customers
       //                                                select new XElement("customer",
       //                                                    new XAttribute("id", c.Id),
       //                                                    new XElement("firstname", c.Name,
      //                                                         new XComment("nice name"))
      //                                                     ));
      //      Console.WriteLine(queryFromDataToXml);


        }
    }
}
