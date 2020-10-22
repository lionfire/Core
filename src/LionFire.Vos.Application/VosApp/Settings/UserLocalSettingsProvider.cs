using LionFire.Vos.VosApp;
using MorseCode.ITask;
using LionFire.Resolves;

namespace LionFire.Settings
{
    public class UserLocalSettingsProvider<T> : HostedLazilyResolves<T>, IUserLocalSettings<T>
        where T : class
    {
        protected override  ITask<IResolveResult<T>> ResolveImpl() => VosAppSettings.UserLocal<T>.H.GetOrInstantiateValue();
    }
}
