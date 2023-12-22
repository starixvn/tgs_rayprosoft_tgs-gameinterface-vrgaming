namespace Tgs.GameInterface.VRGaming.Core.ApplicationServices;

public record GameWagerQuery(
    Uri ApiUrl,
    string Version,
    string Merchantid,
    string Deskey,
    DateTimeOffset StartTime,
    DateTimeOffset EndTime,
    int PageNumber = 1,
    int PageSize = 1000);
