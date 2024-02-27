using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PrivateProxy;
using Stride.Physics;
using Stride.Engine;
using System.Reflection;
using Stride.Core.Mathematics;
using SharpFont;

namespace LionFire.Stride_;

// Upstream PR idea: avoid the need for this accessibility circumvention
[GeneratePrivateProxy(typeof(PhysicsComponent))]
public partial class PhysicsComponentProxy;


[GeneratePrivateProxy(typeof(CharacterComponent))]
public partial class CharacterComponentProxy;


[GeneratePrivateProxy(typeof(PhysicsSkinnedComponentBase))]
public partial class PhysicsSkinnedComponentBaseProxy;

[GeneratePrivateProxy(typeof(Simulation))]
public partial class SimulationProxy;



public static class UpdateTransformationComponentX
{

    // Upstream PR Idea: avoid this accessibility hack
    public static void UpdateTransformationComponent(this CharacterComponent c, ref Matrix worldTransform)
    {
        object[] arguments = [worldTransform];
        c.GetType().GetMethod(nameof(UpdateTransformationComponent), BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(c, arguments);
    }
}

public static class PhysicsComponentX
{

    public static void Attach(this PhysicsComponent @this, PhysicsProcessor.AssociatedData data)
    {
        @this.GetType().GetMethod(nameof(Attach), BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(@this, [data]);
    }
    public static void Detach(this PhysicsComponent @this)
    {
        @this.GetType().GetMethod(nameof(Detach), BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(@this,null);
    }
    public static void UpdateBones(this PhysicsComponent @this)
    {
        @this.GetType().GetMethod(nameof(UpdateBones), BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(@this,null);
    }

}

public static class SimulationX
{
    public static void ClearCollisionDataOf(this Simulation @this, PhysicsComponent component)
    {
        @this.GetType().GetMethod(nameof(ClearCollisionDataOf), BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(@this, [component]);
    }

}

