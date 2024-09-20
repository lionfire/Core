#nullable enable

using System.Linq.Expressions;
using System.Reflection;

namespace LionFire.Blazor.Components;

internal class KeyedCollectionViewUtilities
{
    public static void BuildPropertyColumn<TValueVM, TValue>(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder builder, System.Reflection.PropertyInfo prop, string? subProperty = null)
    {
        mi.MakeGenericMethod(typeof(TValueVM), typeof(TValue), prop.PropertyType).Invoke(null, new object?[] { builder, prop, subProperty });
    }

    #region (private)


    private static MethodInfo mi = typeof(KeyedCollectionViewUtilities).GetMethod(nameof(_BuildPropertyColumn), BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public);

#if false
    //[Conditional("DEBUG")]
    //void ___LightUpSyntaxHighlighting() => _BuildPropertyColumn<string, string, string>(null, null, null);
#endif

    private static void _BuildPropertyColumn<TValueVM, TValue, T>(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder builder, System.Reflection.PropertyInfo prop, string? subProperty = null)
    {
        builder.OpenComponent<PropertyColumn<TValueVM, T>>(0);

        ParameterExpression parameter = Expression.Parameter(typeof(TValueVM), "p");
        Expression<Func<TValueVM, T>> lambda;
        if (subProperty == null)
        {
            MemberExpression property = Expression.Property(parameter, prop.Name);
            lambda = Expression.Lambda<Func<TValueVM, T>>(property, parameter);
        }
        else
        {
#if true
            MemberExpression objectProperty = Expression.Property(parameter, subProperty);
            //UnaryExpression castExpression = Expression.Convert(objectProperty, typeof(TValue));
            MemberExpression nameProperty = Expression.Property(objectProperty, prop.Name);
            lambda = Expression.Lambda<Func<TValueVM, T>>(nameProperty, parameter);
#else
            MemberExpression objectProperty = Expression.Property(parameter, subProperty);
            UnaryExpression castExpression = Expression.Convert(objectProperty, typeof(TValue));
            MemberExpression nameProperty = Expression.Property(castExpression, prop.Name);
            lambda = Expression.Lambda<Func<TValueVM, T>>(nameProperty, parameter);
#endif
        }

        builder.AddAttribute(1, "Property", lambda);
        builder.CloseComponent();
    }
    
    #endregion

}


// OLD DUPE
//file class FileReflectionUtilities
//{

//    private static MethodInfo mi = typeof(KeyedCollectionViewUtilities).GetMethod("_NewMethod", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public);
//    public static void NewMethod<TValueVM, TValue>(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder builder, System.Reflection.PropertyInfo prop, string? subProperty = null)
//    {
//        mi.MakeGenericMethod(typeof(TValueVM), typeof(TValue), prop.PropertyType).Invoke(null, new object?[] { builder, prop, subProperty });
//    }

//    [Conditional("DEBUG")]
//    void X() => _NewMethod<string, string, string>(null, null, null);

//    static void _NewMethod<TValueVM, TValue, T>(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder builder, System.Reflection.PropertyInfo prop, string? subProperty = null)
//    {
//        builder.OpenComponent<PropertyColumn<TValueVM, T>>(0);

//        ParameterExpression parameter = Expression.Parameter(typeof(TValueVM), "p");
//        Expression<Func<TValueVM, T>> lambda;
//        if (subProperty == null)
//        {
//            MemberExpression property = Expression.Property(parameter, prop.Name);
//            lambda = Expression.Lambda<Func<TValueVM, T>>(property, parameter);
//        }
//        else
//        {
//#if true
//            MemberExpression objectProperty = Expression.Property(parameter, subProperty);
//            //UnaryExpression castExpression = Expression.Convert(objectProperty, typeof(TValue));
//            MemberExpression nameProperty = Expression.Property(objectProperty, prop.Name);
//            lambda = Expression.Lambda<Func<TValueVM, T>>(nameProperty, parameter);
//#else
//            MemberExpression objectProperty = Expression.Property(parameter, subProperty);
//            UnaryExpression castExpression = Expression.Convert(objectProperty, typeof(TValue));
//            MemberExpression nameProperty = Expression.Property(castExpression, prop.Name);
//            lambda = Expression.Lambda<Func<TValueVM, T>>(nameProperty, parameter);
//#endif
//        }

//        builder.AddAttribute(1, "Property", lambda);
//        builder.CloseComponent();
//    }
//}
