using System.Net;
using System.Text;
using System.Windows.Forms;
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
            if (string.IsNullOrWhiteSpace(userNameTextBox.Text))
            {
                MessageBox.Show("Please enter a username before connecting.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

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
                _client.ServerDisconnected += OnServerDisconnected;
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
            var message = messageTextBox.Text.Split("\n\r".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            StringBuilder splitMessage = new StringBuilder();

            foreach (var line in message)
            {
                splitMessage.Append($"{line} ");
            }

            // Check if the message is empty or contains only whitespace characters
            if (string.IsNullOrEmpty(splitMessage.ToString()))
            {
                return;
            }

            try
            {
                await _client.SendMessageAsync(splitMessage.ToString());
                messageTextBox.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error sending message: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


        }

        private void OnServerDisconnected(object sender, EventArgs e)
        {
            disconnectButton.Invoke(new Action(() =>
            {
                disconnectButton.Enabled = false;
                connectButton.Enabled = true;
                userNameTextBox.Enabled = true;
                connectionStatus.Text = "Disconnected";
            }));

            messagesRichTextBox.Invoke(new Action(() => messagesRichTextBox.AppendText("Connection with server lost\n")));
        }
    }
}