namespace LionFire.Summarizer;

public class DefaultSummarizer : ISummarizer
{
    public Summary Summarize(object obj) => obj?.ToString() ?? "(null)";
}



