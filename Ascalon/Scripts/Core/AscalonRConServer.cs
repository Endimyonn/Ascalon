using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Sockets;
using System.Net;

public class AscalonRConServer
{
    public static Thread serverThread;
    public static int serverPort = 7701;

    public async void StartListening()
    {
        await Task.Run(() =>
        {
            TcpListener listener = new TcpListener(IPAddress.Any, serverPort);

            bool listenerActive = false;
            try
            {
                listener.Start();
                listenerActive = true;
            }
            catch (System.Net.Sockets.SocketException)
            {
                Console.WriteLine("Cannot start AscalonRConServer as the port " + serverPort + " is already in use!");
                listenerActive = false;
            }


            if (listenerActive)
            {
                while (true)
                {
                    try
                    {
                        TcpClient client = listener.AcceptTcpClient();

                        //get the incoming data
                        NetworkStream stream = client.GetStream();
                        byte[] buffer = new byte[client.ReceiveBufferSize];

                        //read the stream, size it down
                        int byteCount = stream.Read(buffer, 0, client.ReceiveBufferSize);

                        //convert data into an rcon call
                        AscalonRConData data = AscalonRConModule.DecodePacket(buffer);

                        if (data.password == (string)Ascalon.GetConVar("rcon_password"))
                        {
                            Ascalon.Call(data.call);
                        }
                        else
                        {
                            //send bad password response
                        }

                        client.Close();
                    }
                    catch (Exception issue)
                    {
                        Console.WriteLine("Issue while receiving RCon packet: " + issue.Message);
                    }
                }
            }
        });
    }

    public void Start()
    {
        if (serverThread != null)
        {
            if (serverThread.IsAlive)
            {
                Console.WriteLine("Cannot start server when it is already running!");
                return;
            }
        }

        ThreadStart threadRef = new ThreadStart(StartListening);
        serverThread = new Thread(threadRef);
        serverThread.Start();
    }

    public static void Stop()
    {
        if (serverThread != null)
        {
            if (serverThread.IsAlive)
            {
                Console.WriteLine("Stopping server...");
                serverThread.Abort();
            }
            else
            {
                Console.WriteLine("Server has already stopped!");
            }
        }
        else
        {
            Console.WriteLine("No server exists!");
        }
    }
}
