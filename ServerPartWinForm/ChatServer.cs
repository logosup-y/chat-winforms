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
                    MainForm.LogMessage("Client connected.");

                    ClientInfo clientInfo = new ClientInfo { TcpClient = client };

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
            NetworkStream stream = clientInfo.TcpClient.GetStream();
            StreamReader reader = new StreamReader(stream, Encoding.UTF8);
            StreamWriter writer = new StreamWriter(stream, new UTF8Encoding(false)) { AutoFlush = true };

            // Get the username and store it in the ClientInfo object
            clientInfo.Username = await reader.ReadLineAsync();

            // Broadcast a welcome message to all clients
            BroadcastMessage($"Server: {clientInfo.Username} has joined the chat.");

            // Read incoming messages from the client and broadcast them to all connected clients
            while (_isRunning && clientInfo.TcpClient.Connected)
            {
                try
                {
                    string message = await reader.ReadLineAsync();
                    if (!string.IsNullOrWhiteSpace(message))
                    {
                        BroadcastMessage($"{clientInfo.Username}: {message}");
                    }
                }
                catch (IOException)
                {
                    // Client has disconnected
                    break;
                }
            }

            // Clean up after client disconnects
            ConnectedClients.Remove(clientInfo);
            clientInfo.TcpClient.Close();
            MainForm.LogMessage($"{clientInfo.Username} has left the chat.");
            BroadcastMessage($"Server: {clientInfo.Username} has left the chat.");
        }

        private void BroadcastMessage(string message)
        {
            Console.WriteLine(message);
            foreach (var clientInfo in ConnectedClients)
            {
                NetworkStream stream = clientInfo.TcpClient.GetStream();
                StreamWriter writer = new StreamWriter(stream, new UTF8Encoding(false)) { AutoFlush = true };
                writer.WriteLine(message);
            }
        }
    }
}
