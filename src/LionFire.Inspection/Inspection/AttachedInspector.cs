
using LionFire.Data.Async.Gets;
using LionFire.Inspection.Nodes;
using ReactiveUI;
using System.Diagnostics;
using System.Security.AccessControl;

namespace LionFire.Inspection;

/// <summary>
/// Keeps Node.Groups in sync with Node.Source for a given Inspector.
/// </summary>
public class AttachedInspector : IDisposable
{
    #region Relationships

    public IInspector Inspector { get; }
    public IInspectedNode Node { get; }

    #endregion

    #region Lifecycle

    public AttachedInspector(IInspector inspector, IInspectedNode node)
    {
        ArgumentNullException.ThrowIfNull(inspector);
        ArgumentNullException.ThrowIfNull(node);

        Inspector = inspector;
        Node = node;

        node.WhenAnyValue(n => n.SourceType).Subscribe(OnSourceTypeChanged);
    }

    public void Dispose()
    {
        Node.Groups?.Edit(updater => {
            foreach (var group in updater.Items.Where(g => g.Inspector == Inspector).Select(g => g.Info.Key)) { updater.Remove(group); }            
        });
    }

    #endregion

    #region State

    /// <summary>
    /// If the Source changes to a type not assignable to this type, this AttachedInspector must be disposed and a fresh attachment made.
    /// </summary>
    public Type? AttachedSourceType { get; set; }
    public bool IsAttached => AttachedSourceType != null;

    #endregion

    private void OnSourceTypeChanged(Type sourceType)
    {
        if (IsAttached)
        {
            if (sourceType.IsAssignableTo(AttachedSourceType)) return;
        }

        // Add new groups from this Inspector
        Node.Groups.Edit(updater =>
        {
            var oldGroups = IsAttached ? updater.Items.Where(g => g.Inspector == Inspector).ToDictionary(g => g.Info.Key) : null;

            var newGroups = new List<IInspectorGroup>();

            foreach (var groupInfo in Inspector.GroupInfos.Values)
            {
                if (groupInfo.IsSourceTypeSupported(sourceType))
                {
                    if (oldGroups != null && oldGroups.Remove(groupInfo.Key)) continue; // Keep existing group
                    
                    var group = groupInfo.CreateNode(Node, Inspector);
                    Debug.Assert(group.Info.Key == groupInfo.Key);
                    newGroups.Add(group);
                }
            }

            if (oldGroups != null)
            {
                foreach (var group in oldGroups.Values) { updater.Remove(group); }
            }

            foreach (var group in newGroups)
            {
                if (updater.Lookup(group.Info.Key) != null) throw new AlreadyException($"Group with key {group.Info.Key} already exists");

                updater.AddOrUpdate(group);
            }
        });
        AttachedSourceType = sourceType;
    }
}
