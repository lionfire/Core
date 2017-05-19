using LionFire.Instantiating;

namespace LionFire.Messaging.Queues
{

    public enum QueueSupervisorOptions
    {
        None = 0,

        RestorePartiallyHandledMessages = 1 << 1,
        FaultPartiallyHandledMessages = 1 << 2,
    }
    public class TQueueSupervisorOptions : ITemplate<TQueueSupervisorOptions>
    {
        public QueueSupervisorOptions Options { get; set; }
    }
    public class QueueSupervisor : ITemplateInstance<TQueueSupervisorOptions>
    {
        public TQueueSupervisorOptions Template { get; set; }
        ITemplate ITemplateInstance.Template { get => Template; set => Template = (TQueueSupervisorOptions)value; }
    }
}
