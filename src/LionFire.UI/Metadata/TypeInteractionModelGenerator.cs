using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Linq;

namespace LionFire.UI.Metadata;

public static class TypeInteractionModelGenerator
{

    public static TypeInteractionModel InitType(Type type, TypeScanOptions? options = null)
    {
        options ??= TypeScanOptions.Default;

        SortedList<int, MemberInfoVM> members = new();
        SortedList<string, MemberInfoVM> conflicts = new();
        SortedList<string, MemberInfoVM> unordered = new();
        List<string>? validationErrors = null;

        #region Properties

        GetItems(options.Properties, o => type.GetProperties(o.BindingFlags), m => new PropertyInfoVM(m));

        #endregion

        #region Fields

        GetItems(options.Fields, o => type.GetFields(o.BindingFlags), m => new FieldInfoVM(m));

        #endregion

        #region Methods

        GetItems(options.Methods, o => type.GetMethods(o.BindingFlags), m => new MethodInfoVM(m));

        #endregion

        var model = new TypeInteractionModel(members.Values.Concat(unordered.Values).Concat(conflicts.Values))
        {
            ValidationErrors = validationErrors,
        };
        return model;

        #region Local

        int? GetOrder(MemberInfo mi)
        {
            return mi.GetCustomAttribute<DisplayAttribute>()?.Order;
        }
        bool ShouldIgnore(MemberInfo mi)
        {
            foreach (var ignoreAttributeType in options.IgnoreMemberAttributes)
            {
                if (mi.GetCustomAttribute(ignoreAttributeType) != null) return true;
            }
            return false;
        }

        void GetItems<T, TViewModel>(MemberScanOptions memberTypeOptions, Func<MemberScanOptions, T[]> get, Func<T, TViewModel> vm)
            where T : MemberInfo
            where TViewModel : MemberInfoVM
        {
            if (!memberTypeOptions.Enabled) return;

            foreach (var mi in get(memberTypeOptions).Where(mi => !ShouldIgnore(mi)))
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
                    unordered.Add(mi.Name, memberVM);
                }
            }
        }

        #endregion

    }

}