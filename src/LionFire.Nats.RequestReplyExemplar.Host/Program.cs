using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using NATS.Client.Core;
using LionFire.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using LionFire.Applications;
using Serilog.Core;

await new HostApplicationBuilder()
    .LionFire(lf => lf
        .AppInfo(new AppInfo("NATS.Samples.RequestReply", "LionFire") { OrgDomain = "lionfire.ca" })

        // TODO:
        //.AppInfo(a => a.SetFromDisplayName("NATS RequestReply Exemplar"))  // AppId: LionFire.Nats.RequestReply.Examplar
        //.Directories(AppDirs.ProgramData) // Set up shop in c:/Program Data/LionFire/Nats.RequestReply.Exemplar

        .Framework()
        )

    .ConfigureServices(s => s
        //.AddSingleton<NatsConnection>(sp => new NatsConnection())
        //.Configure<NatsOpts>(o =>
        //{
        //    // TODO: I don't think binder will work, so maybe add overload:
        //    //.Configure<NatsOpts>((configuration, opts) => configuration.WithBind(opts))
        //    o.Url = Environment.GetEnvironmentVariable("NATS_URL") ?? "127.0.0.1:4222";
        //})
        .AddSingleton<NatsConnection>() // TODO REVIEW - Singleton or Transient?, TODO - get NatsOpts from IServiceProvider
        .AddHostedService<AppInfoNatsHostedService>()
        .AddHostedService<X>()
    )
#if true
    .Build()
    .RunAsync();
#else
    .RunAsync(async sp =>
    {
        var hal = sp.GetRequiredService<IHostApplicationLifetime>();

        var stopwatch = Stopwatch.StartNew();
        ILogger Logger = sp.GetRequiredService<ILogger<Program>>();


        var url = Environment.GetEnvironmentVariable("NATS_URL") ?? "127.0.0.1:4222";
        Log($"[CON] Connecting to {url}...");

        var opts = NatsOpts.Default with { Url = url };
        await using var nats = new NatsConnection(opts);

        #region Subscribe

        Task? responder = null;

        async Task<INatsSub<int>> Subscribe()
        {
            var sub = await nats.SubscribeCoreAsync<int>("hello.*", opts: new NatsSubOpts { MaxMsgs = 5 });
            var reader = sub.Msgs;
            responder = Task.Run(async () =>
            {
                try
                {
                    await foreach (var msg in reader.ReadAllAsync())
                    {
                        var name = msg.Subject.Split('.')[1];
                        Log($"[REP] Received {msg.Subject}");
                        //await Task.Delay(500);
                        await msg.ReplyAsync($"Hello {name}!");
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, $"[REP exception]");
                }
            });
            return sub;
        }
        await using var sub = await Subscribe();

        #endregion

        var replyOpts = new NatsSubOpts { Timeout = TimeSpan.FromSeconds(2) };

        await SendSampleRequests();
        async Task SendSampleRequests()
        {
            Log("[REQ] From red");
            var reply = await nats.RequestAsync<int, string>("hello.red", 0, replyOpts: replyOpts);
            Log($"[REQ] {reply.Data}");

            Log("[REQ] From blue");
            reply = await nats.RequestAsync<int, string>("hello.blue", 0, replyOpts: replyOpts);
            Log($"[REQ] {reply.Data}");

            Log("[REQ] From black");
            reply = await nats.RequestAsync<int, string>("hello.black", 0, replyOpts: replyOpts);
            Log($"[REQ] {reply.Data}");
        }

        while (!hal.ApplicationStopping.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(1500));
        }

        await sub.UnsubscribeAsync();

        if (responder != null) await responder;

        //await SendSampleRequestWithNoListener();
        async Task SendSampleRequestWithNoListener()
        {
            try
            {
                var reply = await nats.RequestAsync<int, string>("hello.red", 0, replyOpts: replyOpts);
                Log($"[REQ] {reply.Data} - This will timeout. We should not see this message.");
            }
            catch (NatsNoReplyException)
            {
                Log("[REQ] timed out!");
            }
        }

        Log("Bye!");

        return;

        void Log(string log) => Logger.LogInformation($"{stopwatch!.Elapsed} {log}");
    });
#endif

public class X : IHostedService
{
    public ILogger<X> Logger { get; }

    NatsConnection nats;
    public X(ILogger<X> logger)
    {
        Logger = logger;
        var url = Environment.GetEnvironmentVariable("NATS_URL") ?? "127.0.0.1:4222";
        Logger.LogInformation($"[CON] Connecting to {url}...");
        var opts = NatsOpts.Default with { Url = url };
        nats = new NatsConnection(opts);
    }

    INatsSub<int>? sub = null;
    Task? responder = null;
    NatsSubOpts replyOpts = new NatsSubOpts { Timeout = TimeSpan.FromSeconds(2) };

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        sub = await nats.SubscribeCoreAsync<int>("hello.*");
        var reader = sub.Msgs;

        responder = Task.Run(async () =>
        {
            try
            {
                await foreach (var msg in reader.ReadAllAsync())
                {
                    var name = msg.Subject.Split('.')[1];
                    Log($"[REP] Received {msg.Subject}");
                    //await Task.Delay(500);
                    await msg.ReplyAsync($"Hello {name}!");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"[REP exception]");
            }
        });


        await SendSampleRequests();
        async Task SendSampleRequests()
        {
            Log("[REQ] From red");
            var reply = await nats.RequestAsync<int, string>("hello.red", 0, replyOpts: replyOpts);
            Log($"[REQ] {reply.Data}");

            Log("[REQ] From blue");
            reply = await nats.RequestAsync<int, string>("hello.blue", 0, replyOpts: replyOpts);
            Log($"[REQ] {reply.Data}");

            Log("[REQ] From black");
            reply = await nats.RequestAsync<int, string>("hello.black", 0, replyOpts: replyOpts);
            Log($"[REQ] {reply.Data}");
        }

    }
    void Log(string log) => Logger.LogInformation($"{log}");

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await sub.UnsubscribeAsync();
        await sub.DisposeAsync();

        if (responder != null) await responder;

        try
        {
            var reply = await nats.RequestAsync<int, string>("hello.red", 0, replyOpts: replyOpts);
            Log($"[REQ] {reply.Data} - This will timeout. We should not see this message.");
        }
        catch (NatsNoRespondersException)
        {
            Log("[REQ] no responders!");
        }
        catch (NatsNoReplyException)
        {
            Log("[REQ] timed out!");
        }

        Log("Bye!");

        await nats.DisposeAsync();
    }
}
