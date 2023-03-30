using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace chat_server_part
{
    public class ClientInfo
    {        public string Username { get; set; }
        public TcpClient TcpClient { get; set; }
    }
}
