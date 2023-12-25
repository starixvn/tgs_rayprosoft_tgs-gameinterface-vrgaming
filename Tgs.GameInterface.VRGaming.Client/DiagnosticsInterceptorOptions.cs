using System.Diagnostics;

namespace Tgs.GameInterface.VRGaming.Client;

public class DiagnosticsInterceptorOptions
{
    public ActivitySource InterceptorActivitySource { get; set; } =
        new ActivitySource($"Tgs.GameInterface.V8Poker.Client.DiagnosticsInterceptor");
}
