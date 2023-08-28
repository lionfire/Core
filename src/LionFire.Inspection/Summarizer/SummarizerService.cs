using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Summarizer;

public interface ISummarizerService
{
    Summary Summarize(object obj)
    {
        foreach (var summarizer in Summarizers)
        {
            var result = summarizer.Summarize(obj);
            if (result != null) return result;
        }
        return null;
    }

    public IEnumerable<ISummarizer> Summarizers { get; }

}

public class SummarizerService : ISummarizerService
{
    public SummarizerService(IEnumerable<ISummarizer> summarizers)
    {
        Summarizers = summarizers;
    }

    public IEnumerable<ISummarizer> Summarizers { get; }
}

