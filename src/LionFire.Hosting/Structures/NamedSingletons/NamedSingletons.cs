#nullable enable
using LionFire.Results;
using LionFire.Structures;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace LionFire.Structures
{

    public class NamedSingletons<TItem>
    {
        protected ConcurrentDictionary<string, TItem> Items { get; } = new();
        protected ConcurrentDictionary<string, Task<TItem>> ItemsBeingCreated { get; } = new();

        public IServiceProvider? ServiceProvider { get; set; }
        protected NamedSingletonProviderOptions<TItem> Options { get; set; }

        #region Construction

        public NamedSingletons(IServiceProvider serviceProvider, IOptionsMonitor<NamedSingletonProviderOptions<TItem>> defaultParameters)
        {
            ServiceProvider = serviceProvider;
            Options = defaultParameters.CurrentValue;
            var nsp = ServiceProvider?.GetService<NamedSingletonProvider<TItem>>();
            CreateFunction = (nsp == null ? null : (Func<string, object[], Task<TItem>>)nsp.Get) ?? DefaultCreateFunction;
        }

        #endregion

        #region Get/Set

        public TItem? this[string key]
        {
            get => Query(key, out var result) ? result : default;
            set
            {
                if (value == null)
                {
                    Items.TryRemove(key, out var _);
                }
                else
                {
                    Items.AddOrUpdate(key, value, (k, e) => value);
                }
            }
        }

        public bool Query(string key, out TItem result) => Items.TryGetValue(key, out result);


        public Task<TItem> GetOrCreateAsync(string key, params object[] parameters)
        {
            Task<TItem> queryResultOrCreateTask;

            lock (_lock)
            {
                queryResultOrCreateTask = Query(key, out var result) ? Task.FromResult(result) : ItemsBeingCreated.GetOrAdd(key, k =>
                {
                    var task = CreateFunction(k, parameters);
                    task.ContinueWith(r =>
                    {
                        if (!Items.TryAdd(key, r.Result)) throw new UnreachableCodeException();
                        if (!ItemsBeingCreated.TryRemove(key, out _)) throw new UnreachableCodeException();
                        return r;
                    });
                    return task;
                });
            }
            return queryResultOrCreateTask;
        }
        private object _lock = new();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="item"></param>
        /// <returns>true if succeeded or was already set to the specified item, false if failed because it was already set</returns>
        public bool TrySet(string key, TItem item) => Items.TryAdd(key, item);

        #endregion

        public Func<string, object[], Task<TItem>> CreateFunction { get; set; }

        private Task<TItem> DefaultCreateFunction(string key, params object[] parameters)
        {
            return NamedSingletonProviderUtils<TItem>.DefaultGet(key, parameters, Options, ServiceProvider);
        }
    }

}
