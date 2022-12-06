using LionFire.Persisters;
using LionFire.Persisters.Expanders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Persisters.SharpZipLib_;

public class SharpZipLibExpanderOptions : ExpanderOptions, ISupportsFileExtensions
{

    #region (static)

    public static List<string> FileExtensionsSupported { get; } = new List<string>
    {
        "zip",
        "tar",
        "bzip2",
        "bz2",
        "gzip",
    };

    #endregion

    #region Constants

    //public override Type? ServiceType => typeof(SharpZipLibExpander);

    #endregion

    public List<string> FileExtensions { get => fileExtensions ??= new(); set => fileExtensions = value; }
    private List<string>? fileExtensions;

}

public static class SharpZipLibExpanderOptionsX
{

    public static SharpZipLibExpanderOptions ConfigureDefault(this SharpZipLibExpanderOptions o)
    {
        o.FileExtensions = new List<string>(SharpZipLibExpanderOptions.FileExtensionsSupported);
        return o;
    }

}