using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Mvvm.ObjectInspection;

public class TypeInteractionModel
{
    public TypeInteractionModel(IEnumerable<ReflectionMemberInfo> members)
    {
        this.members = members.ToList();
    }

    /// <summary>
    /// Sorted by Order
    /// </summary>
    public IReadOnlyList<ReflectionMemberInfo> Members => members;
    List<ReflectionMemberInfo> members = new();


    public List<string>? ValidationErrors { get; set; }
}
