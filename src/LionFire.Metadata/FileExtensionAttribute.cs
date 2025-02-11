using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire;

/// <summary>
/// (Virtual) File extension
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface)]
public class FileExtensionAttribute : Attribute
{

    public FileExtensionAttribute(string extension)
    {
        Extension = extension;
    }

    public string Extension { get; }

}