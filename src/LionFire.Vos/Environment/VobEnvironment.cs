using LionFire.MultiTyping;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Vos.Environment
{
    public class VobEnvironment : VobNodeBase<VobEnvironment>, IMultiTypable
    {
        public VobEnvironment(Vob vob) : base(vob)
        {
        }

        public bool Inherit { get; set; }

        public MultiType MultiType { get; set; }
        public IMultiTyped MultiTyped => MultiType;

        public Dictionary<string, object> OwnVariables { get; } = new Dictionary<string, object>();


        public void Add<T>(string key, T value)
            => OwnVariables.Add(key, value);

        public T Get<T>(string key)
        {
            if (OwnVariables.ContainsKey(key)) return (T)OwnVariables[key];
            if (Inherit)
            {
                var parent = this.ParentValue;
                if (parent != null) return parent.Get<T>(key);
            }
            return default;
        }
    }

}

namespace LionFire.FlexDictionary
{
    public interface IFlex
    {
        ConcurrentDictionary<string, object> FlexDictionary { get; }
    }
//#error NEXT: Not sure
    public class FlexMemberOptions
    {
        // When are options used?
        //  - Add - check to see if already exists.  If so, throw if AllowMultiple = false
        //  - GetOrAddDefault - use registered factory for type, or the default generic factory.
        public bool AllowMultiple { get; set; }
    }

    public static class FDExtensions
    {
        //public static void GetOrAdd<T>(this ConcurrentDictionary<string, object> dict, string key, T value)
        //{
        //    dict.GetOrAdd()
        //}

        #region Options

        #region Default

        public static Func<FlexMemberOptions> DefaultOptionsFactory = () => new FlexMemberOptions();

        public static Func<FlexMemberOptions> GetDefaultOptionsFactory(this IFlex flex)
            => (Func<FlexMemberOptions>)flex.FlexDictionary.GetOrAdd("_optionsFactory", DefaultOptionsFactory);

        #endregion

        public static string GetOptionsKey<T>() => $"_options:({typeof(T).FullName})";
        public static string GetOptionsKey<T>(string name) => $"_options:({typeof(T).FullName}){name}";

        public static bool TryGetOptions<T>(this IFlex flex, out FlexMemberOptions options)
        {
            var result = flex.FlexDictionary.TryGetValue(GetOptionsKey<T>(), out var o);
            options = (FlexMemberOptions)o;
            return result;
        }

        public static bool TryGetOptions<T>(this IFlex flex, string name, out FlexMemberOptions options)
        {
            var result = flex.FlexDictionary.TryGetValue(GetOptionsKey<T>(name), out var o);
            options = (FlexMemberOptions)o;
            return result;
        }

        public static FlexMemberOptions Options<T>(this IFlex flex)
            => (FlexMemberOptions)flex.FlexDictionary.GetOrAdd(GetOptionsKey<T>(), flex.GetDefaultOptionsFactory());
        public static FlexMemberOptions Options<T>(this IFlex flex, string name)
                   => (FlexMemberOptions)flex.FlexDictionary.GetOrAdd(GetOptionsKey<T>(name), flex.GetDefaultOptionsFactory());

        #endregion

        public static Func<T> DefaultFactory<T>() => () => Activator.CreateInstance<T>();

        public static string GetTypeKey<T>() => $"({typeof(T).FullName})";

        public static T AsType<T>(this IFlex flex) => flex.FlexDictionary.TryGetValue(GetTypeKey<T>(), out var v) ? (T)v : default;
        public static T AsTypeOrCreateDefault<T>(this IFlex flex, Func<T> factory = null) => (T) flex.FlexDictionary.GetOrAdd(GetTypeKey<T>(), (factory ?? DefaultFactory<T>())());
    }

}
