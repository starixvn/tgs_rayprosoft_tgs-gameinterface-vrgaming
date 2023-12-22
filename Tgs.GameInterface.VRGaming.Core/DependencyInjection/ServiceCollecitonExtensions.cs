using System.Reflection;

using Tgs.GameInterface.VRGaming.Core;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollecitonExtensions
{
    public static IServiceCollection AddGameInterfaceCore(
        this IServiceCollection services)
        => services
        .RegisterAllServiceUtilityTypes(
            Assembly.GetExecutingAssembly(),
            ServiceLifetime.Transient)
        .AddHttpClient<IGameApiClient, GameApiClient>()
        .Services
        .AddSingleton<IAesCryptographyUtensil, AesCryptographyUtensil>()
        .AddSingleton<ISystemClock, SystemClock>();
}
