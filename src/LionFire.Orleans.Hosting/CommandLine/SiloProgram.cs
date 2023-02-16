using LionFire.Hosting;
using System.CommandLine;

[Obsolete("TODO: Composition alternative")]
public class SiloProgram : SiloProgram<SiloProgram> { }

[Obsolete("TODO: Composition alternative")]
public class SiloProgram<TConcrete> : RunnableCommandLineProgram<TConcrete>
     where TConcrete : SiloProgram<TConcrete> 
{
    protected override void OnBuildingCommandLine(RootCommand root)
    {
        root.AddOrleansCommands(this);
        base.OnBuildingCommandLine(root);
    }
}
