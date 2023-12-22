namespace Tgs.GameInterface.VRGaming.Core.ApplicationServices;

public record FundOutQuery(
    Uri ApiUrl,
    string Version,
    string Merchantid,
    string Deskey,
    string PlayerCode,
    string OrderId,
    decimal Amount);
