using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Linq;

namespace LionFire.Inspection;

public static class TypeInteractionModelGenerator
{

    public static TypeInteractionModel InitType(Type type, TypeScanOptions? options = null)
    {
        options ??= TypeScanOptions.Default;

        SortedList<int, ReflectionMemberInfo> members = new();
        SortedList<string, ReflectionMemberInfo> conflicts = new();
        SortedList<string, ReflectionMemberInfo> unordered = new();
        List<string>? validationErrors = null;

        #region Properties

        GetItems(options.Properties, o => type.GetProperties(o.BindingFlags), m => new ReflectionPropertyInfo(m));

        #endregion

        #region Fields

        GetItems(options.Fields, o => type.GetFields(o.BindingFlags), m => new ReflectionFieldInfo(m));

        #endregion

        #region Methods

        GetItems(options.Methods, o => type.GetMethods(o.BindingFlags), m => new ReflectionMethodInfo(m));

        #endregion

        #region Events

        GetItems(options.Events, o => type.GetEvents(o.BindingFlags), m => new ReflectionEventInfo(m));

        #endregion

        var model = new TypeInteractionModel(members.Values.Concat(unordered.Values).Concat(conflicts.Values))
        {
            ValidationErrors = validationErrors,
        };
        return model;

        #region Local

        int? GetOrder(System.Reflection.MemberInfo mi)
        {
            return mi.GetCustomAttribute<DisplayAttribute>()?.Order;
        }
        bool ShouldIgnore(System.Reflection.MemberInfo mi)
        {
            foreach (var ignoreAttributeType in options.IgnoreMemberAttributes)
            {
                if (mi.GetCustomAttribute(ignoreAttributeType) != null) return true;
            }
            return false;
        }

        void GetItems<T, TViewModel>(MemberScanOptions memberTypeOptions, Func<MemberScanOptions, T[]> get, Func<T, TViewModel> vm)
            where T : System.Reflection.MemberInfo
            where TViewModel : ReflectionMemberInfo
        {
            if (!memberTypeOptions.Enabled) return;

            HashSet<string>? overlapping = null;

            foreach (var mi in get(memberTypeOptions).Where<T>((T mi) => !ShouldIgnore(mi)))
            {
                var memberVM = vm(mi);

                var order = GetOrder(mi);

                if (order.HasValue)
                {
                    if (members.ContainsKey(order.Value))
                    {
                        conflicts.Add(mi.Name, memberVM);
                    }
                    else
                    {
                        members.Add(order.Value, memberVM);
                    }
                }
                else
                {
                    var name = mi.Name;
                    if (unordered.ContainsKey(name) || overlapping?.Contains(name) == true)
                    {
                        overlapping ??= new();
                        overlapping.Add(name);

                        if (mi is MethodInfo methodInfo)
                        {
                            name = NameForMethodInfo(methodInfo); // TODO: this fails if two overloads have matching Type names (fallback to Fullname or something else)
                        }
                        if (unordered.Remove(name, out var existing))
                        {
                            unordered.Add(existing.MemberInfo is MethodInfo ? NameForMethodInfo((MethodInfo)existing.MemberInfo) : name, existing);
                        }
                        #region (local) Methods
                        string NameForMethodInfo(MethodInfo methodInfo) => $"{name}({string.Join(',', methodInfo.GetParameters().Select(pi => pi.ParameterType.Name))})";
                        #endregion
                    }
                    unordered.Add(name, memberVM);
                }
            }
        }

        #endregion

    }

}