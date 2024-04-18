namespace Tgs.GameInterface.VRGaming.Core.ApplicationServices;

public record PlayerCreateCommand(
	Uri ApiUrl,
	string Version,
	string MerchantId,
	string Deskey,
	string PlayerCode);
