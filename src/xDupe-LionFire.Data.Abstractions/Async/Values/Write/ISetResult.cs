using LionFire.Results;

namespace LionFire.Data.Sets;

#if NotRecommended

/// <summary>
/// Not recommended over ITransferResult, since this doesn't add anything
/// </summary>
public interface ISetResult : ITransferResult { }

#endif