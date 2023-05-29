using System.Net;
using System.Net.Sockets;
using System.Text;

class Program
{
    public static void Main(string[] args)
    {
        int i = 0;

        IPEndPoint ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5000);
       
        using var udpClient = new UdpClient(1235);
        while (i <= 2)
        {
            Thread.Sleep(500);
            var mes = Console.ReadLine();
            var response = Encoding.UTF8.GetBytes(mes);
            udpClient.Send(response, response.Length, ep);
            Console.WriteLine("data sent");
            i++;
            var receiveData = udpClient.Receive(ref ep);
            var received = Encoding.UTF8.GetString(receiveData);
            Console.WriteLine(received);
        }
    }
}