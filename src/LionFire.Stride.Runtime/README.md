
## Status

WIP, not ready yet

## LionFire.Stride.Runtime.dll Overview

This aims first to provide:

- a headless (server-only, no graphics / input / audio / etc.) version of Stride's Game class and any other classes in Stride.Games.dll
- a thought experiment for how to perhaps someday merge headless support back into Stride.Games.dll while keeping changes to a reasonable minimum.

## How to use

The gist:

```
services.AddSingleton<StrideRuntimeServer>();
/// ...
var strideServer = ServiceProvider.GetService<StrideRuntimeServer>();
await strideServer.StartAsync(default);  // you can also register as an IHostedService for automatic start
```

Full instructions TBD.

# Technical details

This section is for Stride developers who want to understand the changes to how Stride classes are used. 

In some cases, stripped down alternatives are created.  In some cases, hacks to bypass `internal` and `private` accessibility are needed.

## New classes: Stripped down 

- HeadlessSceneSystem
- HeadlessPhysicsProcessor

## Accessibility hacks

It would be easy and safe to modify Stride to avoid the need for these hacks by making things more public/protected, or by merging this LionFire DLL back into Stride.Games.dll somehow.

- Simulation
- PhysicsProcessor


## Changes required

Cannot use these for headless:

- Game class
- SceneSystem (we provide HeadlessSceneSystem instead)
  - We need it for doing updates on Entities
- GamePlatform (because it's currently internal to Stride.Games.dll, though Stride PR [#1315](https://github.com/stride3d/stride/pull/1315) aims to make it public)
  - We have IGamePlatformEx instead 

## Suggestions / Ideas for PRs for Stride.Games.dll
- PhysicsComponent.set_Simulation: change from internal to public
  - This 
- Make SceneSystem support headless
  - Either by making a base class that is headless-only 
  - or have ISceneSystem with a SceneInstance property that SceneSystem and HeadlessSceneSystem implement

## Related projects

Simpler and limited (as of Feb 2024) proofs of concept of server-only that do not try to reuse much of Stride.Games.dll:
- https://github.com/Ethereal77/Stride.ClientServerSample
- https://github.com/xen2/Xenko.ClientServerSample

