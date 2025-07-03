// Based on LiteNetLib sample

using LiteNetLib;
using LiteNetLib.Utils;
using System;
using System.Threading;

namespace LionFire.RPC.Modes;

class SimpleHelloServer : IMode
{
    public void Run()
    {
        int port = 6130;
        Console.WriteLine($"Listening on port {port} for client connection...");
        var listener = new EventBasedNetListener();
        bool messageReceived = false;

        #region Events

        listener.ConnectionRequestEvent += request =>
        {
            request.AcceptIfKey("unused key");
        };

        listener.PeerConnectedEvent += peer =>
        {
            Console.WriteLine($"Client connected: {peer}");
        };

        listener.NetworkReceiveEvent += (peer, reader, channel, method) =>
        {
            string msg = reader.GetString(100);
            Console.WriteLine($"Received from client: {msg}");
            var writer = new NetDataWriter();
            writer.Put($"Hello client, got your message: {msg}");
            peer.Send(writer, DeliveryMethod.Unreliable);
            messageReceived = true;
        };

        listener.PeerDisconnectedEvent += (peer, info) =>
        {
            Console.WriteLine($"Client disconnected: {info.Reason}");
        };

        #endregion

        var server = new NetManager(listener);
        server.Start(port);

        int waited = 0;
        while (!messageReceived && waited < 5000)
        {
            server.PollEvents();
            Thread.Sleep(10);
            waited += 10;
        }
        if (!messageReceived)
        {
            Console.WriteLine("No message received from client in 5 seconds.");
        }
        server.Stop();
    }
}
