using LionFire.IO;

namespace LionFire.Inspection;

//public class CustomMemberInfo<TState> : CustomMemberInfo
//{
//    public Func<object, CustomMemberInfo, IMemberVM<TState>> CreateFunc { get; set; } = (obj, mi) => throw new InvalidOperationException();
//    public IMemberVM<TState> Create(object obj) => CreateFunc(obj, this);
//}

public class CustomMemberInfo : InspectorMemberInfo
{
    public CustomMemberInfo(string name, Type type, IODirection ioDirection)
    {
        Name = name;
        Type = type;
        IODirection = ioDirection;
    }

    public override string Name { get; }
    public override Type Type { get; }

    public override IODirection IODirection { get; }

    public Func<object, CustomMemberInfo, INode> CreateFunc { get; set; } = (obj, mi) => throw new InvalidOperationException();

    public override INode Create(object obj) => CreateFunc(obj,this);
}
