//css_args /ac
using System;
using System.IO;
using System.Diagnostics;


void main(string[] args)
{
	
	Run("start TestScript.csx");
	
	
	Run("start LionFire.Trading.Feeds.TrueFx[QuoteReceiver]");

	Run("start LionFire.Execution.Utilities/[Count");   
	
	Run("start https://gist.githubusercontent.com/jaredthirsk/91660b3f6c1e4174a9698029458967f8/raw/4e1f0f9d0f5086b43660427abb73a18721eed3a9/Environment.SpecialFolders.csscript.cs");
	
}

void Run(string arg)
{
	var path = @"E:\bin\lx.cmd";
	var psi = new ProcessStartInfo("cmd.exe",  "/c " + path + " " + arg);
	psi.CreateNoWindow = true;
	psi.UseShellExecute = false;
	psi.RedirectStandardOutput = true;
	psi.RedirectStandardError  = true;
	var p = Process.Start(psi);
	
	p.OutputDataReceived += (object sender, DataReceivedEventArgs e) =>
        Console.WriteLine("> " + e.Data);
	p.BeginOutputReadLine();
	
	p.WaitForExit();	
}