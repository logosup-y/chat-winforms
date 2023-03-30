using chat_client_part;
using System;
using System.Net;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        Console.Write("Enter your username: ");
        string username = Console.ReadLine();

        var client = new ChatClient(username);

        Console.WriteLine($"Connecting to chat server as {username}...");
        await client.ConnectAsync(IPAddress.Loopback, 4000);

        Console.WriteLine("Connected. Type your messages below:");

        while (true)
        {
            string message = Console.ReadLine();
            await client.SendMessageAsync(message);
        }
    }
}