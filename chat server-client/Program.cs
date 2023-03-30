using chat_server_part;
using System;
using System.Net;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Starting chat server...");
        var server = new ChatServer(IPAddress.Any, 4000);
        await server.StartAsync();
    }
}