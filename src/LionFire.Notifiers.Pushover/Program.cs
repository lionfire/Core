using LionFire.Notifications;
using NATS.Client.Core;
using Newtonsoft.Json;

await using var nats = new NatsConnection();

Console.WriteLine("Hello, World!");

//Task.Run(async () =>
{
    var subject = "notifiers.pushover.outbox";
    subject = "alerts"; // TEMP

    while (true)
    {
        var sub = await nats.SubscribeCoreAsync<byte[]>(subject);
        while (await sub.Msgs.WaitToReadAsync())
        {
            if (sub.Msgs.TryRead(out var msg))
            {
                var alert = JsonConvert.DeserializeObject<Alert>(System.Text.Encoding.UTF8.GetString(msg.Data ?? []));
                Console.WriteLine("[ALERT] read Alert from NATS " + alert);
            }
            else
            {
                Console.WriteLine("[ALERT] failed to read Alert from NATS");

            }
        }
        Console.Write(".");
    }
    //sub.MessageHandler = (sender, args) =>
    //{
    //    var json = System.Text.Encoding.UTF8.GetString(args.Message.Data);
    //    Console.WriteLine("[ALERT] " + json);
    //    return Task.CompletedTask;
    //};
    //await foreach (var msg in await nats.SubscribeAsync("notifiers.pushover.outbox", "default")) ;

    //, (sender, args) =>
    //{
    //    var json = System.Text.Encoding.UTF8.GetString(args.Message.Data);
    //    Console.WriteLine("[ALERT] " + json);
    //    return Task.CompletedTask;
    //});
    //while (true)
    //{
    //    //await Task.Delay(1000);
    //    //await nats.PublishAsync("notifiers.pushover.inbox", "Hello, World!");
    //}
}
//);

