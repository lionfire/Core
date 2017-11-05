cd C:\Src\Core\bin\LionFire.Framework\%1\netstandard2.0

 C:\Users\ja\.nuget\packages\ILRepack\2.0.13\tools\ILRepack.exe  /out:LionFire.Framework.Pack.dll LionFire.Framework.dll LionFire.Base.dll LionFire.Structures.dll LionFire.MultiTyping.Abstractions.dll LionFire.Core.dll  LionFire.Environment.dll LionFire.Instantiating.Abstractions.dll LionFire.Execution.dll LionFire.Execution.Abstractions.dll LionFire.Extensions.Logging.dll LionFire.Assets.dll  LionFire.MultiTyping.Abstractions.dll LionFire.Assets.Abstractions.dll LionFire.Applications.Abstractions.dll LionFire.StateMachines.dll LionFire.StateMachines.Abstractions.dll 
 
 rem Microsoft.Extensions.DependencyInjection.dll Microsoft.Extensions.DependencyInjection.Abstractions.dll Microsoft.Extensions.Logging.Abstractions.dll Microsoft.Extensions.Logging.dll 

xcopy /y LionFire.Framework.Pack.dll C:\st\Investing-Bots\cAlgo\bin

