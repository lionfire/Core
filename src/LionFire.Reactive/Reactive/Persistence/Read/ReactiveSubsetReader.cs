using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using DynamicData;
using DynamicData.Kernel;

namespace LionFire.Reactive.Persistence;

// AI Generated

/// <summary>
/// A reader that projects a subset of keys from a source IObservableReader, based on a dynamic observable of active keys.
/// Useful for creating filtered views (e.g. Portfolios) without loading all items from the source (e.g. Disk).
///
/// Uses two-cache architecture (DynamicData best practice):
/// - Keys cache: Lightweight, always populated immediately when keys are discovered
/// - Values cache: Populated on-demand when values are actually needed (lazy loading)
///
/// This enables scenarios like virtual scrolling where you show 100k keys but only load
/// values for the ~50 items currently visible on screen.
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TValue"></typeparam>
public class ReactiveSubsetReader<TKey, TValue> : IObservableReader<TKey, TValue>, IDisposable
    where TKey : notnull
    where TValue : notnull
{
    #region Dependencies

    private readonly IObservableReader<TKey, TValue> source;
    private readonly IObservable<IEnumerable<TKey>>? activeKeys;

    #endregion

    #region State

    // Separate caches - DynamicData best practice
    // Keys cache: lightweight, always populated immediately
    private readonly SourceCache<TKey, TKey> keyCache = new(k => k);

    // Values cache: stores (Key, Optional<Value>) pairs, populated on-demand for lazy loading
    private readonly SourceCache<(TKey Key, Optional<TValue> Value), TKey> valueCache = new(x => x.Key);

    private readonly CompositeDisposable disposables = new();

    // Track which keys have active value subscriptions
    private readonly Dictionary<TKey, IDisposable> valueSubscriptions = new();
    private readonly HashSet<TKey> loadedKeys = new();
    private readonly object syncLock = new();

    #endregion

    #region Lifecycle

    /// <summary>
    /// Creates a ReactiveSubsetReader with external activeKeys (backward compatible).
    /// </summary>
    public ReactiveSubsetReader(
        IObservableReader<TKey, TValue> source,
        IObservable<IEnumerable<TKey>> activeKeys)
        : this(source, activeKeys, listenToSourceKeys: false)
    {
    }

    /// <summary>
    /// Creates a ReactiveSubsetReader with configurable key discovery.
    /// </summary>
    /// <param name="source">The source reader containing all data</param>
    /// <param name="activeKeys">Observable of keys to include in subset (can be null if listenToSourceKeys is true)</param>
    /// <param name="listenToSourceKeys">If true, also discover keys from source.Keys</param>
    public ReactiveSubsetReader(
        IObservableReader<TKey, TValue> source,
        IObservable<IEnumerable<TKey>>? activeKeys,
        bool listenToSourceKeys)
    {
        this.source = source ?? throw new ArgumentNullException(nameof(source));
        this.activeKeys = activeKeys;

        if (activeKeys == null && !listenToSourceKeys)
        {
            throw new ArgumentException(
                "Either activeKeys must be provided or listenToSourceKeys must be true",
                nameof(activeKeys));
        }

        disposables.Add(keyCache);
        disposables.Add(valueCache);

        // Subscribe to key sources
        if (activeKeys != null)
        {
            SubscribeToActiveKeys();
        }

        if (listenToSourceKeys)
        {
            SubscribeToSourceKeys();
        }
    }

    /// <summary>
    /// Creates a ReactiveSubsetReader that discovers all keys from the source.
    /// </summary>
    public static ReactiveSubsetReader<TKey, TValue> FromSource(IObservableReader<TKey, TValue> source)
    {
        return new ReactiveSubsetReader<TKey, TValue>(source, activeKeys: null, listenToSourceKeys: true);
    }

    #endregion

    #region Key Discovery

    private void SubscribeToActiveKeys()
    {
        // Use SourceCache to handle diffing of the active keys list
        var diffCache = new SourceCache<TKey, TKey>(k => k);
        disposables.Add(diffCache);

        // Update diffCache whenever activeKeys changes, using EditDiff to minimize churn
        var sub = activeKeys!
            .Subscribe(keys => diffCache.EditDiff(keys, (a, b) => a.Equals(b)));
        disposables.Add(sub);

        // Watch for changes in the key set to manage subscriptions
        var keySubscription = diffCache.Connect()
            .Subscribe(changes =>
            {
                foreach (var change in changes)
                {
                    switch (change.Reason)
                    {
                        case ChangeReason.Add:
                            AddKey(change.Key);
                            break;
                        case ChangeReason.Remove:
                            RemoveKey(change.Key);
                            break;
                        // Update is unlikely for keys-as-values, but if it happens, no action needed
                    }
                }
            });

        disposables.Add(keySubscription);
    }

    private void SubscribeToSourceKeys()
    {
        disposables.Add(source.ListenAllKeys());

        var sub = source.Keys.Connect()
            .Subscribe(changes =>
            {
                foreach (var change in changes)
                {
                    switch (change.Reason)
                    {
                        case ChangeReason.Add:
                            AddKey(change.Key);
                            break;
                        case ChangeReason.Remove:
                            RemoveKey(change.Key);
                            break;
                    }
                }
            });
        disposables.Add(sub);
    }

    #endregion

    #region Key/Value Management

    private void AddKey(TKey key)
    {
        lock (syncLock)
        {
            // Add to key cache immediately - keys are visible right away without loading values
            keyCache.AddOrUpdate(key);

            // Add placeholder to value cache with None
            valueCache.AddOrUpdate((key, Optional<TValue>.None));

            // Don't subscribe to value yet - lazy loading
            if (!valueSubscriptions.ContainsKey(key))
            {
                valueSubscriptions[key] = Disposable.Empty;
            }
        }
    }

    private void RemoveKey(TKey key)
    {
        lock (syncLock)
        {
            keyCache.RemoveKey(key);
            valueCache.RemoveKey(key);

            if (valueSubscriptions.TryGetValue(key, out var sub))
            {
                sub.Dispose();
                valueSubscriptions.Remove(key);
            }
            loadedKeys.Remove(key);
        }
    }

    /// <summary>
    /// Ensures the value for a key is being loaded from the source.
    /// Called lazily when the value is actually needed.
    /// </summary>
    private void EnsureValueLoaded(TKey key)
    {
        lock (syncLock)
        {
            if (loadedKeys.Contains(key)) return;
            if (!keyCache.Lookup(key).HasValue) return; // Key not in subset

            loadedKeys.Add(key);

            var sub = source.GetValueObservable(key)
                .Subscribe(value =>
                {
                    var optional = value == null
                        ? Optional<TValue>.None
                        : Optional<TValue>.Create(value);
                    valueCache.AddOrUpdate((key, optional));
                });

            // Replace the empty disposable with the real subscription
            if (valueSubscriptions.TryGetValue(key, out var existing))
            {
                existing.Dispose();
            }
            valueSubscriptions[key] = sub;
        }
    }

    /// <summary>
    /// Unloads the value for a key, keeping only the key.
    /// Useful for memory management in virtual scrolling.
    /// </summary>
    public void UnloadValue(TKey key)
    {
        lock (syncLock)
        {
            if (!loadedKeys.Contains(key)) return;

            if (valueSubscriptions.TryGetValue(key, out var sub))
            {
                sub.Dispose();
                valueSubscriptions[key] = Disposable.Empty;
            }
            loadedKeys.Remove(key);

            // Reset to None but keep the key
            valueCache.AddOrUpdate((key, Optional<TValue>.None));
        }
    }

    #endregion

    #region Virtual Scrolling

    private IObservable<IEnumerable<TKey>>? listenTo;
    private IDisposable? listenToSubscription;

    /// <summary>
    /// Observable of keys that should have their values loaded.
    /// Use with virtual scrolling to only load values for visible items.
    /// When set, values are loaded for keys in the observable and optionally
    /// unloaded when keys leave the set.
    /// </summary>
    public IObservable<IEnumerable<TKey>>? ListenTo
    {
        get => listenTo;
        set
        {
            listenToSubscription?.Dispose();
            listenTo = value;

            if (value != null)
            {
                var previousKeys = new HashSet<TKey>();

                listenToSubscription = value
                    .Subscribe(visibleKeys =>
                    {
                        var currentKeys = visibleKeys.ToHashSet();

                        // Load newly visible keys
                        foreach (var key in currentKeys.Except(previousKeys))
                        {
                            EnsureValueLoaded(key);
                        }

                        // Optionally unload keys that are no longer visible
                        // (commented out - can be enabled for aggressive memory management)
                        // foreach (var key in previousKeys.Except(currentKeys))
                        // {
                        //     UnloadValue(key);
                        // }

                        previousKeys = currentKeys;
                    });

                disposables.Add(listenToSubscription);
            }
        }
    }

    #endregion

    #region IObservableReader Implementation

    /// <summary>
    /// All keys in the subset. Available immediately without loading values.
    /// </summary>
    public IObservableCache<TKey, TKey> Keys => keyCache.AsObservableCache();

    /// <summary>
    /// Values for keys in the subset. Values are Optional.None until loaded.
    /// </summary>
    public IObservableCache<Optional<TValue>, TKey> Values => _valuesCache ??= CreateValuesCache();
    private IObservableCache<Optional<TValue>, TKey>? _valuesCache;

    private IObservableCache<Optional<TValue>, TKey> CreateValuesCache()
    {
        var cache = valueCache.Connect().Transform(x => x.Value).AsObservableCache();
        disposables.Add(cache);
        return cache;
    }

    public IObservableCache<Optional<TValue>, TKey> ObservableCache => Values;

    public IDisposable ListenAllKeys()
    {
        // For subset reader with external keys, this is a no-op
        // For source-based discovery, ensure we're listening
        return Disposable.Empty;
    }

    public ValueTask<IDisposable> ListenAllValues()
    {
        // Load values for all known keys
        lock (syncLock)
        {
            foreach (var key in keyCache.Keys.ToList())
            {
                EnsureValueLoaded(key);
            }
        }

        return new ValueTask<IDisposable>(Disposable.Create(() =>
        {
            // Could optionally unload all values here
        }));
    }

    public IObservable<TValue?> GetValueObservable(TKey key)
    {
        // Ensure value is being loaded when someone requests it
        EnsureValueLoaded(key);
        return source.GetValueObservable(key);
    }

    public IObservable<TValue?>? GetValueObservableIfExists(TKey key)
    {
        lock (syncLock)
        {
            if (!keyCache.Lookup(key).HasValue)
            {
                return source.GetValueObservableIfExists(key);
            }
        }
        return GetValueObservable(key);
    }

    #endregion

    #region IDisposable

    public void Dispose()
    {
        lock (syncLock)
        {
            foreach (var sub in valueSubscriptions.Values)
            {
                sub.Dispose();
            }
            valueSubscriptions.Clear();
            loadedKeys.Clear();
        }

        listenToSubscription?.Dispose();
        disposables.Dispose();
    }

    #endregion
}
