using System.Net;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace ClientPartWinForm
{
    public partial class ClientForm : Form
    {
        private ChatClient _client;
        public ClientForm()
        {
            InitializeComponent();
            disconnectButton.Enabled = false;
        }

        private async void connectButton_Click(object sender, EventArgs e)
        {
            connectButton.Enabled = false;
            userNameTextBox.Enabled = false;

            string username = userNameTextBox.Text;
            _client = new ChatClient(username);

            try
            {
                await _client.ConnectAsync(IPAddress.Loopback, 5000);
                disconnectButton.Enabled = true;
                connectionStatus.Text = $"Connected as {username}";
                _client.MessageReceived += OnMessageReceived;               
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error connecting to server: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                connectButton.Enabled = true;
                userNameTextBox.Enabled = true;
            }
        }

        private void OnMessageReceived(object sender, string message)
        {
            messagesRichTextBox.Invoke(new Action(() => messagesRichTextBox.AppendText($"{message}\n")));
        }

        private void disconnectButton_Click(object sender, EventArgs e)
        {
            disconnectButton.Enabled = false;
            connectButton.Enabled = true;
            userNameTextBox.Enabled = true;

            _client.TcpClient.Close();
            connectionStatus.Text = "Disconnected";
        }

        private async void sendButton_Click(object sender, EventArgs e)
        {

            string message = messageTextBox.Text.Trim();

            // Check if the message is empty or contains only whitespace characters
            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            try
            {
                await _client.SendMessageAsync(message);
                messageTextBox.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error sending message: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }                    
        }
    }
}