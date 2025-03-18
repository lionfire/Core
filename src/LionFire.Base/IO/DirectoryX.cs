using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LionFire.IO;

public static class DirectoryAsync
{
    public static Task<bool> ExistsAsync(string path) => Task.Run(() => Directory.Exists(path));
}
