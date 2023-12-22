using Tgs.ServiceUtility;

namespace Tgs.GameInterface.VRGaming.Core.ApplicationServices;

internal class PlayerCreateCommandService(IGameApiClient client) : ICommandService<PlayerCreateCommand>
{
    public ValueTask ExecuteAsync(
        PlayerCreateCommand command,
        CancellationToken cancellationToken = default)
        => client.CreateUserAsync(
            apiUrl: command.ApiUrl,
            version: command.Version,
            merchantid: command.Merchantid,
            deskey: command.Deskey,
            playerName: command.PlayerCode,
            cancellationToken: cancellationToken);
}
