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
using System.Windows.Shapes;
using System.IO;
using System.Security;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace Kaldersvell_Connect_Windows
{
    /// <summary>
    /// Interaction logic for AddConnection.xaml
    /// </summary>
    public partial class AddConnection : Window
    {
        public AddConnection()
        {
            InitializeComponent();
            AddSubmit.Click += AddSubmit_Click;
        }

        private void AddSubmit_Click(object sender, RoutedEventArgs e)
        {
            //Create Connection object
            Connection newConnection = new Connection(ConnectionName.Text, ConnectionIP.Text, ConnectionUsername.Text, ConnectionPassword.Password);
            //Create a JSON string from the Connection object
            string output = JsonConvert.SerializeObject(newConnection);
            //If the folder doesn't exist, make it
            System.IO.Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Documents\\Kaldersvell Connect\\Connections\\");
            //Write the JSON to the file
            File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Documents\\Kaldersvell Connect\\Connections\\" + ConnectionName.Text.Replace(' ', '_') + ".kcn", output);
            //Close Window
            this.Close();
        }
    }
}
