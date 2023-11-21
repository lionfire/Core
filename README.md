
# LionFire.Core

Ambitious set of mini-frameworks being developed for other LionFire packages, mostly targeting .NET 7.0+ (with some deprecated parts in .NET Framework 4.8 and netstandard2.0.)  I plan to drop .NET 7 for 8 once one of my needs for .NET 7 disappears, (Spotware cTrader).

Docs: https://lionfire.readthedocs.io
Home: https://open.lionfire.software

# Roadmap

I am fleshing all this out for my own purposes as I need to and am inclined to, with the goal of creating components and frameworks that are useful in the long term.  While I intend for the frameworks and libraries to be decoupled, they may share some common interfaces that hopefully raise the lowest common denominator from the BCL, making some patterns possible that I intend to use heavily, such as multi-typing.

If you find a certain part interesting and want to help advance it into a first class library or framework, let me know.

# Philosophy

- Base layer
  - Minimal dependencies
- Core toolkit
  - provides some innovative cross-cutting functionality
  - mostly unopinionated
- various LionFire Toolkits
  - the bulk of complex innovation
  - may depend on each other
  - Intended to be a la carte: some toolkits will die, some will live on.  They are kept separate so this can happen organically.
- LionFire Framework
  - An opinionated holistic framework that depends on several LionFire Toolkits
  - the idea: `dotnet new lionfire-app` to get a new Program.cs with a boatload of features from one line of code: `new HostApplicationBuilder().LionFire();`
