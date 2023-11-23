using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Metadata;

[AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
public class OrderAttribute : Attribute
{
    public OrderAttribute(float order)
    {
        Order = order;
    }

    public float Order { get; }
}
