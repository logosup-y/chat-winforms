using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerPartWinForm
{
    public class ClientInfo
    {
        public string? Username { get; set; }
        public TcpClient? TcpClient { get; set; }
    }
}
