using LionFire.FlexObjects;
using LionFire.Ontology;
using LionFire.Structures;
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.ExtensionMethods.Objects;

public static class ObjectAsTypeX
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <param name="result"></param>
    /// <param name="allowUnwrapDefault">If an object has an interface specifically for wrapping/containing T, return that interface's value even if default (i.e. null) according to EqualityComparer&lt;T&gt;.Default</param>
    /// <returns></returns>
    public static bool TryUnwrapAs<T>(this object obj, out T result, bool allowUnwrapDefault = false)
    {
        if (obj is T t) { result = t; return true; }

        if (obj is IReadWrapper<T> w && (allowUnwrapDefault || EqualityComparer<T>.Default.Equals(w.Value, default)))
        {
            result = w.Value; return true;
        }

        if (obj is IHas<T> has && (allowUnwrapDefault || EqualityComparer<T>.Default.Equals(has.Object, default)))
        {
            result = has.Object; return true;
        }

        if (obj is IFlex flex)
        {
            if (flex.Query<T>(out result)) { return true; }
        }

        result = default;
        return false;
    }
}
