using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Data.Entity;
//using System.Data.Linq.Mapping;
using System.Data.Objects.DataClasses;
using System.Data.Objects;




namespace EntityFreamwork_Classes
{
   
    

    class Program
    {
        // You'll need to reference System.Data.Entity.dll
        [EdmEntityType(NamespaceName = "NutshellModel", Name = "Customer")]
        public partial class Customer
        {
            [EdmScalarPropertyAttribute(EntityKeyProperty = true, IsNullable = false)]
            public int ID { get; set; }
            [EdmScalarProperty(EntityKeyProperty = false, IsNullable = false)]
            public string Name { get; set; }
        }
     

        static void Main(string[] args)
        {

            var context = new ObjectContext("name=newEntities");
            context.DefaultContainerName = "newEntities"; 
            ObjectSet<Customer> customers = context.CreateObjectSet<Customer>(); 
            Console.WriteLine(customers.Count());              // # of rows in table.
            Customer cust = customers.Single(c => c.ID == 2);  // Retrieves Customer with ID of 2.
            

        }
    }
}
