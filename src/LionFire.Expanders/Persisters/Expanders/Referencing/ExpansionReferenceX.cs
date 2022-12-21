using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Persisters.Expanders;

public static class ExpansionReferenceX
{
    public static ExpansionReference<T> ToExpansionReference<T>(this string uri)
        => ExpansionReference<T>.Parse(uri);
    public static ExpansionReference ToExpansionReference(this string uri)
        => ExpansionReference.Parse(uri);
}
