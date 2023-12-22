namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGameInterface(
        this IServiceCollection services)
        => services
        .AddGameInterfaceCore();
}
