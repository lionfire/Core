#nullable enable

using LionFire.Metaverse.Users;
using System.Threading.Tasks;

namespace LionFire.Metaverse.Users;

public interface IUserNameChange : Orleans.IGrainWithStringKey
{
    Task<SecuredResponse<NameChangeResult>> ChangeName(string userId, string newName, string changedBy, NameChangeReason reason, string? reasonDetail = null, string? comment = null, bool force = false);
}
