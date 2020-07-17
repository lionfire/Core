#define ConcurrentHandles
#define WARN_VOB
//#define INFO_VOB
#define TRACE_VOB

using LionFire.Referencing;

namespace LionFire.Vos
{
    
    public class VobSaveEvent : EventBase
    {
        public VobSaveEvent(VobReference logicalReference, IReference targetReference)
        {
            //VobEventType eventType = VobEventType.Save;
            this.LogicalReference = logicalReference;
            this.TargetReference = targetReference;
        }

        #region EventType

        public VobEventType EventType {
            get { return VobEventType.Save; }
        }
        //    get { return eventType; }
        //    set { eventType = value; }
        //} private VobEventType eventType;

        #endregion

        #region Reference

        public VobReference LogicalReference {
            get { return logicalReference; }
            set {
                if (logicalReference == value) return;
                if (logicalReference != default(VobReference)) throw new AlreadySetException();
                logicalReference = value;
            }
        }
        private VobReference logicalReference;

        #endregion


        #region TargetReference

        public IReference TargetReference {
            get { return targetReference; }
            set {
                if (targetReference == value) return;
                if (targetReference != default(IReference)) throw new AlreadySetException();
                targetReference = value;
            }
        }
        private IReference targetReference;

        #endregion


        public override string ToString()
        {
            return (IsBefore ? "Saving " : "Saved ") + logicalReference + " to " + targetReference;
        }
    }


}
