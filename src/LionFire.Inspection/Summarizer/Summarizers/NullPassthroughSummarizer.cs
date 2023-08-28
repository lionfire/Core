namespace LionFire.Summarizer;

public class NullPassthroughSummarizer : ISummarizer
{
    public Summary Summarize(object obj) => obj?.ToString();
}



