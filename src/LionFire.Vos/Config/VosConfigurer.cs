using LionFire.Dependencies;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using LionFire.Vos.Mounts;
using System.Text;
using LionFire.ExtensionMethods;

namespace LionFire.Vos
{
    public class RootVobOptions
    {
        public List<TMount> Mounts { get; set; } = new List<TMount>();
    }

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
