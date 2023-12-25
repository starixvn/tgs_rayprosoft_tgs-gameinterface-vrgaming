namespace Tgs.GameInterface.VRGaming.Core.ApplicationServices;

public record PlayerLogoutCommand(
    Uri ApiUrl,
    string Version,
    string MerchantId,
    string Deskey,
    string PlayerCode);
