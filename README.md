
# LionFire.Core Repository

Ambitious set of mini-frameworks being developed for other LionFire packages, mostly targeting .NET 7.0+ (with some deprecated parts in .NET Framework 4.8 and netstandard2.0.)  I plan to drop .NET 7 for 8 once one of my needs for .NET 7 disappears, (Spotware cTrader).

Docs: https://lionfire.readthedocs.io
Home: https://open.lionfire.software

# Status

This is only really intended to be usable to me, but feel free to mine the codebase for interesting parts, and let me know if you think the community would benefit from anything being a properly published and supported OSS project.  I intend to do this for several packages, but I am not in a rush at the moment.

# Roadmap

I am fleshing all this out for my own purposes as I need to and am inclined to, with the goal of creating components and frameworks that are useful in the long term.  While I intend for the frameworks and libraries to be decoupled, they may share some common interfaces that hopefully raise the lowest common denominator from the BCL, making some patterns possible that I intend to use heavily, such as multi-typing.

If you find a certain part interesting and want to help advance it into a first class library or framework, let me know.

# Philosophy

- [Base layer](#base-layer)
  - Minimal dependencies
- [Core](#core) Toolkit
  - provides some innovative cross-cutting functionality
  - mostly unopinionated
- [Toolkits](#toolkits)
  - unopinionated
  - the bulk of complex innovation
  - may depend on each other
  - Intended to be a la carte: some toolkits will die, some will live on.  They are kept separate so this can happen organically.  Though some of them form a deep stack for data persistence.
- The [LionFire Framework](#framework)
  - An opinionated holistic framework that integrates several LionFire Toolkits, by default
  - the idea: `dotnet new lionfire-app` to get a new Program.cs with the boatload of features I want for every app from one line of code: `new HostApplicationBuilder().LionFire();`

# Outline of Available Libraries

## See also

First, here are projects that have been spun off into their own repo:

### State Machines

 - (https://github.com/lionfire/class-state-machine)[Class State Machine] generator

### Trading

 - (https://github.com/lionfire/Trading)[Trading] related code (crypto, forex, bots, etc.)

## Base layer

### LionFire.Base

- no dependencies
- Augments .NET's BCL

### LionFire.Flex

- Add `object? FlexData { get; set; }` to your class, inherit IFlex, and store any strongly typed data in it.
- A strongly typed approach to making a C# dynamic / expando object.
- Depends on Base

### Binding

- Data binding from one object to another

### Behaviors

- Composable behaviors, for workflows or AI

### LionFire.Structures

- A large grab bag of collection and other data types
- Depends on Base, Flex.

## Toolkits

### Core

- A grab bag of essential common metadata and essential capabilities required by other toolkits
- Attributes
- Dependency Injection / Service Location
- (I will probably try to minimize this.)

### Hosting

- My own extensions to Microsoft.Extensions.Hosting, IServiceCollection, etc.
- Wraps IHostBuilder and HostApplicationBuilder

### AspNetCore

- small set of utilities for AspNetCore

### Serialization

- An open-ended serialization framework

### Referencing

- For making sense of URLs

### Handles

- Handles have a Reference, and can do things with it, like load or save data.
- Open-ended, to add your own persistence or RPC layers

### Persisistence

- An open-ended framework for persisting anything to anything

### Virtual Object System (VOS)

 - Mount Filesystem directories, Databases, Zip files, or whatever you want into a virtual filesystem
 - Multiple mounts can be mounted into the same virtual filesystem for an overlay effect.  (Or just use exclusive mounts if this is too crazy.)
 - Dependency Injection via virtual directory inheritence.
 - Can serve as a blackboard or way to introspect your application
 - Depends on: Persistence, Handles, Referencing, Serialization

### Instantiating

 - A toolkit for creating instances of things based on templates, and optionally parameters

## "Framework layer"

Warning: opinionated beyond this point.

### Core.Extras

- General purpose things along the lines of LionFire.Core but that are infrequently or not universally used
- Depends on Core, and may depend on several other toolkits' Abstractions dll.
- (Should maybe be renamed to LionFire.Framework.Minimal)

### LionFire.AspNetCore.Framework

- The current best opinionated way to build an ASP.NET Core application

### LionFire.Vos.VosApp

- Provides opinionated default capabilities to a VOS application (configuration, etc.)
- (Consider renaming to LionFire.Vos.App.Framework)

### LionFire.Framework

- Brings all the toolkits together for the current best opinionated way to build an application

## Other

### Dycen

- Stands for: 'Dynamic concept environment'
- An experiment for "Code as Data"
- The idea: write C# code at runtime, to run in a sandbox, without recompiling a running app.
- Hasn't been touched in a while.  Probably needs some thought about how to integrate with modern roslyn, and/or external processes.  I know other languages/runtimes have capability like this -- wonder if anyone in .NET has made any progress on this front.
