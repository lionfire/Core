using System;

namespace LionFire.Shell
{
    public interface IRecoverableErrorShell
    {
        bool AskUserToContinueOnException(Exception exception);
    }
}
