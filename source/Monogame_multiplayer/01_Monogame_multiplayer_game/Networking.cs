using System;
using System.IO;
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
    private int port;

    public Server(string password, int port)
    {
        server = new NetManager(listener);
        if (!server.Start(port))
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

struct NetworkSettings
{
    public string HostIP;
    public int Port;
    public bool IsServer;
    public string Password;
    
    public NetworkSettings(string IP, int port, bool isServer, string password)
    {
        HostIP = IP;
        Port = port;
        IsServer = isServer;
        Password = password;

        if (!File.Exists("network_settings.txt"))
        {
            File.WriteAllText("network_settings.txt", $"{HostIP}\n{Port}\n{Password}\n{IsServer}");
        }
        else
        {
            string[] lines = File.ReadAllLines("NetworkSettings.txt");
            HostIP = lines[0];
            Port = int.Parse(lines[1]);
            Password = lines[2];
            if (lines[3] == "true") isServer = true;
            else isServer = false;
        }
        
        Console.WriteLine($"Using the following network settings");
        Console.WriteLine($"Adress: {HostIP}:{Port}"); Console.WriteLine($"Password: {Password}"); Console.WriteLine($"Runs as server: {isServer}");
    }
}