using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


// The .NET Framework defines a standard pattern for writing events. The pattern provides
// consistency across both Framework and user code.
namespace StandarEventPattern
{
    //EventArgs is a base class for con-veying information for
    //an event. In our Stock example, we would subclassEventArgs
    //to convey the old and new prices when a PriceChanged event is fired:
     public class PriceChangedEventArgs : EventArgs
    {
        public readonly decimal LastPrice;
        public readonly decimal NewPrice;

        public PriceChangedEventArgs(decimal lastPrice, decimal newPrice)
        {
            LastPrice = lastPrice; NewPrice = newPrice;
        }
    }

    public class Stock
    {
        string symbol;
        decimal price;

        public Stock(string symbol) { this.symbol = symbol; }

        //The next step is to define an event of the
        //chosen delegate type. Here, we use the generic EventHandler delegate:
        public event EventHandler<PriceChangedEventArgs> PriceChanged;


        //Finally, the pattern requires that you write a protected virtual method
        //that fires the event. The name must match the name of the event, prefixed
        //with the word On, an dthen accept a single EventArgs argument:
        protected virtual void OnPriceChanged(PriceChangedEventArgs e)
        {
            if (PriceChanged != null) PriceChanged(this, e);
        }

        public decimal Price
        {
            get { return price; }
            set
            {
                if (price == value) return;
                decimal oldPrice = price;
                price = value;
                OnPriceChanged(new PriceChangedEventArgs(oldPrice, price));
            }
        }
    }

    class Program
    {
        static void Main()
        {
            Stock stock = new Stock("THPW");
            stock.Price = 27.10M;
            // Register with the PriceChanged event
            stock.PriceChanged += stock_PriceChanged;
            stock.Price = 31.59M;
        }

        static void stock_PriceChanged(object sender, PriceChangedEventArgs e)
        {
            if ((e.NewPrice - e.LastPrice) / e.LastPrice > 0.1M)
                Console.WriteLine("Alert, 10% stock price increase!");
        }
    }
}
