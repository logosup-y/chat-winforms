using System.Net;
using System.Text;

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
            connectionPortTextBox.Text = "Port";
            connectionPortTextBox.ForeColor = Color.Gray;

        }

        private async void connectButton_Click(object sender, EventArgs e)
        {
            var (isInputValid, port) = ValidateInput();
            if (!isInputValid)
            {
                return;
            }

            connectButton.Enabled = false;
            usernameTextBox.Enabled = false;
            connectionPortTextBox.Enabled = false;
            
            string username = usernameTextBox.Text;
            _client = new ChatClient(username);

            bool isConnected = false;
            bool isUsernameTaken = false;

            try
            {
                isConnected = await _client.ConnectAsync(IPAddress.Loopback, port);
            }
            catch
            {
                MessageBox.Show($"Error connecting to server", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                connectButton.Enabled = true;
                usernameTextBox.Enabled = true;
                connectionPortTextBox.Enabled = true;
                return;
            }

            if (isConnected)
            {
                DisconnectButton.Enabled = true;
                connectionStatus.Text = $"Connected as \"{username}\"";
                sendButton.Enabled = true;

                _client.MessageReceived += OnMessageReceived;                
                _client.ServerDisconnected += OnServerDisconnected;

                messagesRichTextBox.Invoke(new Action(() => messagesRichTextBox.AppendText($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] You are connected\n")));
            }
            else
            {
                MessageBox.Show("Username is already taken. Please choose another one.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                connectButton.Enabled = true;
                usernameTextBox.Enabled = true;
                connectionPortTextBox.Enabled = true;
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
            connectionPortTextBox.Enabled = true;

            _client?.TcpClient.Close();            
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
                MessageBox.Show($"Error sending message", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

            messagesRichTextBox.Invoke(new Action(() => messagesRichTextBox.AppendText($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] You've disconnected\n")));
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

        private void ConnectionPortTextBox_Click(object sender, EventArgs e)
        {
            if (connectionPortTextBox.Text == "Port")
            {
                connectionPortTextBox.Text = "";
                connectionPortTextBox.ForeColor = Color.Black;
            }
        }

        private void ConnectionPortTextBox_Enter(object sender, EventArgs e)
        {
            if (connectionPortTextBox.Text == "Port")
            {
                connectionPortTextBox.Text = "";
                connectionPortTextBox.ForeColor = Color.Black;
            }
        }

        private void ConnectionPortTextBox_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(connectionPortTextBox.Text))
            {
                connectionPortTextBox.Text = "Port";
                connectionPortTextBox.ForeColor = Color.Gray;
            }
        }

        private (bool, ushort) ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(usernameTextBox.Text) || usernameTextBox.Text == "Input your username")
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