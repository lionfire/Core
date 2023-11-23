#nullable enable

namespace LionFire.Data.Async.Sets;

public interface IStagesSet<T> : IWriteStagesSet<T> //: IReadStagesSet<T>, 
{
    T? StagedValue { get; set; }
}

//public interface IReadStagesSet<out T> : ISetter
//{
//    T? ReadStagedValue { get; }    
//}

public class StagesSetWriter
{

    public static IEnumerable<Type> GetStagesSetTypes(ISetter s) => s.GetType().GetInterfaces().Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IStagesSet<>)).Select(i => i.GetGenericArguments().First());
    public static IEnumerable<Type> GetSetterTypes(ISetter s) => s.GetType().GetInterfaces().Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ISetter<>)).Select(i => i.GetGenericArguments().First());
    public static IEnumerable<Type> GetNonstagingSetterTypes(ISetter s) => GetSetterTypes(s).Where(t => !GetStagesSetTypes(s).Contains(t));
}

public interface IWriteStagesSet : ISetter
{
    void DiscardStagedValue();
}

public interface IWriteStagesSet<in T> : IWriteStagesSet
{
    //void StageValue(T value);

    //    T? StagedValue { set; }
    bool HasStagedValue { get; set; }
}

public interface IStagesSetWithPersistenceFlags<T> : IStagesSet<T>
{
    PersistenceFlags Flags { get; set; }
}