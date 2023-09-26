using LionFire.IO;

namespace LionFire.Inspection;

//public class CustomMemberInfo<TState> : CustomMemberInfo
//{
//    public Func<object, CustomMemberInfo, IMemberVM<TState>> CreateFunc { get; set; } = (obj, mi) => throw new InvalidOperationException();
//    public IMemberVM<TState> Create(object obj) => CreateFunc(obj, this);
//}

//public class CustomNodeInfo : NodeInfoBase
//{
//    public CustomNodeInfo(string name, Type type, IODirection ioDirection)
//    {
//        Name = name;
//        Type = type;
//        IODirection = ioDirection;
//    }

//    public Func<object, CustomNodeInfo, INode> CreateFunc { get; set; } = (obj, mi) => throw new InvalidOperationException();

//    public override InspectorNodeKind NodeKind => throw new NotImplementedException();

//    //public override INode Create(object obj) => CreateFunc(obj,this);
//}
