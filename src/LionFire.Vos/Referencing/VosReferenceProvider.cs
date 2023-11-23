using LionFire.Referencing;
using System.Collections.Generic;
using System.IO;

namespace LionFire.Vos;

public class VobReferenceProvider : ReferenceProviderBase<VobReference>
{
    public override string UriScheme => "vos";

    public override (VobReference reference, string error) TryGetReference(string path)
        => (new VobReference(path), null);
}

public class TypedVobReferenceProvider : TypedReferenceProviderBase<VobReference<object>>
{
    public override string UriScheme => "vos";

    public override (TReference result, string? error) TryGetReference<TReference>(string uri, bool aggregateErrors = false)
    {
        if (!typeof(TReference).IsGenericType || typeof(TReference).GetGenericTypeDefinition() != typeof(IReference<>))
        {
            return (default, $"{nameof(TReference)} must be IReference<TValue>");
        }

        var valueType = typeof(TReference).GetGenericArguments().Single();

        var referenceType = typeof(VobReference<>).MakeGenericType(valueType);

        var uriObj = new Uri(uri);
        var path = uriObj.AbsolutePath;

        var ctor = typeof(VobReference<>).MakeGenericType(valueType).GetConstructor(new Type[] { typeof(string) });
        return ((TReference)ctor.Invoke(new object[] { path }),null);

        //return ((TReference)Activator.CreateInstance(referenceType, uriObj.AbsolutePath), null);

    }
}
