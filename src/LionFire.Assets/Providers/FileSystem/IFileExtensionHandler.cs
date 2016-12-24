using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Assets.Providers.FileSystem
{
    public interface IFileExtensionHandler
    {
        IEnumerable<string> FileExtensions { get; }
    }
}
