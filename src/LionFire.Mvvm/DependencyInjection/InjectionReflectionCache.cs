using System.Reflection;

namespace LionFire.Mvvm.DependencyInjection;

public static class InjectionReflectionCache<T, TInput>
{
    public static bool IncludeInputInConstructorParameters { get; set; }
    public static PropertyInfo? InjectProperty { get; set; }
    public static FieldInfo? InjectField { get; set; }

    static InjectionReflectionCache()
    {
        Init();
    }

    public static void Init()
    {
        foreach (var ctor in typeof(T).GetConstructors())
        {
            IncludeInputInConstructorParameters = ctor.GetParameters().Where(p => p.ParameterType.IsAssignableFrom(typeof(TInput))).Any();
        }
        if (!IncludeInputInConstructorParameters)
        {
            InjectProperty = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Where(pi => pi.CanWrite && pi.PropertyType == typeof(TInput)).FirstOrDefault();
            if (InjectProperty == null)
            {
                InjectField = typeof(T).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Where(pi => pi.FieldType == typeof(TInput)).FirstOrDefault();
            }
        }
        // ENH: inspect attributes that explicitly specify injection
    }
}
