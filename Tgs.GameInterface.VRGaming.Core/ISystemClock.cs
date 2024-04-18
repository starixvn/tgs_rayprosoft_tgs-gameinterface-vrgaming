namespace Tgs.GameInterface.VRGaming.Core;

public interface ISystemClock
{
	DateTimeOffset CurrentTime { get; }
}
