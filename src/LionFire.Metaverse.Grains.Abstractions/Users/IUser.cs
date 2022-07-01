#nullable enable

using Orleans.Http.Abstractions;
using System;
using System.Threading.Tasks;

namespace LionFire.Metaverse.Users;

// REVIEW this api

/// <summary>
/// Core for a User:
///  - Does it exist?
///  - When was it created?
///  - What is the Name?
/// </summary>
[Route]
public interface IUser : Orleans.IGrainWithStringKey
{
    /// <summary>
    /// Return true if the user has been created, false otherwise
    /// </summary>
    /// <returns>True if user has been craeted, otherwise false</returns>
    //[HttpGet("[action]")] 
    async Task<bool> Exists() => (await GetCreationTime()).HasValue;

    /// <summary>
    /// Creation time of user
    /// </summary>
    /// <returns>Creation time of user, or null if user was never created</returns>
    Task<DateTime?> GetCreationTime();

    /// <summary>
    /// Returns when the user's name most recently became valid
    /// </summary>
    /// <returns>when the user's name most recently became valid, or null if the user has never had a name</returns>
    Task<DateTime?> GetNameSince();

    /// <summary>
    /// Create a user for the current Id.  Sets the CreationTime for the UserId, so that Exists will return true.
    /// (FUTURE: This could raise any system-wide events for creating a user.  )
    /// </summary>
    /// <returns>true if created, null if user already created, false if id is invalid for creating a user</returns>
    Task<bool?> Create();

    /// <summary>
    /// Get the name of the user
    /// </summary>
    /// <returns>User's name</returns>
    Task<string?> GetName();

    Task SetName(string newName);

}
