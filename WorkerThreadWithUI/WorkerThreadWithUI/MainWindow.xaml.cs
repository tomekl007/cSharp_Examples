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
using System.Threading;

namespace WorkerThreadWithUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        void Main()
        {
            new MainWindow().ShowDialog();
        }

        TextBox txtMessage;
        public MainWindow()
        {
            InitializeComponent();
            initComponent();
            new Thread(Work).Start();
        }

        void Work()
        {
            Thread.Sleep(5000);           // Simulate time-consuming task
            UpdateMessage("The answer");
        }

        void UpdateMessage(string message)
        {
            Action action = () => txtMessage.Text = message;
            Dispatcher.BeginInvoke(action);
        }


        void initComponent()
        {
            SizeToContent = SizeToContent.WidthAndHeight;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            Content = txtMessage = new TextBox
            { Width = 250, Margin = new Thickness(10), Text = "Ready" };
        }

	
    }
}
