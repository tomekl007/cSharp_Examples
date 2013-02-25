using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Data.Linq;
using System.Data.Linq.Mapping;            // in System.Data.Linq.dllusing System.Data.Linq.Mapping;
//using System.Data.Sql;

namespace linqToSql
{
    class Program
    {
        //The [Table] attribute, in the System.Data.Linq.Mapping
        // namespace, tells L2S thatan object of this type
        //represents a row in a database table
        [Table(Name = "Customer")]
        public class Customer 
        { 
            [Column(IsPrimaryKey = true)] 
            public int ID; 
            [Column] 
            public string Name;
           
        }

        [Table(Name = "Przyklad")]
        public class Przyklad
        {
            [Column(IsPrimaryKey = true)]
            public int ID;
            [Column]
            public string Imie;
            [Column]
            public string Nazwisko;
        }



       [Table(Name = "Purchase")]
        public class Purchase
        {
            [Column(IsPrimaryKey = true)]
            public int ID;
            [Column]
            public int customerId;
           // [Column] problem with float
           // public float price;
            [Column]
            public String description;
        }
        class Test 
        { 
            static void Main() 
            { 
                
            /*    create table Customer(  ID int not null primary key,  
                    Name varchar(30))insert Customer values (1, 'Tom')
                
                insert Customer values (2, 'Dick')
                insert Customer values (3, 'Harry')
                insert Customer values (4, 'Mary')
                insert Customer values (5, 'Jay')*/



                DataContext dataContext = new DataContext("Customer.sdf"); 
                Table<Customer> customers = dataContext.GetTable<Customer>();
                
              
                IQueryable<string> query = from c in customers 
                                           where c.Name.Contains("a") 
                                           orderby c.Name.Length 
                                           select c.Name.ToUpper(); 
                
                foreach (string name in query) 
                    Console.WriteLine(name);

            //    Table<Przyklad> przyklady = dataContext.GetTable<Przyklad>();
          //     IQueryable<string> query2 = from p in przyklady
                                 //          select p.Nazwisko;
                
            //    foreach (string name in query2)
           //         Console.WriteLine(name);

                //combining interpreted and local queries
               

            IEnumerable<string> q = customers  
                .Select (c => c.Name.ToUpper())  
                .OrderBy (n => n)  
                .Pair()                         // Local from this point on.  
                .Select ((n, i) => "Pair " + i.ToString() + " = " + n);
            
            foreach (string element in q) Console.WriteLine (element);

                //using regex
          //  Regex wordCounter = new Regex(@"\b(\w|[-'])+\b"); 
            //    var queryReg = customers
            //        .Where(c => c.Name == "Marry" 
            //            && wordCounter.Matches(c.Name).Count < 100);
            //    //sql server does not support reqex so it trow exception
            //    foreach (Customer element in queryReg) Console.WriteLine(element);
                
                //to solve it
            Regex wordCounter = new Regex(@"\b(\w|[-'])+\b");
                //first retrive data from sql 
                IEnumerable<Customer> sqlQuery = customers
                    .Where(c => c.Name == "Marry"); 
                
                //then filter it locally
                IEnumerable<Customer> localQuery = sqlQuery
                     .Where(c=> wordCounter.Matches(c.Name).Count < 100);

                foreach (Customer element in localQuery) Console.WriteLine(element.Name);

                //change data in database
                Customer cust = customers
                    .OrderBy(c => c.Name)
                    .First(); 
                cust.Name = "Updated Name";
                dataContext.SubmitChanges();

                IEnumerable<Customer> allQuery = customers
                                                .Select(n => n);
                     

                foreach (Customer element in allQuery) Console.WriteLine(element.Name);

                Customer custo = customers.Single(c => c.ID == 2);
                Console.WriteLine("cust with id 2 = "  + custo.Name);

                //object tracking

              //  Customer cust1 = customers.OrderBy(n => n.Name).First();

                Table<Purchase> purchases = dataContext.GetTable<Purchase>();
                IQueryable<Purchase> allP = purchases
                                            .Select(n => n);

                foreach (Purchase element in allP) Console.WriteLine(element.description);

                Purchase p1 = purchases.Single(p => p.ID == 1);
                Customer forPurchaseP1 = customers.Single(c => c.ID == p1.customerId);

                Console.WriteLine("customer for purachase p1  = " + forPurchaseP1.Name);
                
                
                
                
                

            } 

            
        }

        
    }
    //extension method to pair up string in collection
    public static class extentionMethod
    {
        public static IEnumerable<string> Pair(this IEnumerable<string> source)
        {
            Console.WriteLine("pair method\n" + source);

            string firstHalf = null;
            foreach (string element in source)
                if (firstHalf == null)
                {
                    
                    firstHalf = element;
                    Console.WriteLine("first half =  "  + firstHalf);
                }
                else
                {
                    yield return firstHalf + ", " + element;
                    firstHalf = null;
                }
        }

        //visual do it for me
        //typed context
        //class NutshellContext : DataContext    // For LINQ to SQL
       // {  public Table<Customer> Customers  
      //  {    get { return GetTable<Customer>(); }  
      //  }  // ... and so on, for each table in the database
      //  } 
    }

}
