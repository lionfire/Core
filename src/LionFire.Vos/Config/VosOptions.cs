using LionFire.Dependencies;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LionFire.ExtensionMethods;

namespace LionFire.Vos
{

    public class VosOptions
    {
        public Dictionary<string, RootVobOptions> NamedRootOptions { get; set; } = new Dictionary<string, RootVobOptions>();

        public RootVobOptions this[string rootName] => NamedRootOptions.GetOrAdd(rootName);

        public RootVobOptions DefaultRoot => NamedRootOptions.GetOrAdd(VosConstants.DefaultRootName);
        //{
        //    get
        //    {
        //        RootVobOptions rvo;
        //        if (!NamedRootOptions.ContainsKey(VosConstants.DefaultRootName))
        //        {
        //            NamedRootOptions.Add(VosConstants.DefaultRootName, rvo = new RootVobOptions());
        //            return rvo;
        //        }
        //        else
        //        {
        //            return NamedRootOptions[VosConstants.DefaultRootName];
        //        }
        //    }
        //}

        //public List<TMount> Mounts { get; set; } = new List<TMount>();

        public IEnumerable<string> RootNames { get; set; } = new List<string>() { "" };

        /// <summary>
        /// If false, requesting a Root of a name not in RootNames will throw an exception.
        /// If true,  requesting a Root of a name not in RootNames will work, but take note that it may not be initialized in the way that pre-known RootNames are.
        /// </summary>
        public bool AllowAdditionalRootNames { get; set; } 

        ///// <summary>
        ///// REVIEW: This used to be the default, but now I'd like to defer it until a startup is complete and it can be invoked explicitly via .
        ///// </summary>
        //public bool AutoInitRootVobs { get; set; }
    }

    //public static class VosOptionsExtensions
    //{
    //    public static IEnumerable<TMount> MountsForRootName(this VosOptions options, string rootName)
    //        => string.IsNullOrEmpty(rootName)
    //            ? options.Mounts.Where(m => m.Options == null || string.IsNullOrEmpty(m.Options.RootName))
    //            : options.Mounts.Where(m => m.Options != null && m.Options.RootName == rootName);
    //}

    //public class VosConfigurer : IDisposable
    //{
    //    IOptionsMonitor<VosOptions> vosOptions;
    //    RootVob rootVob;

    //    private IDisposable changeListener;


    //    public VosConfigurer(IOptionsMonitor<VosOptions> vosOptions, RootVob rootVob)
    //    {
    //        changeListener = vosOptions.OnChange(ApplyOptions);
    //        foreach (var x in vosOptions.)
    //    }

    //    public void ApplyOptions(VosOptions options, string rootName = null)
    //    {

    //    }

    //    public void Dispose() => throw new NotImplementedException();
    //}
}
