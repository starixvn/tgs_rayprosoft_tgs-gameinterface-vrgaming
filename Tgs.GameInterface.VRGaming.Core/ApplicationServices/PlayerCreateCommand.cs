namespace Tgs.GameInterface.VRGaming.Core.ApplicationServices;

public record PlayerCreateCommand(
    Uri ApiUrl,
    string Version,
    string Merchantid,
    string Deskey,
    string PlayerCode);
