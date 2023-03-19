
namespace LionFire.Cqrs;

// Based on: https://cezarypiatek.github.io/post/why-i-dont-use-mediatr-for-cqrs/

public interface IQueryHandler<in TQuery, TQueryResult>
{
    Task<TQueryResult> Handle(TQuery query, CancellationToken cancellation);
}
