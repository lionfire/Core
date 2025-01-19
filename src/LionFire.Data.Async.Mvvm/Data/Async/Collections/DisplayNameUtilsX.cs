using LionFire.ExtensionMethods.Types;

namespace LionFire.Data.Mvvm;

public static class DisplayNameUtilsX
{
    public static string DisplayNameForType(Type t) => t.Name.Replace("Item", "").TrimLeadingIOnInterfaceType();
}

