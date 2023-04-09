using System.Net.Sockets;
using System.Net;
using System.Text;

namespace ClientPartWinForm
{
    public class ChatClient
    {
        public string Username { get; private set; }
        public TcpClient TcpClient { get; private set; }

        private StreamWriter? _writer;
        private StreamReader? _reader;

        public event EventHandler<string>? MessageReceived;

        public event EventHandler? ServerDisconnected;        

        public ChatClient(string username)
        {
            Username = username;
        }

        public async Task<bool> ConnectAsync(IPAddress serverIpAddress, ushort port)
        {
            TcpClient = new TcpClient();
            await TcpClient.ConnectAsync(serverIpAddress, port);

            NetworkStream stream = TcpClient.GetStream();
            _writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };
            _reader = new StreamReader(stream, Encoding.UTF8);

            // Send the username to the server
            await _writer.WriteLineAsync(Username);

            string? serverResponse = await _reader.ReadLineAsync();

            if (serverResponse?.StartsWith("Username is already taken. Please choose another one.") == true)
            {
                Console.WriteLine("Username is already taken. Please choose another one.");
                CloseConnection();                
                return false;
            }            

            _ = Task.Run(() => MonitorConnectionAsync());
            _ = Task.Run(() => ReceiveMessagesAsync());

            return true;
        }

        public async Task SendMessageAsync(string message)
        {            
            await _writer.WriteLineAsync(message);
        }

        private async Task ReceiveMessagesAsync()
        {            
            while (TcpClient.Connected)
            {
                try
                {
                    string? message = await _reader.ReadLineAsync();

                    if (!string.IsNullOrEmpty(message))
                    {
                        MessageReceived?.Invoke(this, message);
                    }
                }
                catch (IOException)
                {
                    break;
                }
            }
        }

        private async Task MonitorConnectionAsync()
        {
            while (IsClientConnected(TcpClient))
            {
                await Task.Delay(500);
                continue;
            }

            CloseConnection();
            ServerDisconnected?.Invoke(this, EventArgs.Empty);
        }

        private bool IsClientConnected(TcpClient tcpClient)
        {
            if (tcpClient == null || !tcpClient.Connected)
            {
                return false;
            }

            try
            {
                if (tcpClient.Client.Poll(0, SelectMode.SelectRead))
                {
                    byte[] buffer = new byte[1];
                    if (tcpClient.Client.Receive(buffer, SocketFlags.Peek) == 0)
                    {
                        // Socket has been closed
                        return false;
                    }
                }

                return true;
            }
            catch (SocketException)
            {
                return false;
            }
        }

        private void CloseConnection()
        {
            TcpClient.Close();
        }
    }
}
