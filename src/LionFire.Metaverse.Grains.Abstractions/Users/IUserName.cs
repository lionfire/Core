using LionFire.Orleans_;

using System.Threading.Tasks;

namespace LionFire.Metaverse.Users;


/// <summary>
/// Reverse-lookup.  Primary source of truth: IUser, but they should always be in sync.
/// </summary>
public interface IUserName : Orleans.IGrainWithStringKey
{
    Task<SecuredResponse<string?>> UserId();
}
