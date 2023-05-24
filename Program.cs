using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Net.NetworkInformation;
using System.Text;
using System.Timers;
using Timer = System.Timers.Timer;

namespace UdpServer
{
    class Program
    {
        public static void Main(string[] args)
        {
            var udpServer = new UdpClient(5000);
            var clients = new Dictionary<IPEndPoint, int>();
            TimeSpan timeout = TimeSpan.FromSeconds(10);
            var clientsToRemove = new List<IPEndPoint>();
            var clientsLastReceiveTime = new Dictionary<IPEndPoint, DateTime>();
            

            Console.WriteLine("udp started with 5000 port");

            try
            {
                while (true)
                {
                    DateTime lastReceivedTime;
                    TimeSpan timeL;
                    IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any,0);
                    var receivedData = udpServer.Receive(ref remoteEP);

                    if (clients.ContainsKey(remoteEP))
                    {
                        clients[remoteEP]++;
                        lastReceivedTime = DateTime.Now;
                    }
                    else
                    {
                        clients[remoteEP] = 1;
                        lastReceivedTime = DateTime.Now;
                        

                    }


                    Console.WriteLine("receive data from " + remoteEP.ToString());
                    var response = Encoding.UTF8.GetBytes(clients[remoteEP].ToString());
                    udpServer.Send(response,response.Length,remoteEP);
                    

                    if ((DateTime.Now - lastReceivedTime).TotalSeconds > timeout.TotalSeconds)
                    {
                        Console.WriteLine("client removed");
                    }

                    /*foreach (var client in clients)
                    {
                        DateTime client_Value = DateTime.FromFileTime(client.Value);
                        if ((DateTime.Now - client_Value).TotalSeconds > timeout.TotalSeconds)
                        {
                            clientsToRemove.Add(client.Key);
                        }
                    }

                    foreach (var key in clientsToRemove)
                    {
                        Console.WriteLine($"{key} has been removed due to inactivity");
                        clients.Remove(key);
                    }*/
                    
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.ReadKey();
        }

        private static void OnTimedEvent(object? sender, ElapsedEventArgs e)
        {
            Console.WriteLine("8 secs");
        }
    }

}