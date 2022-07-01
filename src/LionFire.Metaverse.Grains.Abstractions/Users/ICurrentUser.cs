using LionFire.Orleans_;

namespace LionFire.Metaverse.Users;

public interface ICurrentUser : Orleans.IGrainWithStringKey
{
    Task<SecuredResponse<string?>> UserName();
    Task<SecuredResponse<string?>> Subject();
}
