using System.Net;

namespace UdpServer
{
    class Client
    {
        public IPEndPoint IpAdress { get; set; }
        public int Count;
        public DateTime lastReceiveTime { get; set; }

        public bool Received { get; set; } = false;

        public Client(IPEndPoint ipAdress)
        {
            IpAdress = ipAdress;
            Count = 1;
        }
    }

}