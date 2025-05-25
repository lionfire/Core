using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.DynamicData_;


/// <summary>
/// Indicates a SourceCache<TValue, TKey> where TValue == TKey can be assumed to have values identical to keys.  Used to create a sort of HashSet using DynamicData.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
public class SetAttribute : Attribute
{
}
