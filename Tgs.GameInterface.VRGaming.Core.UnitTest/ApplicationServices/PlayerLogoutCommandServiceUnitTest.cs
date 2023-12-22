using NSubstitute;
using NSubstitute.ReceivedExtensions;
using Tgs.GameInterface.VRGaming.Core.ApplicationServices;

namespace Tgs.GameInterface.VRGaming.Core.UnitTest.ApplicationServices;

public class PlayerLogoutCommandServiceUnitTest
{
    [Fact]
    public async void Normal_Process()
    {
        var command = new PlayerLogoutCommand(
            ApiUrl: new Uri("https://fe.vrbetdemo.com/"),
            Version: "1.0",
            Merchantid: "PEGA",
            Deskey: "8VF408JBV2B406488F2VH2X684T6R6DV",
            PlayerCode: "4ucmqplxb029");

        var fakeGameApiClient = Substitute.For<IGameApiClient>();

        var service = new PlayerLogoutCommandService(
            fakeGameApiClient);

        await service.ExecuteAsync(command);

        _ = fakeGameApiClient
            .Received(1)
            .KickUserAsync(
            apiUrl: Arg.Is(command.ApiUrl),
            version: Arg.Is(command.Version),
            merchantid: Arg.Is(command.Merchantid),
            deskey: Arg.Is(command.Deskey),
            playerName: Arg.Is(command.PlayerCode),
            cancellationToken: Arg.Any<CancellationToken>());
    }
}
