using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.RPC.Grains;

[Alias("hello")]
[GenerateSerializer]
public class HelloMessage
{
    [Id(0)]
    public string? Message { get; set; } 
    [Id(1)]
    public int Number{ get; set; } 

    public HelloMessage() { } // Parameterless constructor for serialization
    public HelloMessage(string message)
    {
        Message = message;
    }
}
