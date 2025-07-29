using DryIoc;

namespace Runeforge.Engine.Instance;

public static class RuneforgeInstances
{
    public static IContainer Container { get; set; }

    public static TService GetService<TService>() where TService : class
    {
        return Container.Resolve<TService>();
    }
}
