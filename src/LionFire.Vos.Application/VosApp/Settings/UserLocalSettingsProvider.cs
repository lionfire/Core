using LionFire.Vos.VosApp;
using MorseCode.ITask;
using LionFire.Data.Async.Gets;

namespace LionFire.Settings
{
    public class UserLocalSettingsProvider<T> : HostedLazilyResolves<T>, IUserLocalSettings<T>
        where T : class
    {
        protected override  ITask<IGetResult<T>> ResolveImpl() => VosAppSettings.UserLocal<T>.H.GetOrInstantiateValue();
    }
}
