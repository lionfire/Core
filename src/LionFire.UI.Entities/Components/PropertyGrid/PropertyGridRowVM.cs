using ReactiveUI;
using LionFire.UI.Components.PropertyGrid;
using Newtonsoft.Json.Linq;
using LionFire.Mvvm.ObjectInspection;
using LionFire.Data.Gets;

namespace LionFire.UI.Components;

public class PropertyVM : ReactiveObject
{

    #region Relationships

    public IObjectEditorVM? ObjectEditorVM { get; set; }

    #endregion

    #region MemberVM


    public MemberVM? MemberVM
    {
        get => memberVM; 
        set
        {
            memberVM = value;
            LazilyGets = memberVM as ILazilyGets<object>;
        }
    }
    private MemberVM? memberVM;

    public IDataMemberVM? DataMemberVM => MemberVM as IDataMemberVM;

    #region Derived

    public ILazilyGets<object>? LazilyGets { get; private set; }
    public Value<object>? AsyncSets { get; private set; }

    public bool ReadOnly => ObjectEditorVM?.ReadOnly == true;

    public bool EditorExistsForType(Type type) // TODO - route this to IViewTypeProvider somehow
    {
        if (type.IsGenericType)
        {
            var generic = type.GetGenericTypeDefinition();
            if (generic == typeof(Nullable<>))
            {
                return EditorExistsForType(type.GetGenericArguments()[0]);
            }
        }

        if (type == typeof(bool)
            || type == typeof(string)
            || type == typeof(int)
            || type == typeof(uint)
            || type == typeof(short)
            || type == typeof(ushort)
            || type == typeof(byte)
            || type == typeof(sbyte)
            || type == typeof(long)
            || type == typeof(ulong)
            || type == typeof(float)
            || type == typeof(double)
            || type == typeof(decimal)
            || type == typeof(DateTime)
            || type == typeof(DateTimeOffset)
            || type == typeof(TimeSpan)
            || type == typeof(char)
            || type == typeof(Guid)
            || type == typeof(Uri)
            )
            return true;
        return false;
    }
    public bool CanEdit => MemberVM.MemberInfoVM.CanWrite
        && !MemberVM.ReadOnly
        && EditorExistsForType(DataType);

    #endregion

    #endregion

    #region State

    public object Value { get; set; }
    public Type? ValueType { get; set; }
    public Type? DataType => (MemberVM as IDataMemberVM)?.DataType;

    public bool ValueTypeDiffersFromMemberType => ValueType != null && ValueType != MemberVM.MemberInfoVM.Type;

    #region Display

    public string? DisplayValue { get; set; }
    public string ValueClass { get; set; } = ""; // TODO: ValueClasses enumerable

    #endregion

    #endregion

    #region Children

    public Func<Type, bool> CanExpand { get; set; } = DefaultCanExpand;
    public static bool DefaultCanExpand(Type type)
    {
        if (type == typeof(string)) return false;
        if (type == typeof(RuntimeTypeHandle)) return false;
        return true;
    }

    public bool HasChildren => ChildMemberVMs?.Any() == true;
    public IEnumerable<MemberVM> ChildMemberVMs { get; set; } = Enumerable.Empty<MemberVM>();

    #endregion


    public void Refresh()
    {
        try
        {
            if (MemberVM is IDataMemberVM d)
            {
                if (d.CanGetValue)
                {
                    Value = d.GetValue();
                    ValueType = Value?.GetType();
                    var result = Value?.ToString();
                    if (MemberVM.MemberInfoVM.Type == typeof(string) && result != null)
                    {
                        DisplayValue = $"\"{result}\"";
                    }
                    ValueClass = result == null ? "null" : result.GetType().Name;
                    DisplayValue = result ?? "(null)";
                }
                else
                {
                    DisplayValue = $"{{{d.MemberInfo.Name}}}";
                }
            }
            else
            {
                DisplayValue = "";
            }
        }
        catch (Exception ex)
        {
            DisplayValue = $"<error: {ex.GetType().Name}>";
            ValueClass = "error";
        }
    }
}

public class PropertyGridRowVM : PropertyVM
{
    //public PropertyGridRowVM(IViewTypeProvider viewTypeProvider)
    //{
    //}

    public PropertyGridVM? PropertyGridVM { get => ObjectEditorVM as PropertyGridVM; set => ObjectEditorVM = value; }
    public bool IsExpanded { get; set; }
}
