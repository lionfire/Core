using LionFire.Referencing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.UI.Workspaces;

[FileExtension("r")]
public class WorkspaceReferenceItem
{
    public IReference? Reference { get; set; }

    public int Order { get; set; }

    public GridPosition? Position { get; set; }
}

public record GridPosition
{
    public int X { get; set; }
    public int Y { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }

}

