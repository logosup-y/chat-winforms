using System.Net.Sockets;
using System.Net;
using System.Text;

namespace ServerPartWinForm
{
    public class ChatServer
    {        
        public IPAddress ServerIpAddress { get; private set; }
        public ushort Port { get; private set; }
        public List<ClientInfo> ConnectedClients { get; private set; }

        public event EventHandler<string>? OnLogMessage;
        public event EventHandler<string>? OnClientConnected;
        public event EventHandler<string>? OnClientDisconnected;

        private readonly TcpListener _listener;
        private bool _isRunning;

        public ChatServer(IPAddress ipAddress, ushort port)
        {           
            ServerIpAddress = ipAddress;
            Port = port;
            ConnectedClients = new List<ClientInfo>();
            _listener = new TcpListener(ServerIpAddress, Port);
        }

        public async Task StartAsync_ListenClients()
        {            
            _listener.Start();
            _isRunning = true;

            RaiseLogMessageEvent($"Server started on {ServerIpAddress}:{Port}");

            while (_isRunning)
            {
                try
                {
                    await AcceptClientAsync();                    
                }
                catch (SocketException ex)
                {
                    // Ignore the exception if the server is stopping
                    if (_isRunning)
                    {
                        RaiseLogMessageEvent($"Error accepting client: {ex.Message}");
                    }
                }                
            }
        }

        public void Stop()
        {
            _isRunning = false;
            _listener.Stop();

            foreach (var client in ConnectedClients)
            {
                client.TcpClient?.Close();
            }

            ConnectedClients.Clear();
        }

        private async Task AcceptClientAsync()
        {
            TcpClient pendingClient = await _listener.AcceptTcpClientAsync();

            ClientInfo clientInfo = new ClientInfo { TcpClient = pendingClient };

            NetworkStream stream = clientInfo.TcpClient.GetStream();
            StreamWriter writer = new(stream, Encoding.UTF8) { AutoFlush = true };
            StreamReader reader = new(stream, Encoding.UTF8);
            

            clientInfo.SetReader(reader);
            clientInfo.SetWriter(writer);

            if (IsClientNameUnique(clientInfo))
            {                
                ConnectedClients.Add(clientInfo);                
                _ = Task.Run(() => HandleClientAsync(clientInfo));
                _ = Task.Run(() => MonitorConnectionAsync(clientInfo));
            }
            else
            {
                await RefuseClient(clientInfo);
            }
        }

        private bool IsClientNameUnique(ClientInfo clientInfo)
        {
            // Read the client's username
            string? Username = clientInfo.Reader.ReadLine();

            if (ConnectedClients.Any(c => c.Username.Equals(Username, StringComparison.OrdinalIgnoreCase)))
            {
                return false;
            }
            else
            {
                clientInfo.Username = Username;
                return true;
            }
        }

        private async Task HandleClientAsync(ClientInfo clientInfo)
        {
            try
            {                
                RaiseLogMessageEvent($"Client \"{clientInfo.Username}\" connected.");
                BroadcastMessage($"Server: Client \"{clientInfo.Username}\" has joined the chat.");
                OnClientConnected?.Invoke(this, clientInfo.Username);

                while (clientInfo.TcpClient.Connected)
                {
                    string message = await clientInfo.Reader.ReadLineAsync();

                    if (clientInfo.TcpClient.Connected && !string.IsNullOrEmpty(message))
                    {
                        RaiseLogMessageEvent($"{clientInfo.Username}: {message}");
                        BroadcastMessage($"{clientInfo.Username}: {message}");
                    }
                    else
                    {
                        break;
                    }
                }
            }
            catch (IOException)
            {
                // Handle disconnection
            }
            finally
            {
                RaiseLogMessageEvent($"Client \"{clientInfo.Username}\" disconnected.");
                BroadcastMessage($"Server: \"{clientInfo.Username}\" has left the chat.");                
                ConnectedClients.RemoveAll(c => c.Username == clientInfo.Username);
                CloseConnection(clientInfo);
                OnClientDisconnected?.Invoke(this, clientInfo.Username);
            }
        }

        private void BroadcastMessage(string message)
        {            
            foreach (var client in ConnectedClients)
            {   
                client.Writer?.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}");
            }
        }

        private void RaiseLogMessageEvent(string message)
        {
            OnLogMessage?.Invoke(this, message);
        }
        
        private async Task RefuseClient(ClientInfo clientInfo)
        {      
            await clientInfo.Writer.WriteLineAsync($"Username is already taken. Please choose another one.");            
            await Task.Delay(500);
            ConnectedClients.Remove(clientInfo);
            CloseConnection(clientInfo);
        }

        private async Task MonitorConnectionAsync(ClientInfo clientInfo)
        {
            while (IsClientConnected(clientInfo))
            {
                await Task.Delay(500);
                continue;
            }

            ConnectedClients.RemoveAll(c => c.Username == clientInfo.Username);
            CloseConnection(clientInfo);
            OnClientDisconnected?.Invoke(this, clientInfo.Username);
        }

        private bool IsClientConnected(ClientInfo clientInfo)
        {
            if (clientInfo.TcpClient == null || !clientInfo.TcpClient.Connected)
            {
                return false;
            }

            try
            {
                if (clientInfo.TcpClient.Client.Poll(0, SelectMode.SelectRead))
                {
                    byte[] buffer = new byte[1];
                    if (clientInfo.TcpClient.Client.Receive(buffer, SocketFlags.Peek) == 0)
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

        private void CloseConnection(ClientInfo clientInfo)
        {
            clientInfo.TcpClient?.Close();
        }
    }
}
