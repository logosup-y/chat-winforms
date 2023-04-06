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
            sendButton.Enabled = false;
            disconnectButton.Enabled = false;
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

            bool connected = false;
            try
            {
                connected = await _client.ConnectAsync(IPAddress.Loopback, 5000);
            }
            catch
            {
                MessageBox.Show($"Error connecting to server", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            if (connected)
            {
                disconnectButton.Enabled = true;
                connectionStatus.Text = $"Connected as \"{username}\"";
                sendButton.Enabled = true;
                _client.MessageReceived += OnMessageReceived;
                _client.UsernameAlreadyTaken += OnUsernameAlreadyTaken;
                _client.ServerDisconnected += OnServerDisconnected;
            }
            else
            {
                MessageBox.Show("Username is already taken. Please choose another one.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                connectButton.Enabled = true;
                usernameTextBox.Enabled = true;
            }
        }

        /* private async void connectButton_Click(object sender, EventArgs e)
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

             try
             {
                 await _client.ConnectAsync(IPAddress.Loopback, 5000);
                 disconnectButton.Enabled = true;
                 connectionStatus.Text = $"Connected as \"{username}\"";
                 sendButton.Enabled = true;                
                 _client.MessageReceived += OnMessageReceived;
                 _client.UsernameAlreadyTaken += OnUsernameAlreadyTaken;
                 _client.ServerDisconnected += OnServerDisconnected;
             }
             catch
             {
                 MessageBox.Show($"Error connecting to server", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                 connectButton.Enabled = true;
                 usernameTextBox.Enabled = true;
             }
         }*/

        private void OnMessageReceived(object sender, string message)
        {
            messagesRichTextBox.Invoke(new Action(() => messagesRichTextBox.AppendText($"{message}\n")));
        }

        private void disconnectButton_Click(object sender, EventArgs e)
        {
            disconnectButton.Enabled = false;
            connectButton.Enabled = true;
            sendButton.Enabled = false;
            usernameTextBox.Enabled = true;

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
            catch
            {
                MessageBox.Show($"Error sending message: Your are not connected", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OnServerDisconnected(object sender, EventArgs e)
        {
            disconnectButton.Invoke(new Action(() =>
            {
                disconnectButton.Enabled = false;
                connectButton.Enabled = true;
                sendButton.Enabled = false;
                usernameTextBox.Enabled = true;
                connectionStatus.Text = "Disconnected";
            }));

            messagesRichTextBox.Invoke(new Action(() => messagesRichTextBox.AppendText($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}] Connection with server lost\n")));            
        }
        
        private void usernameTextBox_Click(object sender, EventArgs e)
        {
            if (usernameTextBox.Text == "Input your username")
            {
                usernameTextBox.Text = "";
                usernameTextBox.ForeColor = Color.Black;
            }
        }

        private void usernameTextBox_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(usernameTextBox.Text))
            {
                usernameTextBox.Text = "Input your username";
                usernameTextBox.ForeColor = Color.Gray;
            }
        }

        private void usernameTextBox_Enter(object sender, EventArgs e)
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