using System;
using LionFire.Ontology;

namespace Dycen;

[NotifyOnCreate]
public class DeletionInfo
{

    public DateTimeOffset CreationTime { get; set; }

    [Mutable]
    public bool Undeleted { get; set; }
}
