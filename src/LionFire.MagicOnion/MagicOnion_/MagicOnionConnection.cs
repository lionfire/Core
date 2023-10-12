using Grpc.Net.Client;
using LionFire.Data.Connections;
using MagicOnion.Client;

namespace LionFire.MagicOnion_;

public class MagicOnionConnection : IConnection
{
    /// <summary>
    /// e.g. "https://localhost:5001"
    /// </summary>
    public string? ConnectionString { get; set; }

    GrpcChannel? channel;

    public Task StartAsync(CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(ConnectionString, nameof(ConnectionString));

        // Connect to the server using gRPC channel.
        channel = GrpcChannel.ForAddress(ConnectionString);

        //// NOTE: If your project targets non-.NET Standard 2.1, use `Grpc.Core.Channel` class instead.
        //// var channel = new Channel("localhost", 5001, new SslCredentials());

        //// Create a proxy to call the server transparently.
        //var client = MagicOnionClient.Create<IMagicOnionTestService>(channel);

        //// Call the server-side method using the proxy.
        //var result = await client.SumAsync(123, 456);
        //Console.WriteLine($"Result: {result}");

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken = default)
    {
        var copy = channel;
        channel = null;
        copy.Dispose();
        return Task.CompletedTask;
    }


}

