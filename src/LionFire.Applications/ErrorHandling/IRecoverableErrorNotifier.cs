#if NBug
using NBug;
#endif

using System;

namespace LionFire.Applications.ErrorReporting
{
    // TODO

    public interface IRecoverableErrorNotifier
    {
        bool AskUserToContinueOnException(Exception exception);
    }
    public class RecoverableErrorNotifier : IRecoverableErrorNotifier
    {
        public virtual bool AskUserToContinueOnException(Exception exception)
        {
            return true;
        }
    }
}
