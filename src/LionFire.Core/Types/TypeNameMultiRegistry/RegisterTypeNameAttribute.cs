using System.Linq;
using System.Reflection;

namespace LionFire.Types;

[System.AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, Inherited = false, AllowMultiple = true)]
public sealed class RegisterTypeNameAttribute : Attribute
{

    public RegisterTypeNameAttribute(string name
        //, bool? autoRegisterFullName // ENH override options.AutoRegisterFullNames 
        //, bool? autoRegisterName // ENH options.AutoRegisterNames 
        , Type? primaryInterface = null
        , string? registryName = null
        )
    {
        Name = name;
        if (registryName == null && primaryInterface != null)
        {
            registryName = primaryInterface.GetCustomAttributes<TypeNameRegistryAttribute>().FirstOrDefault()?.Name;
        }
        if (registryName == null) throw new ArgumentNullException($"{nameof(registryName)} null and could not be determined from TypeNameRegistryAttribute");
        this.RegistryName = registryName;
    }

    public string Name { get; }
    public string RegistryName { get; }
}
