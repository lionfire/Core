namespace LionFire.Summarizer;

public class Summary
{
    #region Lifecycle

    public static implicit operator string?(Summary summary) => summary?.Text;
    public static implicit operator Summary(string? text) => new Summary { Text = text };

    #endregion

    public bool IsDeferred { get; set; }
    public bool HasSideEffects { get; set; }

    public Func<Task>? Evaluate { get; set; }

    public string? Text { get; set; }
}



