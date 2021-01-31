using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LionFire.LiveSharp
{
    public class LiveSharpNotifier
        : INotificationHandler<UpdatedMethodNotification>
        , INotificationHandler<UpdatedResourceNotification>
    {
        public event Action<UpdatedMethodNotification> MethodChanged;
        Task INotificationHandler<UpdatedMethodNotification>.Handle(UpdatedMethodNotification notification, CancellationToken cancellationToken)
        {
            MethodChanged?.Invoke(notification);
            return Task.CompletedTask;
        }


        public event Action<UpdatedResourceNotification> ResourceChanged;
        
        Task INotificationHandler<UpdatedResourceNotification>.Handle(UpdatedResourceNotification notification, CancellationToken cancellationToken)
        {
            ResourceChanged?.Invoke(notification);
            return Task.CompletedTask;
        }

    }

    
}
