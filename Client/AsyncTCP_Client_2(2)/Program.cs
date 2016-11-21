using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace AsyncTCP_Client_2_2_
{
    class Program
    {
        static void Main(string[] args)
        {
            IPAddress ipAddr = IPAddress.Parse("127.0.0.1");
            int host = 11000;
            IPEndPoint ipEnd = new IPEndPoint(ipAddr, host);
            Console.Write("Enter your name: ");
            string UserName = Console.ReadLine();

            Socket sender = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            string send = null;

            try
            {
                sender.Connect(ipEnd);
                sender.Send(Encoding.UTF8.GetBytes(UserName));

                Task.Factory.StartNew((() =>
                {
                    while (true)
                    {
                        try
                        {
                            byte[] bytes = new byte[1024];
                            int recieve = sender.Receive(bytes);
                            string rec = Encoding.UTF8.GetString(bytes, 0, recieve);
                            if (rec != send)
                                Console.WriteLine(rec);
                        }
                        catch (Exception)
                        {

                        }
                    }
                }));

                while (true)
                {
                    send = Console.ReadLine();
                    if (send.ToLower() == "exit")
                    {
                        sender.Send(Encoding.UTF8.GetBytes("Exit"));
                        sender.Send(Encoding.UTF8.GetBytes(UserName));
                        sender.Shutdown(SocketShutdown.Both);
                        sender.Close();
                        break;
                    }

                    send = UserName + ": " + send;
                    sender.Send(Encoding.UTF8.GetBytes(send));
                }
            }
            catch (Exception)
            {

            }
        }
    }
}
