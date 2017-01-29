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
using Newtonsoft.Json;
using System.Net.NetworkInformation;
using System.Net;

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
            load.Click += Load_Click;
        }

        private void Load_Click(object sender, RoutedEventArgs e)
        {
            Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Documents\\Kaldersvell Connect\\Connections\\");
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Documents\\Kaldersvell Connect\\Connections\\";
            var result = fileDialog.ShowDialog();
            string file = "";
            switch (result)
            {
                case System.Windows.Forms.DialogResult.OK:
                    file = fileDialog.FileName;
                    break;
                case System.Windows.Forms.DialogResult.Cancel:
                default:
                    break;
            }
            if(file != "")
            {
                string input = File.ReadAllText(file);
                Connection c = JsonConvert.DeserializeObject<Connection>(input);
                Ping pingSender = new Ping();
                IPAddress address = IPAddress.Parse(c.IP);
                PingReply reply = pingSender.Send(address);

                if (reply.Status == IPStatus.Success)
                {
                    ConnectionView newConnectionView = new ConnectionView(c);
                    newConnectionView.ShowDialog();
                }
                
            }
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            AddConnection newConnection = new AddConnection();
            newConnection.ShowDialog();
        }
    }
}
