using System;
using System.Threading;
using LiteNetLib;
using LiteNetLib.Utils;
using Microsoft.Xna.Framework;

namespace experimental_chat;

public class Client
{
    private EventBasedNetListener listener = new EventBasedNetListener();
    private NetManager client;
    private bool isRunning = false;
    
    public Client(string password, string hostIP)
    {
        client = new NetManager(listener);
        client.Start();
        client.Connect(hostIP, 9050, password);
        
        listener.NetworkReceiveEvent += OnNetworkReceive;
        isRunning = true;
    }
    
    public void Update()
    {
        if (isRunning)
        {
            client.PollEvents();
        }
    }
    
    private void OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channel, DeliveryMethod deliveryMethod)
    {
        Console.WriteLine("We got: {0}", reader.GetString(100));
        reader.Recycle();
    }
    
    public void StopClient()
    {
        isRunning = false;
        client.Stop();
    }
}

public class Server 
{
    private EventBasedNetListener listener = new EventBasedNetListener();
    private NetManager server;
    private bool isRunning = false;

    public Server(string password)
    {
        server = new NetManager(listener);
        server.Start(9050);
        
        listener.ConnectionRequestEvent += request =>
        {
            if (server.ConnectedPeersCount < 2)
                request.AcceptIfKey(password);
            else
                request.Reject();
        };

        listener.PeerConnectedEvent += peer =>
        {
            Console.WriteLine("We got connection: {0}", peer.Address);
            NetDataWriter writer = new NetDataWriter();
            writer.Put("Hello client!");
            peer.Send(writer, DeliveryMethod.ReliableOrdered);
        };

        isRunning = true;
    }

    public void Update()
    {
        if (isRunning)
        {
            server.PollEvents();
        }
    }

    public void StopServer()
    {
        isRunning = false;
        server.Stop();
    }
}