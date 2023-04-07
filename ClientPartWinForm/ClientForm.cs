using System.Net;
using System.Text;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace ClientPartWinForm
{
    public partial class ClientForm : Form
    {
        private ChatClient? _client;
        public ClientForm()
        {
            InitializeComponent();
            sendButton.Enabled = false;
            DisconnectButton.Enabled = false;
            usernameTextBox.Text = "Input your username";
            usernameTextBox.ForeColor = Color.Gray;
        }

        private async void connectButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(usernameTextBox.Text) || usernameTextBox.Text == "Input your username")
            {
                MessageBox.Show("Please enter a username before connecting.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            connectButton.Enabled = false;
            usernameTextBox.Enabled = false;

            string username = usernameTextBox.Text;
            _client = new ChatClient(username);

            bool isConnected = false;

            try
            {
                isConnected = await _client.ConnectAsync(IPAddress.Loopback, 5000);
            }
            catch
            {
                MessageBox.Show($"Error connecting to server", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            if (isConnected)
            {
                DisconnectButton.Enabled = true;
                connectionStatus.Text = $"Connected as \"{username}\"";
                sendButton.Enabled = true;

                _client.MessageReceived += OnMessageReceived;
                _client.UsernameAlreadyTaken += OnUsernameAlreadyTaken;
                _client.ServerDisconnected += OnServerDisconnected;

                messagesRichTextBox.Invoke(new Action(() => messagesRichTextBox.AppendText($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] You are connected\n")));
            }
            else
            {
                MessageBox.Show("Username is already taken. Please choose another one.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                connectButton.Enabled = true;
                usernameTextBox.Enabled = true;
            }
        }

        private void OnMessageReceived(object sender, string message)
        {
            messagesRichTextBox.Invoke(new Action(() => messagesRichTextBox.AppendText($"{message}\n")));
        }

        private void DisconnectButton_Click(object sender, EventArgs e)
        {
            DisconnectButton.Enabled = false;
            connectButton.Enabled = true;
            sendButton.Enabled = false;
            usernameTextBox.Enabled = true;

            _client?.TcpClient.Close();
            connectionStatus.Text = "Disconnected";
        }

        private async void SendButton_Click(object sender, EventArgs e)
        {
            var message = messageTextBox.Text.Split("\n\r".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            StringBuilder splitMessage = new StringBuilder();

            foreach (var line in message)
            {
                splitMessage.Append($"{line} ");
            }

            if (string.IsNullOrEmpty(splitMessage.ToString()))
            {
                return;
            }

            try
            {
                await _client.SendMessageAsync(splitMessage.ToString());
                messageTextBox.Clear();
            }
            catch
            {
                MessageBox.Show($"Error sending message: Your are not connected", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OnServerDisconnected(object sender, EventArgs e)
        {
            DisconnectButton.Invoke(new Action(() =>
            {
                DisconnectButton.Enabled = false;
                connectButton.Enabled = true;
                sendButton.Enabled = false;
                usernameTextBox.Enabled = true;
                connectionStatus.Text = "Disconnected";
            }));

            messagesRichTextBox.Invoke(new Action(() => messagesRichTextBox.AppendText($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}] Connection with server lost\n")));
        }

        private void UsernameTextBox_Click(object sender, EventArgs e)
        {
            if (usernameTextBox.Text == "Input your username")
            {
                usernameTextBox.Text = "";
                usernameTextBox.ForeColor = Color.Black;
            }
        }

        private void UsernameTextBox_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(usernameTextBox.Text))
            {
                usernameTextBox.Text = "Input your username";
                usernameTextBox.ForeColor = Color.Gray;
            }
        }

        private void UsernameTextBox_Enter(object sender, EventArgs e)
        {
            if (usernameTextBox.Text == "Input your username")
            {
                usernameTextBox.Text = "";
                usernameTextBox.ForeColor = Color.Black;
            }
        }

        private void OnUsernameAlreadyTaken(object sender, EventArgs e)
        {
            MessageBox.Show("Username is already taken. Please choose another one.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            connectButton.Invoke(new Action(() =>
            {
                connectButton.Enabled = true;
                usernameTextBox.Enabled = true;
            }));
        }
    }
}