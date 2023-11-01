﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace LionFire.Data.Id;

/// <summary>
/// A mechanism that can determine the Id of an object, given the object and some strategies.
/// </summary>
public class IdAdapter : IIdAdapter
{
    #region Configuration

    public IEnumerable<IIdMappingStrategy> Strategies { get; }

    #endregion

    #region Lifecycle

    public IdAdapter(IdAdapterConfiguration configuration)
    {
        Strategies = configuration.Strategies;
    }

    #endregion

    #region State

    ConcurrentDictionary<Type, IIdMappingStrategy> resolvedStrategies { get; } = new ConcurrentDictionary<Type, IIdMappingStrategy>();

    #endregion


    #region (public) Methods

    public (bool, string) TryGetId(object obj)
    {
        if (obj == null) return (false, default);

        if(resolvedStrategies.TryGetValue(obj.GetType(), out IIdMappingStrategy strat))
        {
            var result = strat.TryGetId(obj);
            if (!result.Item1) throw new Exception($"IIdMappingStrategy.TryGetId previously returned true for this type ({obj.GetType().FullName}) but now returned false.  This is not expected.");
            return result;
        }

        foreach (var strategy in Strategies)
        {
            var result = strategy.TryGetId(obj);
            if (result.Item1)
            {
                resolvedStrategies.TryAdd(obj.GetType(), strategy);
                return (true, result.Item2);
            }
        }
        return (false, default);
    }

    public string GetId(object obj)
    {
        var result = TryGetId(obj);
        if (!result.Item1) throw new ArgumentException($"No strategy could determine how to get Id for object of type ${obj?.GetType().FullName}");
        return result.Item2;
    }

    #endregion

}
