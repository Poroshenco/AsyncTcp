using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace AsyncTCP_Server_2
{
    class Program
    {
        static void Main(string[] args)
        {
            int port = 11000;
            IPAddress ipAddr = IPAddress.Parse("127.0.0.1");
            IPEndPoint ipEnd = new IPEndPoint(ipAddr,port);

            Dictionary<string,Socket> dic = new Dictionary<string, Socket>();

            Socket sListener = new Socket(ipAddr.AddressFamily,SocketType.Stream,ProtocolType.Tcp);

            List<Socket> sockets = new List<Socket>(); 
            try
            {
                sListener.Bind(ipEnd);
                sListener.Listen(20);

                while (true)
                {
                    byte[] user = new byte[1024];
                    Socket accept = sListener.Accept();
                    

                    int UserRec = accept.Receive(user);
                    string Name = Encoding.UTF8.GetString(user, 0, UserRec);

                    dic[Name] = accept;
                    Console.WriteLine("К нам подключился " + Name);

                    Task.Factory.StartNew((() =>
                    {
                        while (true)
                        {
                            string str = "Server: "+Console.ReadLine();

                            
                        }
                    }));


                    Task.Factory.StartNew((() =>
                    {
                        try
                        {
                            Socket acceptTask = accept;

                            while (true)
                            {
                                byte[] bytes = new byte[1024];

                                string str = null;

                                int recieve = acceptTask.Receive(bytes);

                                string rec = Encoding.UTF8.GetString(bytes, 0, recieve);

                                if (rec == "Exit")
                                {
                                    recieve = acceptTask.Receive(bytes);
                                    Console.WriteLine(Encoding.UTF8.GetString(bytes, 0, recieve)+" покинул нас.");
                                    dic.Remove(Encoding.UTF8.GetString(bytes, 0, recieve));
                                    break;
                                }

                                Console.WriteLine(rec);

                                foreach (var socket in sockets)
                                {
                                    socket.Send(Encoding.UTF8.GetBytes(rec));
                                }
                                
                            }
                        }
                        catch (Exception)
                        {
                            Console.WriteLine("Он покинул нас...");
                            throw;
                        }
                    }));
                }
            }
            catch (Exception)
            {
                throw;
            }

        }
    }
}
