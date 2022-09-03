#nullable enable
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Execution;

public delegate void OnProgressDelegate(int completed, int total);

public class ProgressListener
{
    public ProgressListener() { }
    public ProgressListener(OnProgressDelegate onProgress) { OnProgress = onProgress; }
    public OnProgressDelegate? OnProgress { get; set; }
}
