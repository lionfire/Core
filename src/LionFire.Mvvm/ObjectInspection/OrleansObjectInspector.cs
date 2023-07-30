using MorseCode.ITask;
using System.Collections.Concurrent;
using System.Reflection;

namespace LionFire.Mvvm.ObjectInspection;


public class OrleansObjectInspector : IObjectInspector
{
    public static bool IsTaskValueType(Type type) => type.IsGenericType
                && (type.GetGenericTypeDefinition() == typeof(Task<>)
                    //|| type.GetGenericTypeDefinition() == typeof(ITask<>)
                    || type.GetGenericTypeDefinition() == typeof(ValueTask<>)
                    );

    public ConcurrentDictionary<Type, CustomObjectInspectorInfo> Infos { get; } = new();

    public bool IsSupportedType(Type type)
        => type.FullName?.StartsWith("OrleansCodeGen.") == true && type.Name.StartsWith("Proxy_");

    public CustomObjectInspectorInfo? GetInfo(Type type)
    {
        if (!IsSupportedType(type)) return null;

        return Infos.GetOrAdd(type, type =>
        {
            var customObjectInspectorInfo = new CustomObjectInspectorInfo();

            Dictionary<string, MethodInfo> readProperties = type.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).Where(mi => mi.GetParameters().Length == 0 && IsTaskValueType(mi.ReturnType)).ToDictionary(mi => mi.Name, mi => mi);

            foreach (var mi in type.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).Where(mi => mi.GetParameters().Length == 1 && readProperties.ContainsKey(mi.Name)))
            {
                var readMethodInfo = readProperties[mi.Name];
                var valueType = readMethodInfo.ReturnType.GetGenericArguments()[0];
                var parameter = mi.GetParameters()[0];
                if (parameter.ParameterType != valueType) continue;

                customObjectInspectorInfo.MemberInfoVMs.Add(new AsyncPropertyInfoVM(mi.Name, mi.ReturnType)
                {
                    Getter = readMethodInfo,
                    Setter = mi,
                });
            }

            return customObjectInspectorInfo;
        });
    }

    public IEnumerable<object> GetInspectedObjects(object obj)
    {
        if (obj == null) yield break;
        var type = obj.GetType();
        var customObjectInspectorInfo = GetInfo(type);

        var customObjectInspector = new CustomObjectInspector(obj, customObjectInspectorInfo);

        foreach (var x in customObjectInspectorInfo.MemberInfoVMs)
        {
            if (x.CanRead)
            {
                if (x.CanWrite)
                {
                    new DataMemberVM
                }
                else
                {

                }
            }
            else if (x.CanWrite)
            {
                throw new NotImplementedException("Write only");
            }
        }
        yield return customObjectInspector;
    }
}

public class CustomObjectInspectorInfo
{
    public List<IMemberInfoVM> MemberInfoVMs { get; set; }
}

public interface ICustomObjectInspector
{
    List<MemberVM> MemberVMs { get; }
    object SourceObject { get; }
}

public class CustomObjectInspector : ICustomObjectInspector
{
    public CustomObjectInspector(object sourceObject, CustomObjectInspectorInfo customObjectInspectorInfo)
    {
        SourceObject = sourceObject;
        Info = customObjectInspectorInfo;
    }

    public List<MemberVM> MemberVMs { get; } = new();
    public object SourceObject { get; }
    public CustomObjectInspectorInfo Info { get; }
}