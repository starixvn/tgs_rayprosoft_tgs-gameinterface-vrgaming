using Tgs.ServiceUtility;

namespace Tgs.GameInterface.VRGaming.Core.ApplicationServices;

internal class PlayerLogoutCommandService(IGameApiClient client) : ICommandService<PlayerLogoutCommand>
{
    public ValueTask ExecuteAsync(
        PlayerLogoutCommand command,
        CancellationToken cancellationToken = default)
        => client.KickUserAsync(
            apiUrl: command.ApiUrl,
            version: command.Version,
            merchantid: command.Merchantid,
            deskey: command.Deskey,
            playerName: command.PlayerCode,
            cancellationToken: cancellationToken);
}
