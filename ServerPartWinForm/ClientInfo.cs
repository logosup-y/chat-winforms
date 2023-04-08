using System.Net.Sockets;

namespace ServerPartWinForm
{
    public class ClientInfo
    {
        public TcpClient? TcpClient { get; set; }
        public string? Username { get; set; }
        public StreamWriter? Writer { get; private set; }
        public StreamReader? Reader { get; private set; }
                
        public void SetWriter(StreamWriter writer)
        {
            Writer = writer;
        }

        public void SetReader(StreamReader reader)
        {
            Reader = reader;
        }
    }
}
