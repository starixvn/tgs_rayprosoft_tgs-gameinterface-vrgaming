using NSubstitute;
using NSubstitute.ReceivedExtensions;
using Tgs.GameInterface.VRGaming.Core.ApplicationServices;

namespace Tgs.GameInterface.VRGaming.Core.UnitTest.ApplicationServices;

public class PlayerCreateCommandServiceUnitTest
{
	[Fact]
	public async void Normal_Process()
	{
		var command = new PlayerCreateCommand(
			ApiUrl: new Uri("https://fe.vrbetdemo.com/"),
			Version: "1.0",
			MerchantId: "PEGA",
			Deskey: "8VF408JBV2B406488F2VH2X684T6R6DV",
			PlayerCode: "4ucmqplxb029");

		var fakeGameApiClient = Substitute.For<IGameApiClient>();

		var service = new PlayerCreateCommandService(
			fakeGameApiClient);

		await service.ExecuteAsync(command);

		_ = fakeGameApiClient
			.Received(1)
			.CreateUserAsync(
			apiUrl: Arg.Is(command.ApiUrl),
			version: Arg.Is(command.Version),
			merchantId: Arg.Is(command.MerchantId),
			deskey: Arg.Is(command.Deskey),
			playerName: Arg.Is(command.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
