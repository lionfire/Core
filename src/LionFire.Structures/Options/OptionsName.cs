using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Structures;

public class OptionsName
{
    #region (static)

    public const string DefaultName = "";
    public static readonly OptionsName Default = new OptionsName("");
    
    #endregion

    #region Construction

    public OptionsName() { }
    public OptionsName(string name)
    {
        Name = name;
    }

    public static implicit operator OptionsName(string name) => new OptionsName(name);
    public static implicit operator string(OptionsName optionsName) => optionsName.Name;

    #endregion

    public string Name { get; set; }
    public string NameOrDefault => Name ?? DefaultName;


}