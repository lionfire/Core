using LionFire.Collections;
using LionFire.Referencing;

namespace LionFire.Persistence.Handles
{

    public interface C : RH<INotifyingReadOnlyCollection<object>> { }

    public interface C<T> : RH<INotifyingReadOnlyCollection<T>>
    {

    }
}
