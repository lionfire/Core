using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Blazor;

public class ComponentParameters<TConcrete>
{
    public IDictionary<string, object?> ToParameterDictionary()
    {
        var d = new Dictionary<string, object?>();
        foreach (var pi in typeof(TConcrete).GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public))
        {
            d.Add(pi.Name, pi.GetValue(this));
        }
        return d;
    }
    public static IDictionary<string, object?> ToParameterDictionary(TConcrete obj)
    {
        var d = new Dictionary<string, object?>();
        foreach (var pi in typeof(TConcrete).GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public))
        {
            d.Add(pi.Name, pi.GetValue(obj));
        }
        return d;
    }
}
