using NSubstitute;
using Tgs.GameInterface.VRGaming.Core.ApplicationServices;
using Tgs.GameInterface.VRGaming.Core.Models;

namespace Tgs.GameInterface.VRGaming.Core.UnitTest.ApplicationServices;

public class FundInGetServiceUnitTest
{
	[Fact]
	public async void Normal_Process()
	{
		var query = new FundInQuery(
			ApiUrl: new Uri("https://fe.vrbetdemo.com/"),
			Version: "1.0",
			MerchantId: "PEGA",
			Deskey: "8VF408JBV2B406488F2VH2X684T6R6DV",
			PlayerCode: "4ucmqplxb029",
			OrderId: "018c8f8ee6a60000000107e7db655fdb",
			Amount: 1.0m);

		var fakeGameApiClient = Substitute.For<IGameApiClient>();

		var response = new TransactionResponse(
			SerialNumber: "018c8f8ee6a60000000107e7db655fdb",
			Resp: "{\"serialNumber\":\"018c8f8ee6a60000000107e7db655fdb\",\"playerName\":\"4ucmqplxb029\",\"type\":1,\"amount\":1.0,\"createTime\":\"2023-12-22T03:33:53Z\",\"state\":0,\"balance\":0.000}"
			);
		_ = fakeGameApiClient.WithdrawAsync(
			apiUrl: Arg.Is(query.ApiUrl),
			version: Arg.Is(query.Version),
			merchantId: Arg.Is(query.MerchantId),
			deskey: Arg.Is(query.Deskey),
			playerName: Arg.Is(query.PlayerCode),
			serialNumber: Arg.Is(query.OrderId),
			amount: Arg.Is(query.Amount),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(response);

		var service = new FundInGetService(
			fakeGameApiClient);

		var actual = await service.GetAsync(query);

		Assert.Equal(response.SerialNumber, actual.OrderId);
		Assert.Equal(response.Resp, actual.Resp);
	}
}
