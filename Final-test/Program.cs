using System.Net;
using System.Net.Sockets;
using System.Text;

namespace UdpServer
{

    class Client
    {
        public IPEndPoint IpAdress { get; set; }
        public int Count;
        public DateTime lastReceiveTime { get; set; }
        

        public Client(IPEndPoint ipAdress)
        {
            IpAdress = ipAdress;
            Count = 1;
        }

    }

    class Program
    {
        public static async Task Main(string[] args)
        {
            
            
            using var udpServer = new UdpClient(5000);
            TimeSpan timeout = TimeSpan.FromSeconds(20);
            var clients = new Dictionary<IPEndPoint, Client>();
            var clientsToRemove = new List<IPEndPoint>();
            var clientsLastReceiveTime = new Dictionary<IPEndPoint, DateTime>();

            

            Console.WriteLine("udp started with 5000 port");

            try
            {
                while (true)
                {

                    IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
                    var receivedData = udpServer.Receive(ref remoteEP);

                    ClientsDataReceiveing(udpServer, timeout, clients, remoteEP, receivedData);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.ReadKey();
        }

        private static async Task ClientsDataReceiveing(UdpClient udpServer, TimeSpan timeout, Dictionary<IPEndPoint, Client> clients, IPEndPoint remoteEP, byte[] receivedData)
        {
            try
            {
                if (clients.ContainsKey(remoteEP))
                {
                    Client existingClient = clients[remoteEP];
                    existingClient.Count++;


                    existingClient.lastReceiveTime = DateTime.Now;
                }
                else
                {
                    Client client = new Client(remoteEP);
                    clients[remoteEP] = client;
                    clients[remoteEP].lastReceiveTime = DateTime.Now;

                }
                var received = Encoding.UTF8.GetString(receivedData);
                Console.WriteLine($"Data - \"{received}\" received from " + remoteEP.ToString());
                var response = Encoding.UTF8.GetBytes(clients[remoteEP].Count.ToString());
                udpServer.Send(response, response.Length, remoteEP);

                await Task.Delay(20000);
                if (DateTime.Now - clients[remoteEP].lastReceiveTime > timeout)
                {
                    Console.WriteLine($"client {clients[remoteEP].IpAdress} removed due to inactivity");

                    if(clients.Remove(remoteEP))
                    {
                        Console.WriteLine("Client removed");
                    }
                    else
                    {
                        Console.WriteLine("Client not found");
                    }
                }
            }


            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }

}