using NSubstitute;
using Tgs.GameInterface.VRGaming.Core.ApplicationServices;
using Tgs.GameInterface.VRGaming.Core.Models;

namespace Tgs.GameInterface.VRGaming.Core.UnitTest.ApplicationServices;

public class PlayerBalanceGetServiceUnitTest
{
	[Fact]
	public async void Normal_Process()
	{
		var query = new PlayerBalanceQuery(
			ApiUrl: new Uri("https://fe.vrbetdemo.com/"),
			Version: "1.0",
			MerchantId: "PEGA",
			Deskey: "8VF408JBV2B406488F2VH2X684T6R6DV",
			PlayerCode: "4ucmqplxb029");

		var fakeGameApiClient = Substitute.For<IGameApiClient>();

		var response = new BalanceResponse(
			IsPlayerExist: true,
			Balance: 1.0m);
		_ = fakeGameApiClient.BalanceAsync(
			apiUrl: Arg.Is(query.ApiUrl),
			version: Arg.Is(query.Version),
			merchantId: Arg.Is(query.MerchantId),
			deskey: Arg.Is(query.Deskey),
			playerName: Arg.Is(query.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(response);

		var service = new PlayerBalanceGetService(
			fakeGameApiClient);

		var actual = await service.GetAsync(query);

		Assert.Equal(response.IsPlayerExist, actual.IsPlayerExist);
		Assert.Equal(response.Balance, actual.Balance);
	}
}
