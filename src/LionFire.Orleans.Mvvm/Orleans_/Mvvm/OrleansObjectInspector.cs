using LionFire.Data.Async;
using LionFire.Data.Mvvm;
using LionFire.ExtensionMethods;
using MorseCode.ITask;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;

namespace LionFire.Inspection;


public class OrleansObjectInspector : IInspector
{
    public static bool IsTypeTaskWithValue(Type type) => type.IsGenericType
                && (type.GetGenericTypeDefinition() == typeof(Task<>)
                    //|| type.GetGenericTypeDefinition() == typeof(ITask<>)
                    || type.GetGenericTypeDefinition() == typeof(ValueTask<>)
                    );

    public static bool IsTypeTaskWithoutValue(Type type) => type.IsGenericType
                && (type.GetGenericTypeDefinition() == typeof(Task)
                    //|| type.GetGenericTypeDefinition() == typeof(ITask)
                    || type.GetGenericTypeDefinition() == typeof(ValueTask)
                    );

    /// <summary>
    /// (no validation)
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static Type UnwrapTaskValueType(Type type) => type.GetGenericArguments()[0];

    public ConcurrentDictionary<Type, CustomObjectInspectorInfo> Infos { get; } = new();

    public bool IsSupportedType(Type type)
        => type.FullName?.StartsWith("OrleansCodeGen.") == true && type.Name.StartsWith("Proxy_");

    public CustomObjectInspectorInfo? GetInfo(Type type)
    {
        if (!IsSupportedType(type)) return null;

        return Infos.GetOrAdd(type, type =>
        {
            var customObjectInspectorInfo = new CustomObjectInspectorInfo();

            Dictionary<string, MethodInfo> readProperties = type.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).Where(mi => mi.GetParameters().Length == 0 && IsTypeTaskWithValue(mi.ReturnType)).ToDictionary(mi => mi.Name, mi => mi);
            Dictionary<string, MethodInfo> writeProperties = type.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).Where(mi => mi.GetParameters().Length == 1 && IsTypeTaskWithoutValue(mi.ReturnType)).ToDictionary(mi => mi.Name, mi => mi);
            HashSet<string> readWriteProperties = readProperties.Keys.Where(k => writeProperties.ContainsKey(k) && UnwrapTaskValueType(readProperties[k].ReturnType) == writeProperties[k].GetParameters()[0].ParameterType).ToHashSet();

            #region Read

            foreach (var kvp in readProperties.Where(kvp => !readWriteProperties.Contains(kvp.Key)))
            {
                var name = kvp.Key;
                var readMethodInfo = kvp.Value;
                var valueType = readMethodInfo.ReturnType.GetGenericArguments()[0];

                //customObjectInspectorInfo.MemberInfos.Add(new CustomMemberInfo(readMethodInfo.Name, valueType, IO.IODirection.Read)
                //{
                //    CreateFunc = (obj, info) => typeof(FuncGets<>).MakeGenericType(valueType)
                //    {
                //        Getter = readMethodInfo,
                //    },
                //});
            }

            #endregion

            #region Write

            foreach (var kvp in writeProperties.Where(kvp => !readWriteProperties.Contains(kvp.Key)))
            {
                var name = kvp.Key;
                var writeMethodInfo = kvp.Value;
                var valueType = writeMethodInfo.GetParameters()[0].ParameterType;

                //customObjectInspectorInfo.MemberInfos.Add(new CustomMemberInfo(writeMethodInfo.Name, valueType, IO.IODirection.Read)
            }

            #endregion

            #region ReadWrite

            foreach (var name in readWriteProperties)
            {
                var writeMethodInfo = writeProperties[name];
                var readMethodInfo = readProperties[name];
                var valueType = readMethodInfo.ReturnType.GetGenericArguments()[0];
                var parameter = writeMethodInfo.GetParameters()[0];
                if (parameter.ParameterType != valueType) continue;


                throw new NotImplementedException();
                //customObjectInspectorInfo.MemberInfos.Add(new CustomMemberInfo(writeMethodInfo.Name, valueType, IO.IODirection.ReadWrite)
                //{                    
                //    CreateFunc = (obj, info) => Activator.CreateInstance(typeof(FuncValue<,>).MakeGenericType(typeof(object), valueType),)
                    
                //        //Getter = readMethodInfo,
                //        //Setter = writeMethodInfo,                    
                //});
            }

            #endregion

            return customObjectInspectorInfo;
        });
    }

    public IEnumerable<object> GetInspectedObjects(object obj)
    {
        if (obj == null) yield break;
        var type = obj.GetType();
        var customObjectInspectorInfo = GetInfo(type);
        if (customObjectInspectorInfo == null) yield break;


        var list = new List<IInspectorNode>();

        foreach (var memberInfo in customObjectInspectorInfo.MemberInfos)
        {
            //if(memberInfo.CanRead())
            IInspectorNode vm = memberInfo.Create(obj);
            list.Add(vm);
        }
        var customObjectInspector = new CustomObjectInspector(obj, list);

        yield return customObjectInspector;
    }
}
