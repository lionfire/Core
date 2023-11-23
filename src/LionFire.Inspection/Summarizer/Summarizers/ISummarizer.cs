namespace LionFire.Summarizer;

public interface ISummarizer
{
    Summary Summarize(object obj) => obj?.ToString() ?? "(null)";
}



