using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Vfs.Conventions;

public class Types
{
    /// <summary>
    /// Type Extension can be omitted if it matches DefaultType
    /// </summary>
    public string? DefaultType { get; set; }
    public bool WhitelistEnabled { get; set; }
    public List<string>? AllowedTypes { get; set; }
    public List<string>? SuggestedTypes { get; set; }
}
