using ReactiveUI;
using LionFire.Inspection.Nodes;
using LionFire.Data.Async.Gets;

namespace LionFire.Inspection.ViewModels;

public class InspectorRowVM : ReactiveObject
{
    #region Relationships

    public InspectorVM? InspectorVM { get; set; }

    [Reactive]
    public NodeVM? NodeVM { get; set; }

    #endregion

    #region NodeVM

    #region Derived

    public bool CanEdit => NodeVM?.Node.Info.CanWrite() == true // REVIEW - better way to access Node's CanWrite?
                                                                //&& NodeVM?.ReadOnly == false
        && InspectorUIX.EditorExistsForType(NodeVM?.Node?.SourceType);

    #endregion

    #endregion

    #region Children

    public Func<Type, bool> CanExpand { get; set; } = InspectorUIX.DefaultCanExpand;

    //public bool? HasChildren => NodeVM?.Children?.Count > 0 == true;
    //public IObservableGets<bool?> HasChildren => ChildNodeVMs?.Any() == true; // TODO: move to NodeVM, add ternary and async support, make sure observability (auto get) is implemented

    //public IEnumerable<ReflectionMemberVM> ChildMemberVMs { get; set; } = Enumerable.Empty<ReflectionMemberVM>();

    //public IGetterVM<IEnumerable<object>> ChildMemberVMs2 { get; private set; } // TODO: IInspectorMemberVM
    //public IEnumerable<ReflectionMemberVM> ChildMemberVMs { get;  set; }

    #endregion

    public bool HasAnythingToShow => NodeVM == null || NodeVM.CanRead || NodeVM.CanWrite || NodeVM.HasChildren != false;

    [Reactive]
    public string DisplayValue { get; set; } = "";

    [Reactive]
    public string ValueClass { get; set; } = "";

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
