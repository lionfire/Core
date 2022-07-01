

namespace LionFire.Metaverse.Users
{
    public interface INameChangePolicy : IGrainWithStringKey
    {
        Task<NameChangePolicy> GetNameChangePolicy();

        //[PermissionRequired("ChangeNamePolicy")] // TODO TOSECURITY
        Task Set(NameChangePolicy newValue);
    }
}
