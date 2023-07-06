using Marten;
using NodaTime;
using System.Threading;

namespace LionFire.Agent.Api.Host.Chat;

public readonly record struct CreateChatSession();
public readonly record struct ChatSessionCreated(Guid Guid);

public static class CreateChatSessionHandler
{
    public static async Task<Guid> CreateSession(IDocumentStore store, CancellationToken cancellationToken = default)
    {
        using var session = store.LightweightSession();
        var chatSession = new ChatSession() { CreationDate = SystemClock.Instance.GetCurrentInstant() };

        session.Insert(chatSession);
        await session.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return chatSession.Id;
    }
    public static async Task<ChatSessionCreated> Handle(CreateChatSession _, IDocumentStore store, CancellationToken cancellationToken = default)
    {
        return new ChatSessionCreated(await CreateSession(store).ConfigureAwait(false));
    }
    
}
