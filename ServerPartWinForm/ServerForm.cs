using System;
using System.Net;
using System.Windows.Forms;

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
        }

        private async void Connect_Click(object sender, EventArgs e)
        {
            ConnectButton.Enabled = false;
            disconnectButton.Enabled = true;

            _server = new ChatServer(IPAddress.Any, 5000);
            _server.OnLogMessage += Server_OnLogMessage;


            _serverTask = _server.StartAsync_ListenClients();

            labelStatus.Text = "Server is running...";

            await _serverTask;
        }

        private void DisconnectButton_Click(object sender, EventArgs e)
        {
            ConnectButton.Enabled = true;
            disconnectButton.Enabled = false;

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
    }
}