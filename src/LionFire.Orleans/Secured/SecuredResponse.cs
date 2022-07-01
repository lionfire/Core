// Retrieved on 220613 from: Apache license
// https://github.com/MV10/OrleansJWT

namespace LionFire.Orleans_;

public class SecuredResponse<TResult>
    : ISecuredResponseValidation
{
    public SecuredResponse() { }
    public SecuredResponse(TResult? result) { Result = result; }

    public static readonly SecuredResponse<TResult> Default = new(default);

    public TResult? Result { get; set; }
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;

    public static implicit operator SecuredResponse<TResult>(TResult result) => new SecuredResponse<TResult> { Result = result };
}
