using LiveSharp;
using MediatR;

namespace LionFire.LiveSharp
{
    public sealed record UpdatedMethodNotification(IUpdatedMethod UpdatedMethod) : INotification;

    public sealed record UpdatedResourceNotification(string Path, string Content) : INotification;

}
