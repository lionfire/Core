using LionFire.Results;

namespace LionFire.Data.Async.Sets;

#if NotRecommended

/// <summary>
/// Not recommended over ITransferResult, since this doesn't add anything
/// </summary>
public interface ISetResult : ITransferResult { }

#endif