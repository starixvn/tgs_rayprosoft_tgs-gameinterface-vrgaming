using NSubstitute;
using Tgs.GameInterface.VRGaming.Core.ApplicationServices;
using Tgs.GameInterface.VRGaming.Core.Models;

namespace Tgs.GameInterface.VRGaming.Core.UnitTest.ApplicationServices;

public class FundStatusGetServiceUnitTest
{
    [Fact]
    public async void Normal_Process()
    {
        var query = new FundStatusQuery(
            ApiUrl: new Uri("https://fe.vrbetdemo.com/"),
            Version: "1.0",
            MerchantId: "PEGA",
            Deskey: "8VF408JBV2B406488F2VH2X684T6R6DV",
            PlayerCode: "4ucmqplxb029",
            OrderId: "018c8f8f201f0000000107e7db655fdb",
            StartTime: new DateTimeOffset(2023, 12, 22, 3, 27, 00, TimeSpan.Zero),
            EndTime: new DateTimeOffset(2023, 12, 22, 3, 42, 00, TimeSpan.Zero));

        var fakeGameApiClient = Substitute.For<IGameApiClient>();

        _ = fakeGameApiClient.TransactionRecordAsync(
            apiUrl: Arg.Is(query.ApiUrl),
            version: Arg.Is(query.Version),
            merchantId: Arg.Is(query.MerchantId),
            deskey: Arg.Is(query.Deskey),
            playerName: Arg.Is(query.PlayerCode),
            serialNumber: Arg.Is(query.OrderId),
            startTime: Arg.Is(query.StartTime),
            endTime: Arg.Is(query.EndTime),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(true);

        var service = new FundStatusGetService(
            fakeGameApiClient);

        var actual = await service.GetAsync(query);

        Assert.True(actual);
    }
}
