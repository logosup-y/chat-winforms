using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ServerPartWinForm
{
    public class ChatServer
    {
        public ServerForm MainForm { get; private set; }
        public IPAddress ServerIpAddress { get; private set; }
        public int Port { get; private set; }
        public List<ClientInfo> ConnectedClients { get; private set; }

        private TcpListener _listener;
        private bool _isRunning;

        public ChatServer(ServerForm mainForm, IPAddress ipAddress, int port)
        {
            MainForm = mainForm;
            ServerIpAddress = ipAddress;
            Port = port;
            ConnectedClients = new List<ClientInfo>();
        }

        public async Task StartAsync()
        {
            _listener = new TcpListener(ServerIpAddress, Port);
            _listener.Start();
            _isRunning = true;

            MainForm.LogMessage($"Server started on {ServerIpAddress}:{Port}");

            while (_isRunning)
            {
                try
                {
                    TcpClient client = await _listener.AcceptTcpClientAsync();
                    
                    ClientInfo clientInfo = new ClientInfo { TcpClient = client };

                    /*MainForm.LogMessage($"{clientInfo.Username} connected.");*/

                    ConnectedClients.Add(clientInfo);

                    Task.Run(() => HandleClientAsync(clientInfo));
                }
                catch (SocketException ex)
                {
                    // Ignore the exception if the server is stopping
                    if (_isRunning)
                    {
                        MainForm.LogMessage($"Error accepting client: {ex.Message}");
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
                clientInfo.TcpClient.Close();
            }

            ConnectedClients.Clear();
        }

        private async Task HandleClientAsync(ClientInfo clientInfo)
        {
            try
            {
                NetworkStream stream = clientInfo.TcpClient.GetStream();
                StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                StreamWriter writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

                // Read the client's username
                clientInfo.Username = await reader.ReadLineAsync();

                MainForm.LogMessage($"Client {clientInfo.Username} connected.");
                BroadcastMessage($"Server: {clientInfo.Username} has joined the chat.");

                while (clientInfo.TcpClient.Connected)
                {
                    string message = await reader.ReadLineAsync();

                    if (clientInfo.TcpClient.Connected && !string.IsNullOrEmpty(message))
                    {
                        MainForm.LogMessage($"{clientInfo.Username}: {message}");
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
                MainForm.LogMessage($"Client {clientInfo.Username} disconnected.");
                BroadcastMessage($"Server: {clientInfo.Username} has left the chat.");
                ConnectedClients.RemoveAll(c => c.Username == clientInfo.Username);
                clientInfo.TcpClient.Close();
            }
        }

        private void BroadcastMessage(string message)
        {
            Console.WriteLine(message);

            var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            string timestampMessage = $"[{timestamp}] {message}";

            foreach (var clientInfo in ConnectedClients)
            {
                NetworkStream stream = clientInfo.TcpClient.GetStream();
                StreamWriter writer = new StreamWriter(stream, new UTF8Encoding(false)) { AutoFlush = true };
                writer.WriteLine(timestampMessage);
            }
        }
    }
}
