﻿using System;
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
            var ipAddr = IPAddress.Parse("127.0.0.1");
            var ipEnd = new IPEndPoint(ipAddr, port);

            Dictionary<string, Socket> dic = new Dictionary<string, Socket>();

            Socket sListener = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

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
                    Console.WriteLine("ВНИМАНИЕ!!! К нам подключился " + Name+"\n");

                    Task.Factory.StartNew((() =>
                    {
                        while (true)
                        {
                            string str = "Server: " + Console.ReadLine();
                            if (dic.Values != null)
                            {
                                foreach (var socket in dic.Values)
                                {
                                    socket.Send(Encoding.UTF8.GetBytes(str));
                                }
                            }
                        }
                    }));

                    Task.Factory.StartNew((() =>
                    {
                        string name = Name;

                        try
                        {
                            Socket acceptTask = accept;

                            while (true)
                            {
                                byte[] bytes = new byte[1024];

                                int recieve = acceptTask.Receive(bytes);

                                string rec = Encoding.UTF8.GetString(bytes, 0, recieve);

                                if (rec == "Exit")
                                {
                                    Console.WriteLine(name + " покинул нас.");
                                    dic.Remove(name);
                                    break;
                                }

                                Console.WriteLine(rec);

                                foreach (var socket in dic.Values)
                                {
                                    socket.Send(Encoding.UTF8.GetBytes(rec));
                                }

                            }
                        }
                        catch (Exception)
                        {
                            Console.WriteLine(Name + " покинул нас.");
                            dic.Remove(name);
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
