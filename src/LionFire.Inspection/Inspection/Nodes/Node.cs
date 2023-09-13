using LionFire.Inspection.ViewModels;
using LionFire.Overlays;
using LionFire.Structures.Keys;
using ReactiveUI;

namespace LionFire.Inspection.Nodes;

public abstract class Node<TInfo> : ReactiveObject, INode
    where TInfo : INodeInfo
{
    #region Identity

    public TInfo Info { get; }
    INodeInfo INode.Info => Info;

    public string Key { get; init; }

    #region Derived

    public IEnumerable<string> PathChunks => Parent?.PathChunks.Append(Key) ?? new[] { Key };

    #endregion

    #endregion

    #region Relationships

    public INode? Parent { get; }

    /// <summary>
    /// Mutable
    /// setting is ignored if SourceNode is set, in which case get always returns SourceNode.Source.
    /// </summary>
    public object? Source { get; init; }

    protected virtual void InitSource() => OnSourceChanged(Source);
    protected virtual void OnSourceChanged(object? source)
    {
        UpdateSourceType(source);
    }

    #endregion

    #region Parameters

    public InspectorContext? Context { get; set; }

    /// <summary>
    /// </summary>
    /// <remarks>
    /// ENH: Inheritance on a per-property basis
    /// </remarks>
    public IInspectorOptions InheritedOptions
        => OverlayX_NEW.NextNonNull<IInspectorOptions, INode>(this, n => n.Context?.Options, InspectorOptions.Default) ?? InspectorOptions.DefaultDefault;

    #endregion

    #region Lifecycle

    protected Node(INode? parent, object? source, TInfo info, string? key = null, InspectorContext? context = null)
    {
        Parent = parent;
        Source = source;
        Info = info;
        Context = context ?? parent?.Context;

        Key = key ?? (parent as IKeyProvider<string, INode>)?.GetKey(this) ?? "";

        InitSource();
    }

    protected Node(INode? parent, INode sourceNode, TInfo info, string? key = null, InspectorContext? context = null)
        : this(parent, source: sourceNode.Source, info, key, context) { }

    #endregion

    #region SourceType

    /// <summary>
    /// When setting, you must set to a type assignable from the Source type (if Source is not null)
    /// </summary>
    public Type SourceType
    {
        get => sourceType;
        set
        {
            if (Source != null && !Source.GetType().IsAssignableTo(value))
            {
                throw new ArgumentException($"{nameof(Source)} of type '{Source.GetType().FullName}' is not assignable to specified type '{value.FullName}'");
            }
            this.RaiseAndSetIfChanged(ref sourceType, value);
        }
    }
    private Type sourceType = typeof(DBNull);

    protected virtual void UpdateSourceType(object? newSource)
    {
        if (newSource != null)
        {
            if (!newSource.GetType().IsAssignableTo(SourceType))
            {
                SourceType = newSource.GetType();
            }
        }
    }

    #endregion

#if OLD // No children

    #region Groups

    public virtual SourceCache<InspectorGroup, string>? Groups => null;

    #endregion

    #region Children

    public virtual IObservable<bool?> HasChildren { get; } = Observable.Return((bool?)false);
    public virtual IObservableCache<INode, string>? Children => empty.AsObservableCache();
    private static readonly SourceCache<INode, string> empty = new SourceCache<INode, string>(n => n.Key);
    
    #endregion

    #region IKeyProvider

    public abstract string GetKey(INode? node);

    #endregion
#endif
}
