// Retrieved on 220613 from: Apache license
// https://github.com/MV10/OrleansJWT

namespace LionFire.Orleans_;

public interface ISecuredResponseValidation
{
    bool Success { get; set; }
    string Message { get; set; }
}
