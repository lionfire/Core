#if false
// Inspired by and derived from Stride's PhysicsProcessor
//
// Stride license:
//
// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
//
// Copyright (c) 2010-2013 SharpDX - Alexandre Mutel
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Collections.Generic;
using Stride.Core;
using Stride.Core.Diagnostics;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Games;
using Stride.Physics;
using Stride.Physics.Engine;
using Stride.Rendering;
using AssociatedData = Stride.Physics.PhysicsProcessor.AssociatedData;
using PrivateProxy;
using System.Reflection;
using Silk.NET.Maths;

namespace LionFire.Stride_.Physics;

// Idea for Upstream PR request: avoid code duplication with PhysicsController, somehow
public class HeadlessPhysicsProcessor : EntityProcessor<PhysicsComponent, PhysicsProcessor.AssociatedData>
{
    //public class AssociatedData
    //{
    //    public PhysicsComponent? PhysicsComponent;
    //    public TransformComponent? TransformComponent;
    //    public ModelComponent? ModelComponent; //not mandatory, could be null e.g. invisible triggers
    //    public bool BoneMatricesUpdated;
    //}

    private readonly List<PhysicsComponent> elements = new List<PhysicsComponent>();
    private readonly List<PhysicsSkinnedComponentBase> boneElements = new List<PhysicsSkinnedComponentBase>();
    private readonly List<CharacterComponent> characters = new List<CharacterComponent>();

    private Bullet2PhysicsSystem? physicsSystem;
    private Scene? parentScene;

    private bool colliderShapesRendering;

    private PhysicsShapesRenderingService debugShapeRendering;

    public HeadlessPhysicsProcessor()
        : base(typeof(TransformComponent))
    {
        Order = 0xFFFE;
    }

    /// <summary>
    /// Gets or sets the associated parent scene to render the physics debug shapes. Assigned with default one on <see cref="OnSystemAdd"/>
    /// </summary>
    /// <value>
    /// The parent scene.
    /// </value>
    public Scene ParentScene
    {
        get => parentScene;
        set
        {
            if (value != parentScene)
            {
                parentScene = value;
            }
        }
    }

    public Simulation? Simulation { get; private set; }

    protected override AssociatedData GenerateComponentData(Entity entity, PhysicsComponent component)
    {
        var data = new AssociatedData
        {
            PhysicsComponent = component,
            TransformComponent = entity.Transform,
            ModelComponent = entity.Get<ModelComponent>(),
        };

        // Upstream PR idea: avoid this circumvention HACK of accessibility of Simulation property
         data.PhysicsComponent.GetType().GetProperty(nameof(data.PhysicsComponent.Simulation))!.SetValue(data.PhysicsComponent, Simulation);
        //data.PhysicsComponent.AsPrivateProxy().Simulation = Simulation;
        return data;
    }

    protected override bool IsAssociatedDataValid(Entity entity, PhysicsComponent physicsComponent, AssociatedData associatedData)
    {
        return
            physicsComponent == associatedData.PhysicsComponent &&
            entity.Transform == associatedData.TransformComponent &&
            entity.Get<ModelComponent>() == associatedData.ModelComponent;
    }

    protected override void OnEntityComponentAdding(Entity entity, PhysicsComponent component, AssociatedData data)
    {
        // Tagged for removal? If yes, cancel it
        if (currentFrameRemovals.Remove(component))
            return;

        component.Attach(data);

        var character = component as CharacterComponent;
        if (character != null)
        {
            characters.Add(character);
        }

        elements.Add(component);

        if (component.BoneIndex != -1)
        {
            boneElements.Add((PhysicsSkinnedComponentBase)component);
        }
    }

    private void ComponentRemoval(PhysicsComponent component)
    {
        Simulation.ClearCollisionDataOf(component);

        if (component.BoneIndex != -1)
        {
            boneElements.Remove((PhysicsSkinnedComponentBase)component);
        }

        elements.Remove(component);

        if (colliderShapesRendering)
        {
            component.RemoveDebugEntity(debugScene);
        }

        var character = component as CharacterComponent;
        if (character != null)
        {
            characters.Remove(character);
        }

        component.Detach();
    }

    private readonly HashSet<PhysicsComponent> currentFrameRemovals = new HashSet<PhysicsComponent>();

    protected override void OnEntityComponentRemoved(Entity entity, PhysicsComponent component, AssociatedData data)
    {
        currentFrameRemovals.Add(component);
    }

    protected override void OnSystemAdd()
    {
        physicsSystem = (Bullet2PhysicsSystem)Services.GetService<IPhysicsSystem>();
        if (physicsSystem == null)
        {
            physicsSystem = new Bullet2PhysicsSystem(Services);
            Services.AddService<IPhysicsSystem>(physicsSystem);
            var gameSystems = Services.GetSafeServiceAs<IGameSystemCollection>();
            gameSystems.Add(physicsSystem);
        }

        ((IReferencable)physicsSystem).AddReference();

         Simulation = physicsSystem.Create(this);

        parentScene = Services.GetSafeServiceAs<SceneSystem>()?.SceneInstance?.RootScene;
    }

    protected override void OnSystemRemove()
    {
        if (physicsSystem != null)
        {
            physicsSystem.Release(this);
            ((IReferencable)physicsSystem).Release();
        }
    }

    internal void UpdateCharacters()
    {
        var charactersProfilingState = Profiler.Begin(PhysicsProfilingKeys.CharactersProfilingKey);
        var activeCharacters = 0;
        //characters need manual updating
        foreach (var element in characters)
        {
            if (!element.Enabled || element.ColliderShape == null) continue;

            var worldTransform = Matrix.RotationQuaternion(element.Orientation) * element.PhysicsWorldTransform;
            //element.UpdateTransformationComponent(ref worldTransform);
            element.UpdateTransformationComponent( ref worldTransform);

            if (element.DebugEntity != null)
            {
                Vector3 scale, pos;
                Quaternion rot;
                worldTransform.Decompose(out scale, out rot, out pos);
                element.DebugEntity.Transform.Position = pos;
                element.DebugEntity.Transform.Rotation = rot;
            }

            charactersProfilingState.Mark();
            activeCharacters++;
        }
        charactersProfilingState.End("Active characters: {0}", activeCharacters);
    }

  
    internal void UpdateBones()
    {
        foreach (var element in boneElements)
        {
            element.UpdateBones();
        }
    }

    public void UpdateRemovals()
    {
        foreach (var currentFrameRemoval in currentFrameRemovals)
        {
            ComponentRemoval(currentFrameRemoval);
        }

        currentFrameRemovals.Clear();
    }
}

#endif