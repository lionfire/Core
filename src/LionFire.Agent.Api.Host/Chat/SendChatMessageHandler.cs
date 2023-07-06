using LionFire.Agent.Api.Host.Chat;
using Marten;
using Wolverine;

namespace LionFire.Chat;

public readonly record struct SendChatMessage(string Message, Guid? ChatSession = null);
public readonly record struct ChatMessageAccepted(Guid ChatSession);



public static class SendChatMessageHandler
{
    public static async Task<ChatMessageAccepted> Handle(SendChatMessage request, IMessageContext bus, IDocumentStore store, CancellationToken cancellationToken = default)
    {
        Guid guid;

        if (request.ChatSession.HasValue)
        {

            using var session = store.LightweightSession();
            var chatSession = await session.LoadAsync<ChatSession>(request.ChatSession.Value, cancellationToken).ConfigureAwait(false);
            if (chatSession == null) throw new ArgumentException("ChatSession not found");
            else guid = request.ChatSession.Value;
        }
        else
        {
            guid = await CreateChatSessionHandler.CreateSession(store, cancellationToken).ConfigureAwait(false);
        }

        //if (request.ChatSession == null)
        //{
        //    var guid = 

        //    var guid = await bus..InvokeAsync<CreateChatSession, ChatSessionCreated>(new CreateChatSession(), cancellationToken);
        //    // New Session
        //    throw new ArgumentException("ChatSession must be specified");
        //}



        //    //var new ChatMessageEnvelope(request.Message, request.ChatSession ?? , "TODO: Sender");

        //    var order = new Order
        //    {
        //        Description = command.Description
        //    };

        //    // Register the new document with Marten
        //    session.Store(order);

        //    // Hold on though, this message isn't actually sent
        //    // until the Marten session is committed
        //    await outbox.SendAsync(new OrderCreated(order.Id));

        //    // This makes the database commits, *then* flushed the
        //    // previously registered messages to Wolverine's sending
        //    // agents
        //    await session.SaveChangesAsync(cancellation);

        var chatMessage = new ChatMessage(request.Message);
        await bus.PublishAsync(chatMessage, new DeliveryOptions { }).ConfigureAwait(false);

        return new ChatMessageAccepted(guid);

    }
}
