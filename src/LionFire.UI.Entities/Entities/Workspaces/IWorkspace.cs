using LionFire.FlexObjects;
using LionFire.Structures;
using Swordfish.NET.Collections;
using System;
using System.Collections.ObjectModel;

namespace LionFire.UI.Workspaces
{
    public interface IWorkspace2 : IKeyed<int>
    {
    }

    /// <remarks>
    /// Data and business logic for a domain object that is a user interface concept
    /// </remarks>
    public interface IWorkspace : IKeyed<string>, IFlex
    {
        ConcurrentObservableSortedDictionary<string, object> Items { get; }

        void Add(object item);
        
    }
}
