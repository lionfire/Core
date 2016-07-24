//css_args /ac
//css_nuget	Newtonsoft.Json
//css_ref e:\src\Core\src\LionFire.Analytics\bin\Debug\net461\lionFire.Analytics.dll

using System;
using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using LionFire.Analytics;

string root = @"C:\ProgramData\LionFire\TimeTracker\Data\TimeTracking\Default\Days\2016\01";
void main(string[] args)
{
	
	HierarchicalRollup.Rollup(root,null,null);
	
}