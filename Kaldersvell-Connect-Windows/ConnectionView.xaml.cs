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
using Chilkat;
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
        StreamReader reader;
        StreamWriter writer;
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
            if (!File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Documents\\Kaldersvell Connect\\Keys\\" + "DefaultOpenSSHPrivateKey.pem"))
            {
                CreateKey();
            }
            if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Documents\\Kaldersvell Connect\\Keys\\" + "DefaultOpenSSHPrivateKey.pem"))
            {
                //ssh = new SshClient(CreateConnectionInfo(c));
                ssh = new SshClient(c.IP, 22, c.Username, c.Password);
                ssh.Connect();

                termkvp = new Dictionary<Renci.SshNet.Common.TerminalModes, uint>();
                termkvp.Add(Renci.SshNet.Common.TerminalModes.ECHO, 53);

                shellStream = ssh.CreateShellStream("xterm", 80, 24, 800, 600, 1024);

                runCommand("cd ~");
                //reader = new StreamReader(shellStream);
                //writer = new StreamWriter(shellStream);
            }
            else
            {
                Console.WriteLine("Error creating connection");
                return;
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
        private ConnectionInfo CreateConnectionInfo(Connection d)
        {
            string privateKeyFilePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Documents\\Kaldersvell Connect\\Keys\\" + "DefaultOpenSSHPrivateKey.pem";
            ConnectionInfo connectionInfo;
            using (var stream = new FileStream(privateKeyFilePath, FileMode.Open, System.IO.FileAccess.Read))
            {
                var privateKeyFile = new PrivateKeyFile(stream);
                AuthenticationMethod authenticationMethod =
                    new PrivateKeyAuthenticationMethod(d.Username, privateKeyFile);

                connectionInfo = new ConnectionInfo(
                    d.IP,
                    d.Username,
                    authenticationMethod);
            }

            return connectionInfo;
        }
        private void CreateKey()
        {
            Chilkat.SshKey key = new Chilkat.SshKey();

            bool success;

            int numBits;
            int exponent;

            //  numBits may range from 384 to 4096.  Typical values are
            //  1024 or 2048.  (must be a multiple of 64)
            //  A good choice for the exponent is 65537.  Chilkat recommends
            //  always using this value.
            numBits = 2048;
            exponent = 65537;
            success = key.GenerateRsaKey(numBits, exponent);
            if (success != true)
            {
                Console.WriteLine("Bad params passed to RSA key generation method.");
                return;
            }

            //  Note: Generating a public/private key pair is CPU intensive
            //  and may take a short amount of time (more than few seconds,
            //  but less than a minute).

            string exportedKey;
            bool exportEncrypted;

            //  Export the RSA private key to OpenSSH, PuTTY, and XML and save.
            exportEncrypted = false;
            exportedKey = key.ToOpenSshPrivateKey(exportEncrypted);
            //  Chilkat provides a SaveText method for convenience...
            Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Documents\\Kaldersvell Connect\\Keys\\");
            success = key.SaveText(exportedKey, Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Documents\\Kaldersvell Connect\\Keys\\" + "DefaultOpenSSHPrivateKey.pem");

            //  ----------------------------------------------------
            //  Now for the public key....
            //  ----------------------------------------------------

            //  The Secure Shell (SSH) Public Key File Format
            //  is documented in RFC 4716.
            exportedKey = key.ToRfc4716PublicKey();
            success = key.SaveText(exportedKey, Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Documents\\Kaldersvell Connect\\Keys\\" + "DefaultSSHPublicKey.pub");

            //  OpenSSH has a separate public-key file format, which
            //  is also supported by Chilkat SshKey:
            exportedKey = key.ToOpenSshPublicKey();
            success = key.SaveText(exportedKey, Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Documents\\Kaldersvell Connect\\Keys\\" + "DefaultOpenSSHPublicKey.pub");
        }
    }
}
