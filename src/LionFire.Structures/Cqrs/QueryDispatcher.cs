using LionFire.ExtensionMethods;

namespace LionFire.Cqrs;

public class QueryDispatcher : IQueryDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    public QueryDispatcher(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

    public Task<TQueryResult> Dispatch<TQuery, TQueryResult>(TQuery query, CancellationToken cancellation)
    {
        var handler = _serviceProvider.GetRequiredService<IQueryHandler<TQuery, TQueryResult>>(typeof(ICommandHandler<TQuery, TQueryResult>));
        return handler.Handle(query, cancellation);
    }
}
