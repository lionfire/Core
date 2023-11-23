// Retrieved on 220613 from: Apache license
// https://github.com/MV10/OrleansJWT

using Orleans;

namespace LionFire.Orleans_;

[GenerateSerializer]
public class SecuredResponse<TResult>
    : ISecuredResponseValidation
{
    public SecuredResponse() { }
    public SecuredResponse(TResult? result) { Result = result; }

    public static readonly SecuredResponse<TResult> Default = new(default);

    [Id(0)]
    public TResult? Result { get; set; }
    [Id(1)]
    public bool Success { get; set; }
    [Id(2)]

    public string Message { get; set; } = string.Empty;
    [Id(3)]

    public int? StatusCode { get; set; }

    public static implicit operator SecuredResponse<TResult>(TResult result) => new SecuredResponse<TResult> { Result = result };

    #region Default responses

    public static SecuredResponse<TResult> Unauthenticated { get; } = new SecuredResponse<TResult>
    {
        Success = false,
        Message = "Unauthenticated",
        StatusCode = 401,
    };
    public static SecuredResponse<TResult> Unauthorized { get; } = new SecuredResponse<TResult>
    {
        Success = false,
        Message = "Unauthorized",
        StatusCode = 403,
    };

    #endregion
}
