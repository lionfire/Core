//namespace LionFire.Agent.Api.Host;

public record CreateEvent(string Source, string Topic, string Title, string Description);



public class CreateEventHandler
{
    public CreateEventHandler()
    {

    }

    public EventCreated Handle(CreateEvent command)
    {
        Guid guid = Guid.NewGuid();
        return new EventCreated(guid);
    }
}
