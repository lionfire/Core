namespace LionFire.IO;

public static class FilesystemRetryPolicy
{
    public const string Default = "Filesystem-Retry-Timeout";

    public static class OnFileChange
    {
        public const string Slow = "OnFileChange-Slow";
        public const string Default = "OnFileChange";
        public const string Quick = "OnFileChange-Quick";
    }

    // Example:
    //private void ExampleUsage()
    //{
    //    ResiliencePipelineProvider<string> pipelineProvider = provider.GetRequiredService<ResiliencePipelineProvider<string>>();
    //    ResiliencePipeline pipeline = pipelineProvider.GetPipeline(FilesystemRetryPolicy.Default);
    //}
}
