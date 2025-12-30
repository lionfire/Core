using LionFire.StateMachines;

namespace LionFire.Hosting;

public enum HostedServiceTransitions
{
    [Transition(null, HostedServiceState.Stopped)]
    Initialize,
    [Transition(HostedServiceState.Stopped, HostedServiceState.Starting)]
    Start,
    [Transition(HostedServiceState.Starting, HostedServiceState.Running)]
    Started,
    [Transition(HostedServiceState.Running, HostedServiceState.Stopping)]
    Stop,
    [Transition(HostedServiceState.Stopping, HostedServiceState.Stopped)]
    Stopped,
}
