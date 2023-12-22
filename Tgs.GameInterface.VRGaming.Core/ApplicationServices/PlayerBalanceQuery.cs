namespace Tgs.GameInterface.VRGaming.Core.ApplicationServices;

public record PlayerBalanceQuery(
    Uri ApiUrl,
    string Version,
    string Merchantid,
    string Deskey,
    string PlayerCode);
