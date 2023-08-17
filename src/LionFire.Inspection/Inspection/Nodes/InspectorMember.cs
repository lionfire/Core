using LionFire.Data.Async.Gets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Inspection;

/// <summary>
/// 
/// </summary>
/// <remarks>
/// 
/// </remarks>
public interface IInspectorMember //: INode
{
    IInspectorMemberInfo? Info { get; }

    IGetter<object>? Getter { get; }
    ISetter<object>? Setter { get; }
}

public class InspectorMember : IInspectorMember
{
}
