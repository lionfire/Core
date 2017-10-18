#r "LionFire.Environment"

using System;
using LionFire;
using LionFire.Messaging;

public class ScriptedClass
{
    public String HelloWorld { get; set; }
    public ScriptedClass()
    {
        LionFireEnvironment.ProgramName = "ScriptProgramName";
        HelloWorld = "Hello Roslyn! Test:" + typeof(MBus).Name;
        Console.WriteLine("Hello from inside script csx resource");
    }
}
var x = new ScriptedClass();
return x.HelloWorld;