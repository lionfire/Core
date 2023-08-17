using LionFire.IO;
using System.Reflection;

namespace LionFire.Inspection;

public class ReflectionEventInfo : ReflectionMemberInfo
{
    public override IODirection IODirection => IODirection.Read;

    public ReflectionEventInfo(EventInfo EventInfo) : base(EventInfo)
    {
    }

    public EventInfo EventInfo { get => (EventInfo)MemberInfo; set => MemberInfo = value; }
    public override INode Create(object obj) => new EventVM(this, obj);
}


