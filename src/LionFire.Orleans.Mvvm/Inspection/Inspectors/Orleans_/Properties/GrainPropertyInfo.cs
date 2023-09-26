using LionFire.Inspection.Nodes;
using System.Reflection;
using LionFire.IO;
using LionFire.ExtensionMethods;

namespace LionFire.Inspection;

public class GrainPropertyInfo : NodeInfoBase
{
    public GrainPropertyInfo(string name, MethodInfo? getMethod, MethodInfo? setMethod)
    {
        if (getMethod == null && setMethod == null) throw new ArgumentException("At least one of these must not be null: getMethod and setMethod");
        Name = name; //  (getMethod ?? setMethod)!.Name;
        Type = (getMethod?.ReturnType ?? setMethod!.GetParameters()[0].ParameterType).UnwrapTaskType();
        IODirection = getMethod != null
            ? setMethod == null
                ? IODirection.Read
                : IODirection.ReadWrite
            : IODirection.Write;
        GetMethod = getMethod;
        SetMethod = setMethod;
    }

    public MethodInfo? GetMethod { get; }
    public MethodInfo? SetMethod { get; }

    public override bool IsAsync => true;
    public override InspectorNodeKind NodeKind => InspectorNodeKind.Data;
}
