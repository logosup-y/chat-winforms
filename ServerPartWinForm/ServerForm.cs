using System.Net;

namespace ServerPartWinForm
{
    public partial class ServerForm : Form
    {
        private ChatServer? _server;
        private Task? _serverTask;

        public ServerForm()
        {
            InitializeComponent();
            disconnectButton.Enabled = false;
            connectionPortTextBox.Text = "Port";
            connectionPortTextBox.ForeColor = Color.Gray;
        }

        private async void Connect_Click(object sender, EventArgs e)
        {
            var (isInputValid, port) = ValidateInput();
            if (!isInputValid)
            {
                return;
            }

            ConnectButton.Enabled = false;
            disconnectButton.Enabled = true;
            connectionPortTextBox.Enabled = false;

            _server = new ChatServer(IPAddress.Any, port);
            _server.OnLogMessage += Server_OnLogMessage;


            _serverTask = _server.StartAsync_ListenClients();

            labelStatus.Text = "Server is running...";

            await _serverTask;
        }

        private void DisconnectButton_Click(object sender, EventArgs e)
        {
            ConnectButton.Enabled = true;
            disconnectButton.Enabled = false;
            connectionPortTextBox.Enabled = true;

            _server?.Stop();
            _serverTask = null;

            labelStatus.Text = "Server is stopped.";
            LogMessage($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Server is stopped.");
        }

        private void LogMessage(string message)
        {
            if (richTextBoxLog.InvokeRequired)
            {
                richTextBoxLog.Invoke(new Action<string>(LogMessage), message);
            }
            else
            {
                richTextBoxLog.AppendText($"{$"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}"}\n");
            }
        }

        private void Server_OnLogMessage(object sender, string message)
        {
            LogMessage(message);
        }

        private void ConnectionPort_Click(object sender, EventArgs e)
        {
            if (connectionPortTextBox.Text == "Port")
            {
                connectionPortTextBox.Text = "";
                connectionPortTextBox.ForeColor = Color.Black;
            }
        }

        private void ConnectionPort_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(connectionPortTextBox.Text))
            {
                connectionPortTextBox.Text = "Port";
                connectionPortTextBox.ForeColor = Color.Gray;
            }
        }

        private void ConnectionPort_Enter(object sender, EventArgs e)
        {
            if (connectionPortTextBox.Text == "Port")
            {
                connectionPortTextBox.Text = "";
                connectionPortTextBox.ForeColor = Color.Black;
            }
        }

        private (bool, ushort) ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(connectionPortTextBox.Text) || connectionPortTextBox.Text == "Input your username")
            {
                MessageBox.Show("Please enter a username before connecting.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return (false, 0);
            }

            if (string.IsNullOrWhiteSpace(connectionPortTextBox.Text) || connectionPortTextBox.Text == "Port")
            {
                MessageBox.Show("Please enter a port number before connecting.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return (false, 0);
            }

            string connectionPortText = connectionPortTextBox.Text;
            ushort port;
            bool conversionSuccessful = ushort.TryParse(connectionPortText, out port);

            if (!conversionSuccessful || port == 0)
            {
                MessageBox.Show("Invalid port number. Please enter a value between 1 and 65,535.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return (false, 0);
            }

            return (true, port);
        }
    }
}