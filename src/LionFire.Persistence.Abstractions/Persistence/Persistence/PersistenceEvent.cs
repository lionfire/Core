using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Persistence
{
    public struct PersistenceEvent
    {
        public PersistenceContext Context { get; set; }
        public PersistenceEventKind Kind { get; set; }
        public PersistenceEventSourceKind SourceKind { get; set; }
    }
}
