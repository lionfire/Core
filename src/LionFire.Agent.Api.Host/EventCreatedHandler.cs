//namespace LionFire.Agent.Api.Host;

public record EventCreated(Guid Guid);


public class EventCreatedHandler
{
    public EventCreatedHandler(ILogger<EventCreatedHandler> logger)
    {
        Logger = logger;
    }
    public EventCreatedHandler()
    {
    }

    public ILogger? Logger { get; }

    public Task Handle(EventCreated command)
    {
        Logger?.LogInformation("EventCreatedHandler: " + command.Guid);
        return Task.CompletedTask;

    }
}