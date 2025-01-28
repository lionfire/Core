using LionFire.FlexObjects;
using LionFire.Structures;
using System.Collections.Generic;

namespace LionFire.UI.Workspaces;

// Overkill?
public interface IWorkspaceItem : IFlex, IKeyed<string>
{
    string Title { get; set; }

    ///// <summary>
    ///// May be same as Title
    ///// </summary>
    //string DisplayTitle { get; set; }

    //// Data items
    //IEnumerable<object> Navs { get; }

    //// 
    //IEnumerable<object> ContentPanes { get;  }
}
