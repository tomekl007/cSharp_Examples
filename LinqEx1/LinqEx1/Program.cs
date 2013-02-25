using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqEx1
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] names = { "Tom", "Dick", "Harry" };
            IEnumerable<String> filteredNames = names.Where(n=> n.Length >= 4);//fluent syntax
            foreach (string s in filteredNames)
                Console.WriteLine(s);

            IEnumerable<String> filteredNames2 = from n in names
                                                 where n.Contains("a")
                                                 select n;//query syntax
            foreach (string s in filteredNames2)
                Console.WriteLine(s);

            //chaining of query
            string[] names2 = {"TOm","Dick","Harry","Marry","jay"};
            IEnumerable<String> query = names2.Where(n => n.Contains("a"))
                                             .OrderBy(n => n.Length)
                                             .Select(n => n.ToUpper());//fluent sytax
            foreach (string s in query)
                Console.WriteLine(s);
            Console.WriteLine();

            IEnumerable<String> query2 = from n in names
                                         where n.Contains("a")
                                         orderby n.Length
                                         select n.ToUpper();//query syntax
            //compiler translates query syntax into fluent syntax

            foreach (string s in query)
                Console.WriteLine(s);

            //subquery 
            //wsystie imiona których długość to długość pierszego w posortowanej listcie według długosci
            IEnumerable<String> subQuery = names2.Where(n => n.Length == names2.OrderBy (n2 =>n2.Length)
                                                                          .Select (n2 => n2.Length).First());//fluent syntax
            foreach (string s in subQuery)
                Console.WriteLine(s);

            IEnumerable<string> outerQuery = from n in names2
                                             where n.Length == 
                                                    (from n2 in names2 orderby n2.Length select n2.Length).First()//query syntax
                                             select n;
            foreach (string s in outerQuery)
                Console.WriteLine(s);

            //same query - written diffrent
            IEnumerable<string> querySame = from n in names2 
                                            where n.Length == names2.OrderBy(n2 => n2.Length).First().Length
                                            select n;

            //using two queries
            int shortest = names2.Min(n => n.Length);
 
            IEnumerable<string> queryOuery2 = from n in names2 
                                              where n.Length == shortest 
                                              select n;
            foreach (string s in queryOuery2)
                Console.WriteLine(s);

            //progresivly query syntax
            IEnumerable<string> queryProg = from n in names
                                            select n.Replace("a", "").Replace("e", "").Replace("i", "")
                                            .Replace("o", "").Replace("u", ""); 
            queryProg = from n in queryProg 
                    where n.Length > 2 
                    orderby n 
                    select n;

            foreach (string s in queryProg)
                Console.WriteLine(s);

            //same in fluent syntax
            IEnumerable<string> queryFluent = names.Select(n => n.Replace("a", "").Replace("e", "")
                                                   .Replace("i", "").Replace("o", "").Replace("u", ""))
                                                   .Where(n => n.Length > 2).OrderBy(n => n);
            foreach (string s in queryFluent)
                Console.WriteLine(s);

            Console.WriteLine();

            //into - restarts the query allowing you to introduce new where,orderby and select clause
            IEnumerable<string> queryInto = from n in names 
                                            select n.Replace("a", "").Replace("e", "").Replace("i", "")
                                                    .Replace("o", "").Replace("u", "") 
                                            into noVowel
                                                  where noVowel.Length > 2 
                                                  orderby noVowel 
                                                  select noVowel;
            foreach (string s in queryInto)
                Console.WriteLine(s);

            //get query to object
           string[] names3 = { "Tom", "Dick", "Harry", "Mary", "Jay" };

            IEnumerable<TempProjectionItem> temp = from n in names3
                                                   select new TempProjectionItem
                                                   {
                                                       Original = n,
                                                       Vowelless = n.Replace("a", "").Replace("e", "").Replace("i", "")
                                                       .Replace("o", "").Replace("u", "")
                                                   };
            //subsequent query of result
            IEnumerable<string> querySub = from item in temp
                                        where item.Vowelless.Length > 2
                                        select item.Original;
            foreach (string s in querySub)
                Console.WriteLine(s);

            //anonymus types

            var query4 = from n in names
                        select new
                            {
                            Original = n,
                            Vowelless = n.Replace ("a", "").Replace ("e", "").Replace ("i", "")
                            .Replace ("o", "").Replace ("u", "")
                            }
                        into temp2
                            where temp2.Vowelless.Length > 2
                            select temp2.Original;
            Console.WriteLine();
            foreach (string s in query4)
                Console.WriteLine(s);

            //let keyword

             IEnumerable<string> queryLet = from n in names2 
                                            let vowelless = n.Replace("a", "").Replace("e", "")
                                            .Replace("i", "").Replace("o", "").Replace("u", "") 
                                            where vowelless.Length > 2 
                                            orderby vowelless 
                                            select n;       // Thanks to let, n is still in scope.
             Console.WriteLine();
             foreach (string s in queryLet)
                 Console.WriteLine(s);


            

        }
        class TempProjectionItem
        {
            public string Original; // Original name
            public string Vowelless; // Vowel-stripped name
        }
    }
}
