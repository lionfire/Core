using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;

namespace LionFire.LionRing;


[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
public sealed class LionOperationAttribute : Attribute
{      
    readonly int? id;
    public NetDeliveryMethod NetDeliveryMethod { get; set; }

    public LionOperationAttribute() { }
    public LionOperationAttribute(int id)
    {
        this.id = id;
    }

    //public LionOperationAttribute() // TODO - autoassign id if none is specified
    //{
    //    this.id = null;
    //}

    public int? Id
    {
        get { return id; }
    }

    public bool IsOneWay
    {
        get; set;
    }

    public bool IsReliable
    {
        get { return isReliable; }
        set { isReliable = value; }
    } private bool isReliable = true;

    public bool IsSequenced
    {
        get { return isSequenced; }
        set { isSequenced = value; }
    } private bool isSequenced = false;
}
