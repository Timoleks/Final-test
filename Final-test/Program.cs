using System.Net;
using System.Net.Sockets;
using System.Text;

namespace UdpServer;

class Program
{
    private static Dictionary<IPEndPoint, Client> _clients = new();

    public static async Task Main(string[] args)
    {
        using var udpServer = new UdpClient(5000);
        TimeSpan timeout = TimeSpan.FromSeconds(20);

        Console.WriteLine("udp started with 5000 port");

        try
        {
            ClientRemove(timeout); 
            while (true)
            {
                IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
                var receivedData = udpServer.Receive(ref remoteEP);
                var received = Encoding.UTF8.GetString(receivedData);
                ClientsDataReceiveing(udpServer, timeout, remoteEP, received);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }

        Console.ReadKey();
    }

    private static async Task ClientsDataReceiveing(UdpClient udpServer, TimeSpan timeout, IPEndPoint remoteEP, string received)
    {
        try
        {
            Client? client;
            var clientExists = _clients.TryGetValue(remoteEP, out client);

            if (received == "Confirmed" && clientExists)
            {
                client!.Received = true;

                return;
            }

            if (clientExists)
            {
                client!.Count++;
                client.Received = false;
                client.lastReceiveTime = DateTime.Now;
                DataSend(udpServer, remoteEP, received);
                return;
            }

            _clients[remoteEP] = new Client(remoteEP);
            _clients[remoteEP].lastReceiveTime = DateTime.Now;

            DataSend(udpServer, remoteEP, received);
               

        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    private static async Task ClientRemove(TimeSpan timeout)
    {
        while (true)
        {
            foreach (var client in _clients.Values)
            {
                if (DateTime.Now - client.lastReceiveTime > timeout)
                {
                    Console.WriteLine($"client {client.IpAdress} removed due to inactivity");

                    if (_clients.Remove(client.IpAdress))
                    {
                        Console.WriteLine("Client removed");
                    }
                    else
                    {
                        Console.WriteLine("Client not found");
                    }
                }
                   
            }
            await Task.Delay(1000);
        }
            
    }

    private static void DataSend(UdpClient udpServer, IPEndPoint remoteEP, string received)
    {
        Console.WriteLine($"Data - \"{received}\" received from " + remoteEP.ToString());
        var response = Encoding.UTF8.GetBytes(_clients[remoteEP].Count.ToString());
        udpServer.Send(response, response.Length, remoteEP);
    }
}