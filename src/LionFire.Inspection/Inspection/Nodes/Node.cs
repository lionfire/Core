using LionFire.Inspection.ViewModels;
using LionFire.Overlays;
using LionFire.Structures.Keys;
using ReactiveUI;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Reactive.Linq;
using System.Reactive.Subjects;
//using static LionFire.Inspection.Nodes.AsyncValueNode<TInfo>;

namespace LionFire.Inspection.Nodes;

#if false // Am I recreating IGetter and AsyncValue?
public interface IValueNode { }
/// <summary>
/// A node whose primary object of interest is a Value.
/// This Value is typically pushed into here via some mechanism that reads from Source.
/// </summary>
/// <typeparam name="TInfo"></typeparam>
public abstract class ValueNode<TInfo> : Node<TInfo>
    where TInfo : INodeInfo
{

    [Reactive]
    public override object? Value { get; set; }
    //public override object? Value { get => this.value.Value; set => this.value.OnNext(value); }
    //protected BehaviorSubject<object?> value = new(null);
}
#endif


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

    public IReactiveNotifyPropertyChanged<INode> SourceChangeEvents => RaiseSourceChangeEvents;
    public ReactiveNotifyPropertyChanged<INode> RaiseSourceChangeEvents { get; } = new();


    protected virtual void InitSource() => OnSourceChanged(Source);
    protected virtual void OnSourceChanged(object? source)
    {
        UpdateSourceType(source);
    }
    protected virtual void InitValue() => OnValueChanged(Value);
    protected virtual void OnValueChanged(object? newValue)
    {
        Debug.WriteLine($"{Key} Value = {newValue}");
        UpdateValueType(newValue);
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
    private Type sourceType = InspectorConstants.NullType;

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

    /// <summary>
    /// When setting, you must set to a type assignable from the Source type (if Source is not null)
    /// </summary>
    public Type ValueType
    {
        get => valueType;
        set
        {
            if (Value != null && !Value.GetType().IsAssignableTo(value))
            {
                throw new ArgumentException($"{nameof(Value)} of type '{Value.GetType().FullName}' is not assignable to specified type '{value.FullName}'");
            }
            this.RaiseAndSetIfChanged(ref valueType, value);
        }
    }
    private Type valueType = InspectorConstants.NullType;
    protected virtual void UpdateValueType(object? newValue)
    {
        if (newValue != null)
        {
            if (!newValue.GetType().IsAssignableTo(SourceType))
            {
                ValueType = newValue.GetType();
            }
        }
    }

    #endregion

    #region State


    /// <summary>
    /// Derived classes may wire this up to an IGetter or AsyncValue
    /// </summary>
    public virtual object? Value { get => Source; set => throw new NotSupportedException(); }
    public virtual IObservable<object?> Values => Observable.Return(Value);

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
