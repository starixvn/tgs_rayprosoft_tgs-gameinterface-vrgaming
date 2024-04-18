namespace Tgs.GameInterface.VRGaming.Core.ApplicationServices;

public record PlayerBalanceResult(
	bool IsPlayerExist,
	decimal Balance);
