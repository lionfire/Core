using System.Threading.Tasks;

namespace LionFire.Metaverse.Users;


public interface IUserNameJournal : Orleans.IGrainWithStringKey
{
    Task<IEnumerable<UserNameChange>> GetNameChanges();
    Task<SecuredResponse<object>> RecordNameChange(UserNameChange c);
}
