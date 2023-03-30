using System;
using System.Net;
using System.Windows.Forms;

namespace ServerPartWinForm
{
    public partial class Form1 : Form
    {
        private ChatServer _server;
        private Task _serverTask;

        public Form1()
        {
            InitializeComponent();
            DisconnectButton.Enabled = false;
        }

        private async void Connect_Click(object sender, EventArgs e)
        {
            ConnectButton.Enabled = false;
            DisconnectButton.Enabled = true;

            _server = new ChatServer(this, IPAddress.Any, 5000);
            _serverTask = _server.StartAsync();

            labelStatus.Text = "Server is running...";            

            await _serverTask;
        }

        private void DisconnectButton_Click(object sender, EventArgs e)
        {
            ConnectButton.Enabled = true;
            DisconnectButton.Enabled = false;
                        
            _server.Stop();
            _serverTask = null;

            labelStatus.Text = "Server is stopped.";
            richTextBoxLog.AppendText("Server stopped.\n");
        }

        public void LogMessage(string message)
        {
            if (richTextBoxLog.InvokeRequired)
            {
                richTextBoxLog.Invoke(new Action<string>(LogMessage), message);
            }
            else
            {
                richTextBoxLog.AppendText($"{message}\n");
            }
        }
    }
}