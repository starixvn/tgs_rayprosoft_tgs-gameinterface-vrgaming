namespace Tgs.GameInterface.VRGaming.Core.ApplicationServices;

public record FundStatusQuery(
    Uri ApiUrl,
    string Version,
    string Merchantid,
    string Deskey,
    string PlayerCode,
    string OrderId,
    DateTimeOffset StartTime,
    DateTimeOffset EndTime);
