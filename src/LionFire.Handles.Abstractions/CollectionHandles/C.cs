using LionFire.Collections;
using LionFire.Referencing;

namespace LionFire.Referencing
{

    public interface C : RH<INotifyingReadOnlyCollection<object>> { }

    public interface C<T> : RH<INotifyingReadOnlyCollection<T>>
    {

    }
}
