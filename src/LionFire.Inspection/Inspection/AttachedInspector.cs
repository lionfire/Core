
using LionFire.Data.Async.Gets;
using LionFire.Inspection.Nodes;
using LionFire.Threading;
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

        //node.WhenAnyValue(n => n.SourceType).Subscribe(OnSourceTypeChanged);
        node.WhenAnyValue(n => n.ValueType).Subscribe(OnValueTypeChanged);
        node.WhenAnyValue(n => n.Value).Subscribe(OnValueChanged);
        //node.WhenAnyValue(n => n.Value).Subscribe(v =>
        //{
        //    //if(v != null) { OnSourceTypeChanged(v.GetType()); }
        //    //foreach (var group in node.Groups.KeyValues)
        //    //{
        //    //    foreach (var groupChild in group.Value.Children.ReadCacheValue ?? Enumerable.Empty<KeyValuePair<string, INode>>())
        //    //    {
        //    //        groupChild.Value.Disc.DiscardValue();
        //    //    }
        //    //    group.Value.Children.DiscardValue();

        //    //    group.Value.Children.Get().AsTask().FireAndForget();// TEMP
        //    //}
        //});
    }

    public void Dispose()
    {
        Node.Groups?.Edit(updater =>
        {
            foreach (var group in updater.Items.Where(g => g.Inspector == Inspector).Select(g => g.Info.Key)) { updater.Remove(group); }
        });
    }

    #endregion

    #region State

    /// <summary>
    /// If the Source changes to a type not assignable to this type, this AttachedInspector must be disposed and a fresh attachment made.
    /// </summary>
    public Type? AttachedValueType { get; set; }
    public bool IsAttached => AttachedValueType != null;

    #endregion

    private void OnValueChanged(object? value)
    {
        if (value != null)
        {
            OnValueTypeChanged(value.GetType());
        }
    }
    private void OnValueTypeChanged(Type valueType)
    {
        //if (IsAttached)
        //{
        //    if (valueType.IsAssignableTo(AttachedValueType)) return;
        //}

        // Add new groups from this Inspector
        Node.Groups.Edit(updater =>
        {
            var oldGroups = IsAttached ? updater.Items.Where(g => g.Inspector == Inspector).ToDictionary(g => g.Info.Key) : null;

            var newGroups = new List<IInspectorGroup>();

            foreach (var groupInfo in Inspector.GroupInfos.Values)
            {
                if (valueType != typeof(NullType) && groupInfo.IsTypeSupported(valueType))
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
        AttachedValueType = valueType;
    }
}
