using NATS.Client.Core;
using Microsoft.Extensions.Logging;
using LionFire.Applications;
using LionFire.ExtensionMethods.Dumping;

public class AppInfoNatsHostedService : NatsHostedService
{
    public AppInfo AppInfo { get; }

    public AppInfoNatsHostedService(NatsConnection natsConnection, ILogger<AppInfoNatsHostedService> logger, AppInfo appInfo) : base(natsConnection, logger)
    {
        AppInfo = appInfo;
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        sub = await nats.SubscribeCoreAsync<int>("appinfo", "x");
        var reader = sub.Msgs;
        responder = Task.Run(async () =>
        {
            try
            {
                await foreach (var msg in reader.ReadAllAsync())
                {
                    //var name = msg.Subject.Split('.')[1];
                    Logger.LogInformation($"[REP] Received {msg.Subject}");
                    await msg.ReplyAsync($"AppInfo2: {AppInfo.Dump()}!");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"[REP exception]");
            }
        });
    }
}
