using DryIoc;
using Runeforge.Engine.Data.Internal;
using Runeforge.Engine.Data.Internal.Services;

namespace Runeforge.Engine.Extensions;

public static class ServiceRegistrationExtension
{
    public static IContainer AddService(
        this IContainer container, Type serviceType, Type implementationType, int priority = 0
    )
    {
        ArgumentNullException.ThrowIfNull(container);

        ArgumentNullException.ThrowIfNull(serviceType);

        ArgumentNullException.ThrowIfNull(implementationType);

        container.Register(serviceType, implementationType, Reuse.Singleton);

        container.AddToRegisterTypedList(new ServiceDefObject(serviceType, implementationType, priority));

        return container;
    }

    public static IContainer AddService(this IContainer container, Type serviceType, int priority = 0)
    {
        return AddService(container, serviceType, serviceType, priority);
    }
}
