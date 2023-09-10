namespace LionFire.Inspection.Nodes;

/// <summary>
/// Groups are writable by IInspectors, and are the intended source for effective Children 
/// </summary>
/// <remarks>
/// Writable accessor:
///  - Groups, for IInspectors to add IInspectorGroups for inspecting the Source
/// </remarks>
public interface IInspectedNode : IHierarchicalNode
{
    /// <summary>
    /// A writable blackboard for adding and removing InspectorGroups that are useful for inspecting the Source.
    /// Recommendation:
    ///  - Use ObjectInspectorService.Attach to automatically manage this.  It will use all available IInspectors, and you can create and register your own.
    /// </summary>
    SourceCache<IInspectorGroup, string> Groups { get; }
}

