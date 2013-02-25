using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading.Tasks;

namespace UIsynchAsynch
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        Button _button = new Button { Content = "Go" };
        TextBlock _results = new TextBlock();

        public MainWindow()
        {
            InitializeComponent();
            var panel = new StackPanel();
            panel.Children.Add(_button);
            panel.Children.Add(_results);
            Content = panel;
            _button.Click += (sender, args) => DisplayPrimeCountsFrom(1);
        }

        void Go()
        {
            for (int i = 1; i < 5; i++)
                _results.Text += GetPrimesCountAsync(i * 1000000, 1000000).ContinueWith( awaiter =>
                   Console.WriteLine(awaiter.Result) ) +
                    " primes between " + (i * 1000000) + " and " + ((i + 1) * 1000000 - 1) + 
                    Environment.NewLine;
        }

        void DisplayPrimeCountsFrom(int i)		// This is starting to get awkward!
        {
            //  var awaiter = GetPrimesCountAsync(i * 1000000 + 2, 1000000).GetAwaiter();
            //  awaiter.OnCompleted

            GetPrimesCountAsync(i * 1000000 + 2, 1000000).ContinueWith
              (awaiter =>
              {
                  _results.Text+=awaiter.Result + " primes between " +
                      (i * 1000000) + " and " + ((i + 1) * 1000000 - 1);
                  if (i++ < 5) DisplayPrimeCountsFrom(i);
                  //else Console.WriteLine("Done");
                  else _results.Text = "done";
              });
        }


       Task<int> GetPrimesCountAsync(int start, int count)
        {
          return Task.Factory.StartNew(()=> ParallelEnumerable.Range(start, count).Count(n =>
            Enumerable.Range(2, (int)Math.Sqrt(n) - 1).All(i => n % i > 0)) );
        }
    }
}
