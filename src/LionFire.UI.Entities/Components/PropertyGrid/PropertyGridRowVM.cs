using ReactiveUI;
using LionFire.UI.Components.PropertyGrid;
using Newtonsoft.Json.Linq;
using LionFire.Inspection;
using LionFire.Data.Async.Gets;
using System.Reflection;
using LionFire.Data.Async;
using LionFire.Data.Async.Sets;
using System.Threading;
using LionFire.Data.Mvvm;

namespace LionFire.UI.Components;

public class AsyncValueMultiplexer
{

}

public class PropertyVM : ReactiveObject
{

    #region Relationships

    public IObjectEditorVM? ObjectEditorVM { get; set; }

    #endregion

    #region MemberVM

    public IInspectorNode? MemberVM
    {
        get => memberVM;
        set
        {
            memberVM = value;

            Getter = memberVM?.Source as IGetter<object>;

            if (memberVM?.Source is ISetter setter)
            {
                StagingSetTypes = StagesSetWriter.GetStagesSetTypes(setter).ToArray();

                if (StagingSetTypes.Length == 0) { SetStagedValue = null; }
                else if (StagingSetTypes.Length > 1) throw new NotImplementedException("More than one IWriteStagesSet<> interface not implemented");
                else
                {
                    var propertyInfo = typeof(IStagesSet<>).MakeGenericType(StagingSetTypes[0]).GetProperty(nameof(IStagesSet<object>.StagedValue))!;
                    SetStagedValue = val => propertyInfo.SetValue(setter, val);
                }

                NonstagingSetTypes = StagesSetWriter.GetNonstagingSetterTypes(setter).ToArray();

                if (NonstagingSetTypes.Length == 0) { SetValue = null; }
                else if (NonstagingSetTypes.Length > 1) throw new NotImplementedException("More than one Non-staging ISetter<> interface not implemented");
                else
                {
                    var methodInfo = typeof(ISetter<>).MakeGenericType(NonstagingSetTypes[0]).GetMethod(nameof(ISetter<object>.Set))!;
                    SetValue = val => methodInfo.Invoke(setter, new object[] { val, CancellationToken.None });
                }
            }
            else
            {
                SetStagedValue = null;
            }
        }
    }
    private IInspectorNode? memberVM;

    #region Derived

    public IGetter<object>? Getter { get; private set; }
    public Value<object>? AsyncValue { get; private set; }

    #region ISetter

    public Type[]? StagingSetTypes { get; private set; }
    private Action<object>? SetStagedValue { get; set; }
    public bool CanSetStagedValue => SetStagedValue != null;

    public Type[]? NonstagingSetTypes { get; private set; }
    private Action<object>? SetValue { get; set; }
    public bool CanSetValue => SetValue != null;



    public IStagesSet<object>? Setter { get; private set; }
    public ISetterRxO<object>? SetterRxO { get; private set; }

    #endregion

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
    public bool CanEdit => MemberVM.Info.CanWrite()
        //&& !MemberVM.ReadOnly
        && EditorExistsForType(Type);

    #endregion

    #endregion

    #region State

    public object Value
    {
        get
        {
            if (AsyncValue != null) { return AsyncValue.ReadCacheValue; }
            if (Getter != null) { return Getter.ReadCacheValue; }
            if (AsyncValue != null) { return AsyncValue.StagedValue; }
            if (Setter != null) { return Setter.StagedValue; }
            return default;
        }
        set
        {

            if (AsyncValue != null) { AsyncValue.StagedValue = value; }
            if (Setter != null) { AsyncValue.StagedValue = value; }

        }
    }
    private object value;

    public Type? CurrentValueType { get; set; }
    public Type? Type => MemberVM.Info.Type;

    public bool ValueTypeDiffersFromMemberType => CurrentValueType != null && CurrentValueType != MemberVM.Info.Type;

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
    public IEnumerable<ReflectionMemberVM> ChildMemberVMs { get; set; } = Enumerable.Empty<ReflectionMemberVM>();

    public IGetterVM<IEnumerable<object>> ChildMemberVMs2 { get; private set; } // TODO: IInspectorMemberVM
    public IEnumerable<ReflectionMemberVM> ChildMemberVMs;

    #endregion


    public void Refresh()
    {
        try
        {
            throw new NotImplementedException();

            //if (MemberVM is IReflectionDataMemberVM d)
            //{
            //    if (MemberVM.Info.CanRead())
            //    {
            //        Value = d.GetValue();
            //        CurrentValueType = Value?.GetType();
            //        var result = Value?.ToString();
            //        if (MemberVM.Info.Type == typeof(string) && result != null)
            //        {
            //            DisplayValue = $"\"{result}\"";
            //        }
            //        ValueClass = result == null ? "null" : result.GetType().Name;
            //        DisplayValue = result ?? "(null)";
            //    }
            //    else
            //    {
            //        DisplayValue = $"{{{d.MemberInfo.Name}}}";
            //    }
            //}
            //else
            //{
            //    DisplayValue = "";
            //}
        }
        catch (Exception ex)
        {
            DisplayValue = $"<error: {ex.GetType().Name.Replace("Exception", "")}>";
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
