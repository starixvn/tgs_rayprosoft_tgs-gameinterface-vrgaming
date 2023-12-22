namespace Tgs.GameInterface.VRGaming.Core.ApplicationServices;

public record GameUrlQuery(
    Uri ApiUrl,
    string Version,
    string Merchantid,
    string Deskey,
    string PlayerCode,
    int ExternalGameId);
