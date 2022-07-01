#if CQRS

using LionFire.Cqrs;
using LionFire.Vos.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LionFire.Vos.Persisters.Cqrs
{
    public class VosQueryDispatcher : IQueryDispatcher
    {
        private readonly IServiceProvider serviceProvider;

        public VosQueryDispatcher(IServiceProvider serviceProvider) => this.serviceProvider = serviceProvider;

        public Task<TCommandResult> Dispatch<TCommand, TCommandResult>(TCommand command, CancellationToken cancellation)
        {
            var handler = serviceProvider.GetRequiredService<ICommandHandler<TCommand, TCommandResult>>();
            return handler.Handle(command, cancellation);
        }
    }
}
#endif