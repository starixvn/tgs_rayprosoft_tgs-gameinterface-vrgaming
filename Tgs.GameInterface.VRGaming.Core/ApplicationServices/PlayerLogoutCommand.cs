namespace Tgs.GameInterface.VRGaming.Core.ApplicationServices;

public record PlayerLogoutCommand(
    Uri ApiUrl,
    string Version,
    string Merchantid,
    string Deskey,
    string PlayerCode);
