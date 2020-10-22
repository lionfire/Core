using LionFire.Execution;
using System.Threading.Tasks;
using System.Linq;
using LionFire.Composables;

namespace LionFire.Execution
{
    public static class ComposableExecutionHelpers
    {
        public static async Task StartChildren(this IComposition composition)
        {
            if (composition == null) return;
            foreach (var child in composition.Children.OfType<IStartable>())
            {
                await child.StartAsync();
            }

        }
    }
}
