namespace LionFire.Persisters.Expanders;

public interface IValidatingReadHandle
{
    Task<(bool IsValid, string? FailReason)> IsValid(ValidityCheckDetail validityCheckDetail = ValidityCheckDetail.Unspecified);
}
