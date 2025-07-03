// Based on LiteNetLib sample

using System;
using System.Threading;
using LiteNetLib;
using LiteNetLib.Utils;

namespace LionFire.RPC.Modes;

class SimpleHelloClient : IMode
{
    public void Run()
    {
        var address = "127.0.0.1";
        int port = 6130;
        var msg = "hello, connected";

        Console.WriteLine($"Connecting to {address}:{port} and sending '{msg}'...");
        var listener = new EventBasedNetListener();
        bool responseReceived = false;

        #region Events

        listener.PeerConnectedEvent += peer =>
        {
            var writer = new NetDataWriter();
            writer.Put(msg);
            peer.Send(writer, DeliveryMethod.Unreliable);
        };

        listener.NetworkReceiveEvent += (peer, reader, channel, method) =>
        {
            string msg = reader.GetString(100);
            Console.WriteLine($"Received: {msg}");
            responseReceived = true;
        };

        listener.PeerDisconnectedEvent += (peer, info) =>
        {
            Console.WriteLine($"Disconnected: {info.Reason}");
        };

        #endregion

        var client = new NetManager(listener);
        client.Start(8158);

        var serverPeer = client.Connect(address, port, "unused key");
        int waited = 0;

        var writer = new NetDataWriter();
        writer.Put("hello2");
        client.SendUnconnectedMessage(writer, address, port);

        while (!responseReceived && waited < 3000)
        {
            client.PollEvents();
            Thread.Sleep(10);
            waited += 10;
        }
        if (!responseReceived)
        {
            Console.WriteLine("No response received in 3 seconds.");
        }
        client.Stop();
    }
}
