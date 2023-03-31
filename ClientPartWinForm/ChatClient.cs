using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ClientPartWinForm
{
    public class ChatClient
    {
        public string Username { get; private set; }
        public TcpClient TcpClient { get; private set; }

        public event EventHandler<string> MessageReceived;

        public event EventHandler ServerDisconnected;

        public ChatClient(string username)
        {
            Username = username;
        }

        public async Task ConnectAsync(IPAddress serverIpAddress, int port)
        {
            TcpClient = new TcpClient();
            await TcpClient.ConnectAsync(serverIpAddress, port);

            NetworkStream stream = TcpClient.GetStream();
            StreamWriter writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

            // Send the username to the server
            await writer.WriteLineAsync(Username);

            // Start listening for incoming messages
            Task.Run(() => MonitorConnectionAsync());
            Task.Run(() => ReceiveMessagesAsync());
        }

        public async Task SendMessageAsync(string message)
        {
            NetworkStream stream = TcpClient.GetStream();
            StreamWriter writer = new StreamWriter(stream, new UTF8Encoding(false)) { AutoFlush = true };

            await writer.WriteLineAsync(message);
        }

        private async Task ReceiveMessagesAsync()
        {
            while (TcpClient.Connected)
            {
                try
                {
                    NetworkStream stream = TcpClient.GetStream();
                    StreamReader reader = new StreamReader(stream, new UTF8Encoding(false));

                    string message = await reader.ReadLineAsync();

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

            // Clean up after disconnection
            TcpClient.Close();
            Console.WriteLine("Disconnected from server.");
        }

        private async Task MonitorConnectionAsync()
        {
            while (TcpClient.Connected)
            {
                await Task.Delay(1000);

                if (TcpClient.Client == null)
                {
                    break;
                }

                if (TcpClient.Client.Poll(0, SelectMode.SelectRead) && TcpClient.Client.Available == 0)
                {
                    ServerDisconnected?.Invoke(this, EventArgs.Empty);                    
                    break;
                }
            }
        }
    }
}
