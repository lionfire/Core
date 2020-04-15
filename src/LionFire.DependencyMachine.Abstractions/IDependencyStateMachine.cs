using System;

namespace LionFire.DependencyMachine
{
    public interface IDependencyStateMachine 
    {
        IServiceProvider ServiceProvider { get; set; }

        void Register(IReactor reactor, bool isAlreadyStarted = false);
        bool Unregister(IReactor reactor);
    }

}