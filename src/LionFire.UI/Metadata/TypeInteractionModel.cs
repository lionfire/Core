using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.UI.Metadata;

public class TypeInteractionModel
{
    public TypeInteractionModel(IEnumerable<MemberInfoVM> members)
    {
        this.members = members.ToList();
    }

    /// <summary>
    /// Sorted by Order
    /// </summary>
    public IReadOnlyList<MemberInfoVM> Members => members;
    List<MemberInfoVM> members = new();


    public List<string>? ValidationErrors { get; set; }
}
