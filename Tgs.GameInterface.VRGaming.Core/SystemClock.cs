using Tgs.GameInterface.VRGaming.Core;

namespace Tgs.GameInterface.VRGaming.Core;

internal class SystemClock : ISystemClock
{
    public DateTimeOffset CurrentTime => DateTimeOffset.UtcNow;
}
