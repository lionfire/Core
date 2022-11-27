using LionFire.Ontology;
using LionFire.FlexObjects;
using System;

namespace LionFire.Structures.Acquisition;

public static class Acquisitor<TNode, TValue>
    where TNode : IParented<TNode>
    where TValue : class
{
    static Acquisitor()
    {
        if (typeof(IFlex).IsAssignableFrom(typeof(TNode)))
        {
            GetValue = GetValue_Flex;
            SetValue = SetValue_Flex;
        }
        else
        {
            GetValue = GetValue_AcquisitorWeakTable;
            SetValue = SetValue_AcquisitorWeakTable;
        }
    }

    public static Func<TNode, TValue> GetValue { get; private set; }
    public static Action<TNode, TValue> SetValue { get; private set; }

    #region Implementation: Flex

    private static TValue GetValue_Flex(TNode o)
    {
        IFlex f = (IFlex)o;
        return f.Get<TValue>();
    }
    private static void SetValue_Flex(TNode o, TValue value)
    {
        IFlex f = (IFlex)o;
        f.Set(value);
    }

    #endregion

    #region Implementation: WeakTable

    private static TValue GetValue_AcquisitorWeakTable(TNode o)
    {
        if (AcquisitorWeakTable<TNode, TValue>.dict.TryGetValue(o, out var result))
        {
            return result;
        }
        return default;
    }

    private static void SetValue_AcquisitorWeakTable(TNode o, TValue value)
    {
#if NET6_0_OR_GREATER
        AcquisitorWeakTable<TNode, TValue>.dict.AddOrUpdate(o, value);
#else
        throw new NotSupportedException();
#endif
    }

    #endregion
}


