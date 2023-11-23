using LionFire.Referencing;
using LionFire.Vos;

namespace LionFire.Persistence.Persisters.Vos;

public class Handlers<TArgs>
{
    public List<Func<TArgs, Task>> List { get; set; }

    public void AddHandler(Func<TArgs, Task> handler)
    {
        List ??= new();
        List.Add(handler);
    }

    public async ValueTask Raise(TArgs args)
    {
        if (List == null) return;
        foreach (var handler in List)
        {
            await handler.Invoke(args).ConfigureAwait(false);
        }
    }
}
