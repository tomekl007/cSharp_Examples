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
using System.ComponentModel;
using System.Threading;


namespace SynchronizationContextEx
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        TextBox txtMessage;
        SynchronizationContext _uiSyncContext;

        void Main()
        {
           // this.CreateSynchronizationContext();
            new MainWindow().ShowDialog();
        }

        public MainWindow()
        {
            InitializeComponent();
            InitComponent();
            // Capture the synchronization context for the current UI thread:
            _uiSyncContext = SynchronizationContext.Current;
            new Thread(Work).Start();
        }

        void Work()
        {
            Thread.Sleep(5000);           // Simulate time-consuming task
            UpdateMessage("The answer");
        }

        void UpdateMessage(string message)
        {
            // Marshal the delegate to the UI thread:
            _uiSyncContext.Post(_ => txtMessage.Text = message, null);
        }

        void InitComponent()
        {
            SizeToContent = SizeToContent.WidthAndHeight;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            Content = txtMessage = new TextBox
            { Width = 250, Margin = new Thickness(10), Text = "Ready" };
        }


    }
}
