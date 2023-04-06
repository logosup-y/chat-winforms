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

        public async Task StartAsync()
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

            // Close all connected clients
            foreach (var clientInfo in ConnectedClients)
            {
                clientInfo.TcpClient?.Close();
            }

            ConnectedClients.Clear();
        }

        private async Task HandleClientAsync(ClientInfo clientInfo, StreamReader reader, StreamWriter writer)
        {
            try
            {                
                RaiseLogMessageEvent($"Client \"{clientInfo.Username}\" connected.");
                BroadcastMessage($"Server: Client \"{clientInfo.Username}\" has joined the chat.");

                while (clientInfo.TcpClient.Connected)
                {
                    string message = await reader.ReadLineAsync();

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
                RaiseLogMessageEvent($"Client {clientInfo.Username} disconnected.");
                BroadcastMessage($"Server: {clientInfo.Username} has left the chat.");
                ConnectedClients.RemoveAll(c => c.Username == clientInfo.Username);
                clientInfo.TcpClient?.Close();
            }
        }

        private void BroadcastMessage(string message)
        {
            /*Console.WriteLine(message);*/

            foreach (var clientInfo in ConnectedClients)
            {
                NetworkStream stream = clientInfo.TcpClient.GetStream();
                StreamWriter writer = new StreamWriter(stream, new UTF8Encoding(false)) { AutoFlush = true };
                writer.WriteLine($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}] {message}");
            }
        }

        protected void RaiseLogMessageEvent(string message)
        {
            OnLogMessage?.Invoke(this, message);
        }

        private async Task AcceptClientAsync()
        {
            TcpClient client = await _listener.AcceptTcpClientAsync();

            ClientInfo clientInfo = new ClientInfo { TcpClient = client };                    

            NetworkStream stream = clientInfo.TcpClient.GetStream();
            StreamReader reader = new(stream, Encoding.UTF8);
            StreamWriter writer = new(stream, Encoding.UTF8);

            if (IsClientNameUnique(clientInfo, reader, writer))
            {
                ConnectedClients.Add(clientInfo);
                _ = Task.Run(() => HandleClientAsync(clientInfo, reader, writer));
            } 
            else
            {
                await RefuseClient(clientInfo);
            }
        }

        private bool IsClientNameUnique(ClientInfo clientInfo, StreamReader reader, StreamWriter writer)
        {
            // Read the client's username
            string? Username = reader.ReadLine();

            if (ConnectedClients.Any(c => c.Username.Equals(Username, StringComparison.OrdinalIgnoreCase)))
            {
                return false;
            }
            else
            {
                clientInfo.Username = Username;
                return true;
            }

            /* if (ConnectedClients.Count <= 1)
             {
                 clientInfo.Username = Username;
                 return true;
             }
             else if (ConnectedClients.Take(ConnectedClients.Count - 1).Any(c => c.Username.Equals(Username, StringComparison.OrdinalIgnoreCase)))
             {
                 // Notify the client that the username is not unique and close the connection
                 clientInfo.Username = Username;                
                 return false;
             }
             else
             {
                 clientInfo.Username = Username;
                 return true;
             }*/
        }

        private async Task RefuseClient(ClientInfo clientInfo)
        {
            NetworkStream stream = clientInfo.TcpClient.GetStream();            
            StreamWriter writer = new(stream, Encoding.UTF8) { AutoFlush = true };

            await writer.WriteLineAsync($"Username is already taken. Please choose another one.");            
            await Task.Delay(500);
            ConnectedClients.Remove(clientInfo);
            clientInfo.TcpClient?.Close();
        }
    }
}
