using System;
using System.Threading;
using LiteNetLib;
using LiteNetLib.Utils;
using Microsoft.Xna.Framework;

namespace _01_Monogame_multiplayer_game;

public class Client
{
    private EventBasedNetListener listener = new EventBasedNetListener();
    private NetManager client;
    private bool isRunning = false;
    private NetPeer serverPeer;
    
    public Client(string password, string hostIP)
    {
        client = new NetManager(listener);
        if (!client.Start())
            throw new Exception("Failed to start client");
            
        client.Connect(hostIP, 9050, password);
        
        // Setup event handlers
        listener.NetworkReceiveEvent += OnNetworkReceive;
        listener.PeerConnectedEvent += peer =>
        {
            Console.WriteLine("Connected to server!");
            serverPeer = peer;
        };
        
        isRunning = true;
    }
    
    public void Update()
    {
        if (isRunning)
        {
            client.PollEvents();
        }
    }
    
    public void SendMessage(string message)
    {
        if (serverPeer != null && serverPeer.ConnectionState == ConnectionState.Connected)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put(message);
            serverPeer.Send(writer, DeliveryMethod.ReliableOrdered);
        }
        else
        {
            Console.WriteLine("Cannot send message - not connected to server");
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

public class Server : IDisposable
{
    private readonly EventBasedNetListener listener = new EventBasedNetListener();
    private readonly NetManager server;
    private bool isRunning = false;
    private bool isDisposed = false;

    public Server(string password)
    {
        server = new NetManager(listener);
        if (!server.Start(9050))
            throw new Exception("Failed to start server");
            
        listener.ConnectionRequestEvent += request =>
        {
            if (server.ConnectedPeersCount < 2)
                request.AcceptIfKey(password);
            else
                request.Reject();
        };

        listener.PeerConnectedEvent += peer =>
        {
            Console.WriteLine($"Client connected: {peer.Address}");
            SendMessageToClient(peer, "Welcome to the server!");
        };

        listener.NetworkReceiveEvent += OnNetworkReceive;

        isRunning = true;
    }

    public void SendMessageToClient(NetPeer peer, string message)
    {
        if (peer.ConnectionState == ConnectionState.Connected)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put(message);
            peer.Send(writer, DeliveryMethod.ReliableOrdered);
        }
    }

    public void BroadcastMessage(string message)
    {
        NetDataWriter writer = new NetDataWriter();
        writer.Put(message);
        server.SendToAll(writer, DeliveryMethod.ReliableOrdered);
    }

    private void OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channel, DeliveryMethod deliveryMethod)
    {
        try
        {
            string message = reader.GetString();
            Console.WriteLine($"Server received from {peer.Address}: {message}");
            
            // Echo the message back to all clients
            BroadcastMessage($"Client {peer.Address} says: {message}");
        }
        finally
        {
            reader.Recycle();
        }
    }

    public void Update()
    {
        if (isRunning && !isDisposed)
        {
            server.PollEvents();
        }
    }

    public void StopServer()
    {
        if (!isDisposed)
        {
            isRunning = false;
            server.Stop();
        }
    }

    public void Dispose()
    {
        if (!isDisposed)
        {
            StopServer();
            isDisposed = true;
        }
        GC.SuppressFinalize(this);
    }
}