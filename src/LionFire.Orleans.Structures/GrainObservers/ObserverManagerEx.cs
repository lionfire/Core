using Orleans.Runtime;
using System.Collections;
using Microsoft.Extensions.Logging;
using DynamicData;

namespace LionFire.Orleans_.Utilities;

// Based on Orleans' ObserverManager as of 2024-10-10

/// <summary>
/// Maintains a collection of observers.
///
/// Ex edition adds:
/// - Observers is now an IObservableCache (from DynamicData)
/// 
/// </summary>
/// <typeparam name="TObserver">
/// The observer type.
/// </typeparam>
public class ObserverManagerEx<TObserver> : ObserverManagerEx<IAddressable, TObserver>
    where TObserver : IAddressable
{

    #region Lifecycle

    /// <summary>
    /// Initializes a new instance of the <see cref="ObserverManagerEx{TObserver}"/> class. 
    /// </summary>
    /// <param name="expiration">
    /// The expiration.
    /// </param>
    /// <param name="log">The log.</param>
    public ObserverManagerEx(TimeSpan expiration, ILogger log, Func<TObserver, IAddressable> keySelector) : base(expiration, log, keySelector)
    {
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="ObserverManagerEx{TObserver}"/> class. 
    /// </summary>
    /// <param name="expiration">
    /// The expiration.
    /// </param>
    /// <param name="log">The log.</param>
    public ObserverManagerEx(TimeSpan expiration, ILogger log, SourceCache<IObserverEntry<TObserver>, IAddressable> sourceCache) : base(expiration, log, sourceCache)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ObserverManagerEx{TObserver}"/> class. 
    /// </summary>
    /// <param name="expiration">
    /// The expiration.
    /// </param>
    /// <param name="log">The log.</param>
    public ObserverManagerEx(TimeSpan expiration, ILogger log) : base(expiration, log, o => o)
    {
    }

    #endregion
}

/// <summary>
/// Maintains a collection of observers.
///
/// Ex edition adds:
/// - OnDefunct event
/// </summary>
/// <typeparam name="TIdentity">
/// The address type, used to identify observers.
/// </typeparam>
/// <typeparam name="TObserver">
/// The observer type.
/// </typeparam>
public class ObserverManagerEx<TIdentity, TObserver> : IEnumerable<TObserver> where TIdentity : notnull
{
    ///// <summary>
    ///// Gets a copy of the observers.
    ///// </summary>
    //public IDictionary<TIdentity, TObserver> Observers => _observers.ToDictionary(_ => _.Key, _ => _.Value.Observer);


    /// <summary>
    /// The observers.
    /// </summary>
    public IObservableCache<IObserverEntry<TObserver>, TIdentity> Observers => _observers;
    private readonly SourceCache<IObserverEntry<TObserver>, TIdentity> _observers;
    //private readonly Dictionary<TIdentity, ObserverEntry> _observers = new();

    /// <summary>
    /// The log.
    /// </summary>
    private readonly ILogger _log;

    /// <summary>
    /// Initializes a new instance of the <see cref="ObserverManagerEx{TIdentity,TObserver}"/> class. 
    /// </summary>
    /// <param name="expiration">
    /// The expiration.
    /// </param>
    /// <param name="log">The log.</param>
    public ObserverManagerEx(TimeSpan expiration, ILogger log, Func<TObserver, TIdentity> keySelector) : this(expiration, log, sourceCache: new(o => keySelector(o.Observer)))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ObserverManagerEx{TIdentity,TObserver}"/> class. 
    /// </summary>
    /// <param name="expiration">
    /// The expiration.
    /// </param>
    /// <param name="log">The log.</param>
    public ObserverManagerEx(TimeSpan expiration, ILogger log, SourceCache<IObserverEntry<TObserver>, TIdentity> sourceCache)
    {
        ExpirationDuration = expiration;
        _log = log ?? throw new ArgumentNullException(nameof(log));
        GetDateTime = () => DateTime.UtcNow;
        _observers = sourceCache;
    }

    /// <summary>
    /// Gets or sets the delegate used to get the date and time, for expiry.
    /// </summary>
    public Func<DateTime> GetDateTime { get; set; }

    /// <summary>
    /// Gets or sets the expiration time span, after which observers are lazily removed.
    /// </summary>
    public TimeSpan ExpirationDuration { get; set; }

    /// <summary>
    /// Gets the number of observers.
    /// </summary>
    public int Count => _observers.Count;


    /// <summary>
    /// Removes all observers.
    /// </summary>
    public void Clear() => _observers.Clear();

    /// <summary>
    /// Ensures that the provided <paramref name="observer"/> is subscribed, renewing its subscription.
    /// </summary>
    /// <param name="id">
    /// The observer's identity.
    /// </param>
    /// <param name="observer">
    /// The observer.
    /// </param>
    /// <exception cref="Exception">A delegate callback throws an exception.</exception>
    public void Subscribe(TIdentity id, TObserver observer)
    {
        // Add or update the subscription.
        var now = GetDateTime();
        var entryMaybe = _observers.Lookup(id);

        if (entryMaybe.HasValue)
        {
            var entry = (ObserverEntry)entryMaybe.Value;
            entry.LastSeen = now;
            entry.Observer = observer;
            if (_log.IsEnabled(LogLevel.Debug))
            {
                _log.LogDebug("Updating entry for {Id}/{Observer}. {Count} total observers.", id, observer, _observers.Count);
            }
        }
        else
        {
            _observers.AddOrUpdate(new ObserverEntry(observer, now));
            if (_log.IsEnabled(LogLevel.Debug))
            {
                _log.LogDebug("Adding entry for {Id}/{Observer}. {Count} total observers after add.", id, observer, _observers.Count);
            }
        }
    }

    /// <summary>
    /// Ensures that the provided <paramref name="id"/> is unsubscribed.
    /// </summary>
    /// <param name="id">
    /// The observer.
    /// </param>
    public void Unsubscribe(TIdentity id)
    {
        _observers.Remove(id);
        if (_log.IsEnabled(LogLevel.Debug))
        {
            _log.LogDebug("Removed entry for {Id}. {Count} total observers after remove.", id, _observers.Count);
        }
    }

    public void CheckForDefunct()
    {
        var now = GetDateTime();
        var defunct = default(List<KeyValuePair<TIdentity, IObserverEntry<TObserver>>>);

        foreach (var observer in _observers.KeyValues)
        {
            if (observer.Value.LastSeen + ExpirationDuration < now)
            {
                // Expired observers will be removed.
                defunct ??= new();
                defunct.Add(observer);
                continue;
            }
        }

        if (defunct != default) { RemoveDefunct(defunct); }
    }

    // TODO: Use this everywhere defunct are removed
    // TODO: change parameter type to IEnumerable<TIdentity>
    private void RemoveDefunct(IEnumerable<KeyValuePair<TIdentity, IObserverEntry<TObserver>>> defunctEntries)
    {
        foreach (var kvp in defunctEntries)
        {
            _observers.Remove(kvp.Key);
            if (_log.IsEnabled(LogLevel.Debug))
            {
                _log.LogDebug("Removing defunct entry for {Id}. {Count} total observers after remove.", kvp.Key, _observers.Count);
            }
        }
        OnDefunct?.Invoke(defunctEntries.Select(kvp => kvp.Key));

    }

    /// <summary>
    /// Notifies all observers.
    /// </summary>
    /// <param name="notification">
    /// The notification delegate to call on each observer.
    /// </param>
    /// <param name="predicate">
    /// The predicate used to select observers to notify.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the work performed.
    /// </returns>
    public async Task Notify(Func<TObserver, Task> notification, Func<TObserver, bool> predicate = null)
    {
        var now = GetDateTime();
        var defunct = default(List<TIdentity>);
        foreach (var observer in _observers.KeyValues)
        {
            if (observer.Value.LastSeen + ExpirationDuration < now)
            {
                // Expired observers will be removed.
                defunct ??= new List<TIdentity>();
                defunct.Add(observer.Key);
                continue;
            }

            // Skip observers which don't match the provided predicate.
            if (predicate != null && !predicate(observer.Value.Observer))
            {
                continue;
            }

            try
            {
                await notification(observer.Value.Observer);
            }
            catch (Exception)
            {
                // Failing observers are considered defunct and will be removed..
                defunct ??= new List<TIdentity>();
                defunct.Add(observer.Key);
            }
        }

        // Remove defunct observers.
        if (defunct != default(List<TIdentity>))
        {
            foreach (var observer in defunct)
            {
                _observers.Remove(observer);
                if (_log.IsEnabled(LogLevel.Debug))
                {
                    _log.LogDebug("Removing defunct entry for {Id}. {Count} total observers after remove.", observer, _observers.Count);
                }
            }
        }
    }

    /// <summary>
    /// Notifies all observers which match the provided <paramref name="predicate"/>.
    /// </summary>
    /// <param name="notification">
    /// The notification delegate to call on each observer.
    /// </param>
    /// <param name="predicate">
    /// The predicate used to select observers to notify.
    /// </param>
    public void Notify(Action<TObserver> notification, Func<TObserver, bool> predicate = null)
    {
        var now = GetDateTime();
        var defunct = default(List<TIdentity>);
        foreach (var observer in _observers.KeyValues)
        {
            if (observer.Value.LastSeen + ExpirationDuration < now)
            {
                // Expired observers will be removed.
                defunct ??= new List<TIdentity>();
                defunct.Add(observer.Key);
                continue;
            }

            // Skip observers which don't match the provided predicate.
            if (predicate != null && !predicate(observer.Value.Observer))
            {
                continue;
            }

            try
            {
                notification(observer.Value.Observer);
            }
            catch (Exception)
            {
                // Failing observers are considered defunct and will be removed..
                defunct ??= new List<TIdentity>();
                defunct.Add(observer.Key);
            }
        }

        // Remove defunct observers.
        if (defunct != default(List<TIdentity>))
        {
            foreach (var observer in defunct)
            {
                _observers.Remove(observer);
                if (_log.IsEnabled(LogLevel.Debug))
                {
                    _log.LogDebug("Removing defunct entry for {Id}. {Count} total observers after remove.", observer, _observers.Count);
                }
            }
        }
    }

    public Action<IEnumerable<TIdentity>>? OnDefunct { get; set; }

    /// <summary>
    /// Removed all expired observers.
    /// </summary>
    public void ClearExpired()
    {
        var now = GetDateTime();
        var defunct = default(List<TIdentity>);
        foreach (var observer in _observers.KeyValues)
        {
            if (observer.Value.LastSeen + ExpirationDuration < now)
            {
                // Expired observers will be removed.
                defunct ??= new List<TIdentity>();
                defunct.Add(observer.Key);
            }
        }

        // Remove defunct observers.
        if (defunct is { Count: > 0 })
        {
            _log.LogInformation("Removing {Count} defunct observers entries.", defunct.Count);
            //OnDefunct?.Invoke(defunct); // Do this before removing, so that the observer can look up values if desired
            foreach (var observer in defunct)
            {
                _observers.Remove(observer);
            }
        }
    }

    /// <summary>
    /// Returns an enumerator that iterates through the collection.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
    /// </returns>
    public IEnumerator<TObserver> GetEnumerator() => _observers.KeyValues.Select(observer => observer.Value.Observer).GetEnumerator();

    /// <summary>
    /// Returns an enumerator that iterates through a collection.
    /// </summary>
    /// <returns>
    /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
    /// </returns>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// An observer entry.
    /// </summary>
    public class ObserverEntry : IObserverEntry<TObserver>
    {
        public ObserverEntry(TObserver observer, DateTime lastSeen)
        {
            Observer = observer;
            LastSeen = lastSeen;
        }

        /// <summary>
        /// Gets or sets the observer.
        /// </summary>
        public TObserver Observer { get; internal set; }

        /// <summary>
        /// Gets or sets the UTC last seen time.
        /// </summary>
        public DateTime LastSeen { get; internal set; }
    }
}

public interface IObserverEntry<TObserver>
{
    TObserver Observer
    {
        get;
    }
    DateTime LastSeen
    {
        get;
    }
}

