using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GameInterface.VRGaming.Core.ApplicationServices;
using Tgs.GameInterface.VRGaming.Core.Models;

namespace Tgs.GameInterface.VRGaming.Core.UnitTest.ApplicationServices;

public class GameWagerUnsettledQueryServiceUnitTest
{
    [Fact]
    public async void Normal_Process()
    {
        var query = new GameWagerUnsettledQuery(
            ApiUrl: new Uri("https://fe.vrbetdemo.com/"),
            Version: "1.0",
            MerchantId: "PEGA",
            Deskey: "8VF408JBV2B406488F2VH2X684T6R6DV",
            StartTime: DateTimeOffset.Parse("2023-12-25T01:20:00Z"),
            EndTime: DateTimeOffset.Parse("2023-12-25T01:30:01Z"),
            PageNumber: 1,
            PageSize: 1000);

        var fakeGameApiClient = Substitute.For<IGameApiClient>();

        var responses = new MerchantQueryResponse[] {
            new(
                IssueNumber:"20231225017",
                SerialNumber:"11225092401078552627",
                ChannelId:"1",
                PlayerName:"4ucmqplxb029",
                State:0,
                SubState:0,
                Cost:1.0m,
                PlayerPrize:0.0m,
                LossPrize:0.0m,
                CreateTime:DateTimeOffset.Parse("2023-12-25T01:24:01Z"),
                UpdateTime:DateTimeOffset.Parse("2023-12-25T01:24:01Z"),
                AdditionalData:"{\"recordCountPerPage\":1000,\"recordPage\":0,\"totalRecords\":1,\"betRecords\":[{\"cost\":1.0,\"unit\":1.0,\"lossPrize\":0.0,\"playerPrize\":0.0,\"merchantPrize\":0.0,\"state\":0,\"betTypeCode\":146,\"playerOdds\":1980,\"count\":1,\"multiple\":1,\"channelId\":1,\"subState\":0,\"prizeDetail\":[],\"ip\":\"220.133.218.203\",\"updateTime\":null,\"createTime\":\"2023-12-25T01:24:01Z\",\"note\":\"\",\"winningNumber\":null,\"issueNumber\":\"20231225017\",\"betTypeName\":\"五星总和大小单双\",\"channelName\":\"VR金星1.5分彩\",\"channelCode\":\"VRSSCA\",\"position\":\"万,千,百,十,个\",\"number\":\"总和小\",\"odds\":\"1.97\",\"playerName\":\"4ucmqplxb029\",\"serialNumber\":\"11225092401078552627\",\"merchantCode\":\"PEGA\"}]}"
                )};
        _ = fakeGameApiClient
            .MerchantQueryBetAsync(
            apiUrl: Arg.Is(query.ApiUrl),
            version: Arg.Is(query.Version),
            merchantId: Arg.Is(query.MerchantId),
            deskey: Arg.Is(query.Deskey),
            isUpdateTime: Arg.Is(false),
            state: Arg.Is(MerchantQueryBetStates.Unsettled),
            startTime: Arg.Is(query.StartTime),
            endTime: Arg.Is(query.EndTime),
            recordPage: Arg.Is(query.PageNumber - 1),
            recordCountPerPage: Arg.Is(query.PageSize),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(responses.ToAsyncEnumerable());

        var service = new GameWagerUnsettledQueryService(
            fakeGameApiClient);

        var actual = await service.QueryAsync(query)
            .ToArrayAsync()
            ?? Enumerable.Empty<GameWagerResult>();

        Assert.True(actual.Any());
        Assert.Equal("20231225017", actual.Single().ExternalRoundId);
        Assert.Equal("11225092401078552627", actual.Single().ExternalWagerId);
        Assert.Equal("1", actual.Single().ExternalGameId);
        Assert.Equal("4ucmqplxb029", actual.Single().PlayerCode);
        Assert.False(actual.Single().IsClosed);
        Assert.False(actual.Single().IsCanceled);
        Assert.False(actual.Single().IsResettle);
        Assert.Equal(DateTimeOffset.Parse("2023-12-25T01:24:01Z"), actual.Single().ExternalCreatedOn);
        Assert.Equal(DateTimeOffset.UnixEpoch, actual.Single().ExternalClosedOn);
        Assert.Equal(DateTimeOffset.UnixEpoch, actual.Single().ExternalResettleOn);
        Assert.Equal(1.0m, actual.Single().Bet);
        Assert.Equal(0.0m, actual.Single().Settle);
        Assert.Equal(0.0m, actual.Single().WinLoss);
        Assert.Equal(responses.Single().AdditionalData, actual.Single().AdditionalData);
    }

    [Fact]
    public async void No_data()
    {
        var query = new GameWagerUnsettledQuery(
            ApiUrl: new Uri("https://fe.vrbetdemo.com/"),
            Version: "1.0",
            MerchantId: "PEGA",
            Deskey: "8VF408JBV2B406488F2VH2X684T6R6DV",
            StartTime: DateTimeOffset.Parse("2023-12-25T01:20:00Z"),
            EndTime: DateTimeOffset.Parse("2023-12-25T01:30:01Z"),
            PageNumber: 1,
            PageSize: 1000);

        var fakeGameApiClient = Substitute.For<IGameApiClient>();

        _ = fakeGameApiClient
            .MerchantQueryBetAsync(
            apiUrl: Arg.Is(query.ApiUrl),
            version: Arg.Is(query.Version),
            merchantId: Arg.Is(query.MerchantId),
            deskey: Arg.Is(query.Deskey),
            isUpdateTime: Arg.Is(false),
            state: Arg.Is(MerchantQueryBetStates.Unsettled),
            startTime: Arg.Is(query.StartTime),
            endTime: Arg.Is(query.EndTime),
            recordPage: Arg.Is(query.PageNumber - 1),
            recordCountPerPage: Arg.Is(query.PageSize),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(Enumerable.Empty<MerchantQueryResponse>().ToAsyncEnumerable());

        var service = new GameWagerUnsettledQueryService(
            fakeGameApiClient);

        var actual = await service.QueryAsync(query)
            .ToArrayAsync()
            ?? Enumerable.Empty<GameWagerResult>();

        Assert.False(actual.Any());
    }
}
