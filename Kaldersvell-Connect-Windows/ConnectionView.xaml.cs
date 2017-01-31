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
using Renci.SshNet;
using System.Text.RegularExpressions;

namespace Kaldersvell_Connect_Windows
{
    /// <summary>
    /// Interaction logic for ConnectionView.xaml
    /// </summary>
    public partial class ConnectionView : Window
    {
        SshClient ssh;
        ShellStream shellStream;
        Connection connection;
        IDictionary<Renci.SshNet.Common.TerminalModes, uint> termkvp;
        public ConnectionView(Connection currentConnection)
        {
            InitializeComponent();
            this.Title = currentConnection.Name;
            this.Closing += ConnectionView_Closing;
            connection = currentConnection;
            CreateConnection(connection);
            runPython2.Click += RunPython2_Click;
            runPython3.Click += RunPython3_Click;
            PIP2.Click += PIP2_Click;
            PIP3.Click += PIP3_Click;
            APT.Click += APT_Click;

        }
        private void APT_Click(object sender, RoutedEventArgs e)
        {
            string package = Microsoft.VisualBasic.Interaction.InputBox("Enter package name:", "apt-get install", "", 200, 200);
            runSudoCommand("sudo apt-get -y install " + package);
        }
        private void PIP3_Click(object sender, RoutedEventArgs e)
        {
            string package = Microsoft.VisualBasic.Interaction.InputBox("Enter package name:", "pip3 install", "", 200, 200);
            runSudoCommand("sudo pip3 install " + package);
        }
        private void PIP2_Click(object sender, RoutedEventArgs e)
        {
            string package = Microsoft.VisualBasic.Interaction.InputBox("Enter package name:", "pip install", "", 200, 200);
            runSudoCommand("sudo pip install " + package);
        }
        private void RunPython3_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Documents\\";
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
            if (file != "")
            {
                using (var scp = new ScpClient(connection.IP, 22, connection.Username, connection.Password))
                {
                    scp.Connect();
                    FileInfo from = new FileInfo(file);
                    scp.Upload(from, "~");

                    scp.Disconnect();
                    runSudoCommand("sudo python3 " + System.IO.Path.GetFileName(file));
                }
            }
        }
        private void RunPython2_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Documents\\";
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
            if (file != "")
            {
                using (var scp = new ScpClient(connection.IP, 22, connection.Username, connection.Password))
                {
                    scp.Connect();
                    FileInfo from = new FileInfo(file);
                    scp.Upload(from, "~");

                    scp.Disconnect();
                    runSudoCommand("sudo python " + System.IO.Path.GetFileName(file));
                }
            }
        }
        private void ConnectionView_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            runSudoCommand("sudo shutdown -h now");
            ssh.Disconnect();
        }
        private void CreateConnection(Connection c)
        {
            try
            {
                //ssh = new SshClient(CreateConnectionInfo(c));
                ssh = new SshClient(c.IP, 22, c.Username, c.Password);
                ssh.Connect();

                termkvp = new Dictionary<Renci.SshNet.Common.TerminalModes, uint>();
                termkvp.Add(Renci.SshNet.Common.TerminalModes.ECHO, 53);

                shellStream = ssh.CreateShellStream("xterm", 80, 24, 800, 600, 1024);

                runCommand("cd ~");
            } catch
            {
                System.Windows.MessageBox.Show("Error connecting to device.", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
            }
        }
        private void runCommand(string s)
        {
            ssh.RunCommand(s);
        }
        private void runSudoCommand(string s)
        {
            var output = shellStream.Expect(new Regex(@"[$>]"));

            shellStream.WriteLine(s);
            output = shellStream.Expect(new Regex(@"([$#>:])"));
            shellStream.WriteLine(connection.Password);
        }

    }
}
