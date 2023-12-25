namespace Tgs.GameInterface.VRGaming.Core.ApplicationServices;

public record GameWagerUnsettledQuery(
    Uri ApiUrl,
    string Version,
    string MerchantId,
    string Deskey,
    DateTimeOffset StartTime,
    DateTimeOffset EndTime,
    int PageNumber = 1,
    int PageSize = 1000);
