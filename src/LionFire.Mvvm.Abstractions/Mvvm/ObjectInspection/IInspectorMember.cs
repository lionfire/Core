namespace LionFire.Inspection;

/// <summary>
/// 
/// </summary>
/// <remarks>
/// 
/// </remarks>
public interface IInspectorMember : IInspectorNode
{
    InspectorChildKind
    IInspectorMemberInfo? Info { get; }

//public interface IInspectorMemberVM<TSource> : IInspectorMemberVM // not needed since ObjectInspection primarily works at runtime without static typing
//{
//    TSource Source { get; }
//}
