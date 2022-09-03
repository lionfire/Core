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
