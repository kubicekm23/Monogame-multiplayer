using System;
using System.Threading;
using LiteNetLib;
using LiteNetLib.Utils;

namespace experimental_chat;


public class Client {
    public Client(string password) {}
}


public class Server {
    EventBasedNetListener listener = new EventBasedNetListener();
    private NetManager server;

    public Server(string heslo)
    {
        server = new NetManager(listener);
        server.Start(9050 /* port */);

        listener.ConnectionRequestEvent += request =>
        {
            if (server.ConnectedPeersCount < 2 /* max connections */)
                request.AcceptIfKey(heslo);
            else
                request.Reject();
        };

        listener.PeerConnectedEvent += peer =>
        {
            Console.WriteLine("We got connection: {0}", peer.Address); // Show peer ip
            NetDataWriter writer = new NetDataWriter(); // Create writer class
            writer.Put("Hello client!"); // Put some string
            peer.Send(writer, DeliveryMethod.ReliableOrdered); // Send with reliability
        };

        while (!Console.KeyAvailable)
        {
            server.PollEvents();
            Thread.Sleep(15);
        }
    }

    public void EndServer()
    {
        server.Stop();
    }
}