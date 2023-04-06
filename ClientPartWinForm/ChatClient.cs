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

        public event EventHandler UsernameAlreadyTaken;

        public ChatClient(string username)
        {
            Username = username;
        }

        public async Task<bool> ConnectAsync(IPAddress serverIpAddress, int port)
        {
            TcpClient = new TcpClient();
            await TcpClient.ConnectAsync(serverIpAddress, port);

            NetworkStream stream = TcpClient.GetStream();
            StreamWriter writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };
            StreamReader reader = new StreamReader(stream, Encoding.UTF8);

            // Send the username to the server
            await writer.WriteLineAsync(Username);

            string? serverResponse = await reader.ReadLineAsync();

            if (serverResponse.StartsWith("Username is already taken. Please choose another one."))
            {
                // Close the connection and raise the ServerDisconnected event
                TcpClient.Close();
                UsernameAlreadyTaken?.Invoke(this, EventArgs.Empty);
                return false;
            }

            // Start listening for incoming messages
            Task.Run(() => MonitorConnectionAsync());
            Task.Run(() => ReceiveMessagesAsync());

            return true;
        }

        /*public async Task ConnectAsync(IPAddress serverIpAddress, int port)
        {
            TcpClient = new TcpClient();
            await TcpClient.ConnectAsync(serverIpAddress, port);

            NetworkStream stream = TcpClient.GetStream();
            StreamWriter writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };
            StreamReader reader = new StreamReader(stream, Encoding.UTF8);

            // Send the username to the server
            await writer.WriteLineAsync(Username);

            string? serverResponse = await reader.ReadLineAsync();

            if (serverResponse.StartsWith("Username is already taken. Please choose another one."))
            {
                // Close the connection and raise the ServerDisconnected event
                TcpClient.Close();
                UsernameAlreadyTaken?.Invoke(this, EventArgs.Empty);
                return;
            }

            // Start listening for incoming messages
            Task.Run(() => MonitorConnectionAsync());
            Task.Run(() => ReceiveMessagesAsync());
        }*/

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
        }

        private async Task MonitorConnectionAsync()
        {
            while (IsClientConnected(TcpClient))
            {
                await Task.Delay(500);

                continue;
            }

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
    }
}
