#nullable enable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using LionFire.Dependencies;
using LionFire.Structures;

namespace LionFire.Referencing;

/// <summary>
/// Reference interface for all references: Vos, Handles.
/// References should be round-trip convertible to and from a string key (from IKeyed)
/// </summary>
[FileExtension("r")]
public interface IReference :
    IUrled
   , IKeyed<string> // is a bonus now.
//, IHostReference // RECENTCHANGE: Don't depend on IHostReference
//, ICompatibleWithSome<string>
{
    string Scheme { get; }

    string Path { get; }
}

public interface IReference<out TValue> : IReference
{
}
