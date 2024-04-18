namespace Tgs.GameInterface.VRGaming.Core.ApplicationServices;

public record PlayerBalanceQuery(
	Uri ApiUrl,
	string Version,
	string MerchantId,
	string Deskey,
	string PlayerCode);
