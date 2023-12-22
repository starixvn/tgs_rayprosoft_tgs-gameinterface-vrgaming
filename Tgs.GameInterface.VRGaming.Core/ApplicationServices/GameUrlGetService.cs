using Tgs.ServiceUtility;

namespace Tgs.GameInterface.VRGaming.Core.ApplicationServices;

public class GameUrlGetService(IGameApiClient client) : IGetService<GameUrlQuery, string>
{
    public ValueTask<string> GetAsync(
        GameUrlQuery query,
        CancellationToken cancellationToken = default)
        => client.LoginValidateAsync(
            apiUrl: query.ApiUrl,
            version: query.Version,
            merchantid: query.Merchantid,
            deskey: query.Deskey,
            playerName: query.PlayerCode,
            channelId: query.ExternalGameId,
            cancellationToken: cancellationToken);
}
