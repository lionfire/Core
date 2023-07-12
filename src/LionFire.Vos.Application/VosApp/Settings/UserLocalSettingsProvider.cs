using LionFire.Vos.VosApp;
using LionFire.Data.Gets;

namespace LionFire.Settings;

public class UserLocalSettingsProvider<T> : HostedGets<T>, IUserLocalSettings<T>
    where T : class
{
    protected override  ITask<IGetResult<T>> GetImpl(CancellationToken cancellationToken = default) => VosAppSettings.UserLocal<T>.H.GetOrInstantiateValue();
}
