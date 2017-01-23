using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Security;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Kaldersvell_Connect_Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            add.Click += Add_Click;
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            AddConnection newConnection = new AddConnection();
            newConnection.ShowDialog();
            string name = "";
            name.Replace(' ', '_');

        }
    }
}
