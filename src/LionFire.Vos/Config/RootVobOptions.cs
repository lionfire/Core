using System.Collections.Generic;
using LionFire.Vos.Mounts;

namespace LionFire.Vos
{
    public class RootVobOptions
    {
        public List<TMount> Mounts { get; set; } = new List<TMount>();
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
