using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.CompilerServices;

namespace ServerPartWinForm
{
    public class ChatServer
    {        
        public IPAddress ServerIpAddress { get; private set; }
        public int Port { get; private set; }
        public List<ClientInfo> ConnectedClients { get; private set; }

        public event EventHandler<string>? OnLogMessage;

        private TcpListener _listener;
        private bool _isRunning;

        public ChatServer(IPAddress ipAddress, int port)
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
                await writer.WriteLineAsync($"You are connected to the server");
                _ = Task.Run(() => HandleClientAsync(clientInfo));
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
                clientInfo.TcpClient?.Close();
            }
        }

        private void BroadcastMessage(string message)
        {            
            foreach (var client in ConnectedClients)
            {
                StreamWriter writer = client.Writer;
                writer.WriteLine($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}] {message}");
            }
        }

        private void RaiseLogMessageEvent(string message)
        {
            OnLogMessage?.Invoke(this, message);
        }
        
        private async Task RefuseClient(ClientInfo clientInfo)
        {
            StreamWriter writer = clientInfo.Writer;

            await writer.WriteLineAsync($"Username is already taken. Please choose another one.");            
            await Task.Delay(500);
            ConnectedClients.Remove(clientInfo);
            clientInfo.TcpClient?.Close();
        }
    }
}
