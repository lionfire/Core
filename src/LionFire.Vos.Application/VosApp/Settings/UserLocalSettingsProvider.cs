using LionFire.Vos.VosApp;
using MorseCode.ITask;
using LionFire.Data.Gets;

namespace LionFire.Settings;

public class UserLocalSettingsProvider<T> : HostedGets<T>, IUserLocalSettings<T>
    where T : class
{
    protected override  ITask<IGetResult<T>> GetImpl() => VosAppSettings.UserLocal<T>.H.GetOrInstantiateValue();
}
