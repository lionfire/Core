using System.Collections.Generic;

namespace LionFire.Vfs.Conventions;

public class Order
{

    /// <summary>
    /// The explicit way to specify order of files in the same folder
    /// </summary>
    public SortedList<float /* order */, string /* fileName */>? Items { get; set; }
}
