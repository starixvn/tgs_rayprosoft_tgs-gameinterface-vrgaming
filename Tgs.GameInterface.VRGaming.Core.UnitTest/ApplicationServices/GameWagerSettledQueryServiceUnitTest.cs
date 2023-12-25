using System;
using System.Threading;
using System.Threading.Channels;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Tgs.GameInterface.VRGaming.Core.ApplicationServices;
using Tgs.GameInterface.VRGaming.Core.Models;

namespace Tgs.GameInterface.VRGaming.Core.UnitTest.ApplicationServices;

public class GameWagerSettledQueryServiceUnitTest
{
    [Fact]
    public async void Winning()
    {
        var query = new GameWagerSettledQuery(
            ApiUrl: new Uri("https://fe.vrbetdemo.com/"),
            Version: "1.0",
            MerchantId: "PEGA",
            Deskey: "8VF408JBV2B406488F2VH2X684T6R6DV",
            StartTime: DateTimeOffset.Parse("2023-12-22T07:08:00Z"),
            EndTime: DateTimeOffset.Parse("2023-12-22T07:13:00Z"),
            PageNumber: 1,
            PageSize: 1000);

        var fakeGameApiClient = Substitute.For<IGameApiClient>();

        var responses = new MerchantQueryResponse[] {
            new(
                IssueNumber:"20231222246",
                SerialNumber:"11222150835182930660",
                ChannelId:"142",
                PlayerName:"4ucmqplxb029",
                State:3,
                SubState:0,
                Cost:1.0m,
                PlayerPrize:1.97m,
                LossPrize:0.0m,
                CreateTime:DateTimeOffset.Parse("2023-12-22T07:08:35Z"),
                UpdateTime:DateTimeOffset.Parse("2023-12-22T07:09:59Z"),
                AdditionalData:"{\"cost\":1.0,\"unit\":1.0,\"lossPrize\":0.0,\"playerPrize\":1.97,\"merchantPrize\":1.97,\"state\":3,\"betTypeCode\":146,\"playerOdds\":1980,\"count\":1,\"multiple\":1,\"channelId\":142,\"subState\":0,\"prizeDetail\":[{\"count\":1,\"number\":\"总和大\",\"awardName\":\"五星总和大小单双\"}],\"ip\":\"220.133.218.203\",\"updateTime\":\"2023-12-22T07:09:59Z\",\"createTime\":\"2023-12-22T07:08:35Z\",\"note\":\"\",\"winningNumber\":\"2,6,7,6,6\",\"issueNumber\":\"20231222246\",\"betTypeName\":\"五星总和大小单双\",\"channelName\":\"VR幸运1.5分彩\",\"channelCode\":\"VRSSCG\",\"position\":\"万,千,百,十,个\",\"number\":\"总和大\",\"odds\":\"1.97\",\"playerName\":\"4ucmqplxb029\",\"serialNumber\":\"11222150835182930660\",\"merchantCode\":\"PEGA\"}"
                )};
        _ = fakeGameApiClient
            .MerchantQueryBetAsync(
            apiUrl: Arg.Is(query.ApiUrl),
            version: Arg.Is(query.Version),
            merchantId: Arg.Is(query.MerchantId),
            deskey: Arg.Is(query.Deskey),
            isUpdateTime: Arg.Is(true),
            state: Arg.Is(MerchantQueryBetStates.All),
            startTime: Arg.Is(query.StartTime),
            endTime: Arg.Is(query.EndTime),
            recordPage: Arg.Is(query.PageNumber - 1),
            recordCountPerPage: Arg.Is(query.PageSize),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(responses.ToAsyncEnumerable());

        var service = new GameWagerSettledQueryService(
            fakeGameApiClient);

        var actual = await service.QueryAsync(query)
            .ToArrayAsync()
            ?? Enumerable.Empty<GameWagerResult>();

        Assert.True(actual.Any());
        Assert.Equal("20231222246", actual.Single().ExternalRoundId);
        Assert.Equal("11222150835182930660", actual.Single().ExternalWagerId);
        Assert.Equal("142", actual.Single().ExternalGameId);
        Assert.Equal("4ucmqplxb029", actual.Single().PlayerCode);
        Assert.True(actual.Single().IsClosed);
        Assert.False(actual.Single().IsCanceled);
        Assert.False(actual.Single().IsResettle);
        Assert.Equal(DateTimeOffset.Parse("2023-12-22T07:08:35Z"), actual.Single().ExternalCreatedOn);
        Assert.Equal(DateTimeOffset.Parse("2023-12-22T07:09:59Z"), actual.Single().ExternalClosedOn);
        Assert.Equal(DateTimeOffset.UnixEpoch, actual.Single().ExternalResettleOn);
        Assert.Equal(1.0m, actual.Single().Bet);
        Assert.Equal(1.97m, actual.Single().Settle);
        Assert.Equal(0.97m, actual.Single().WinLoss);
        Assert.Equal(responses.Single().AdditionalData, actual.Single().AdditionalData);
    }

    [Fact]
    public async void Winning_is_changed_to_Not_Winning()
    {
        var query = new GameWagerSettledQuery(
            ApiUrl: new Uri("https://fe.vrbetdemo.com/"),
            Version: "1.0",
            MerchantId: "PEGA",
            Deskey: "8VF408JBV2B406488F2VH2X684T6R6DV",
            StartTime: DateTimeOffset.Parse("2023-12-22T07:08:00Z"),
            EndTime: DateTimeOffset.Parse("2023-12-22T07:13:00Z"),
            PageNumber: 1,
            PageSize: 1000);

        var fakeGameApiClient = Substitute.For<IGameApiClient>();

        var responses = new MerchantQueryResponse[] {
            new(
                IssueNumber:"20231222246",
                SerialNumber:"11222150835182930660",
                ChannelId:"142",
                PlayerName:"4ucmqplxb029",
                State:2,
                SubState:8,
                Cost:1.0m,
                PlayerPrize:1.97m,
                LossPrize:1.97m,
                CreateTime:DateTimeOffset.Parse("2023-12-22T07:08:35Z"),
                UpdateTime:DateTimeOffset.Parse("2023-12-22T08:27:34Z"),
                AdditionalData:"{\"cost\":1.0,\"unit\":1.0,\"lossPrize\":1.97,\"playerPrize\":1.97,\"merchantPrize\":1.97,\"state\":2,\"betTypeCode\":146,\"playerOdds\":1980,\"count\":1,\"multiple\":1,\"channelId\":142,\"subState\":8,\"prizeDetail\":[],\"ip\":\"220.133.218.203\",\"updateTime\":\"2023-12-22T08:27:34Z\",\"createTime\":\"2023-12-22T07:08:35Z\",\"note\":\"\",\"winningNumber\":\"2,6,8,0,6\",\"issueNumber\":\"20231222246\",\"betTypeName\":\"五星总和大小单双\",\"channelName\":\"VR幸运1.5分彩\",\"channelCode\":\"VRSSCG\",\"position\":\"万,千,百,十,个\",\"number\":\"总和大\",\"odds\":\"1.97\",\"playerName\":\"4ucmqplxb029\",\"serialNumber\":\"11222150835182930660\",\"merchantCode\":\"PEGA\"}"
                )};
        _ = fakeGameApiClient
            .MerchantQueryBetAsync(
            apiUrl: Arg.Is(query.ApiUrl),
            version: Arg.Is(query.Version),
            merchantId: Arg.Is(query.MerchantId),
            deskey: Arg.Is(query.Deskey),
            isUpdateTime: Arg.Is(true),
            state: Arg.Is(MerchantQueryBetStates.All),
            startTime: Arg.Is(query.StartTime),
            endTime: Arg.Is(query.EndTime),
            recordPage: Arg.Is(query.PageNumber - 1),
            recordCountPerPage: Arg.Is(query.PageSize),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(responses.ToAsyncEnumerable());

        var service = new GameWagerSettledQueryService(
            fakeGameApiClient);

        var actual = await service.QueryAsync(query)
            .ToArrayAsync()
            ?? Enumerable.Empty<GameWagerResult>();

        Assert.True(actual.Any());
        Assert.Equal("20231222246", actual.Single().ExternalRoundId);
        Assert.Equal("11222150835182930660", actual.Single().ExternalWagerId);
        Assert.Equal("142", actual.Single().ExternalGameId);
        Assert.Equal("4ucmqplxb029", actual.Single().PlayerCode);
        Assert.True(actual.Single().IsClosed);
        Assert.False(actual.Single().IsCanceled);
        Assert.True(actual.Single().IsResettle);
        Assert.Equal(DateTimeOffset.Parse("2023-12-22T07:08:35Z"), actual.Single().ExternalCreatedOn);
        Assert.Equal(DateTimeOffset.Parse("2023-12-22T08:27:34Z"), actual.Single().ExternalClosedOn);
        Assert.Equal(DateTimeOffset.Parse("2023-12-22T08:27:34Z"), actual.Single().ExternalResettleOn);
        Assert.Equal(1.0m, actual.Single().Bet);
        Assert.Equal(0.0m, actual.Single().Settle);
        Assert.Equal(-1.0m, actual.Single().WinLoss);
        Assert.Equal(responses.Single().AdditionalData, actual.Single().AdditionalData);
    }

    [Fact]
    public async void Not_Winning()
    {
        var query = new GameWagerSettledQuery(
            ApiUrl: new Uri("https://fe.vrbetdemo.com/"),
            Version: "1.0",
            MerchantId: "PEGA",
            Deskey: "8VF408JBV2B406488F2VH2X684T6R6DV",
            StartTime: DateTimeOffset.Parse("2023-12-22T08:35:00Z"),
            EndTime: DateTimeOffset.Parse("2023-12-22T08:40:00Z"),
            PageNumber: 1,
            PageSize: 1000);

        var fakeGameApiClient = Substitute.For<IGameApiClient>();

        var responses = new MerchantQueryResponse[] {
            new(
                IssueNumber:"20231222305",
                SerialNumber:"11222163639026769723",
                ChannelId:"142",
                PlayerName:"4ucmqplxb029",
                State:2,
                SubState:0,
                Cost:1.0m,
                PlayerPrize:0.0m,
                LossPrize:0.0m,
                CreateTime:DateTimeOffset.Parse("2023-12-22T08:36:39Z"),
                UpdateTime:DateTimeOffset.Parse("2023-12-22T08:38:27Z"),
                AdditionalData:"{\"cost\":1.0,\"unit\":1.0,\"lossPrize\":0.0,\"playerPrize\":0.0,\"merchantPrize\":0.0,\"state\":2,\"betTypeCode\":146,\"playerOdds\":1980,\"count\":1,\"multiple\":1,\"channelId\":142,\"subState\":0,\"prizeDetail\":[],\"ip\":\"220.133.218.203\",\"updateTime\":\"2023-12-22T08:38:27Z\",\"createTime\":\"2023-12-22T08:36:39Z\",\"note\":\"\",\"winningNumber\":\"5,3,8,4,9\",\"issueNumber\":\"20231222305\",\"betTypeName\":\"五星总和大小单双\",\"channelName\":\"VR幸运1.5分彩\",\"channelCode\":\"VRSSCG\",\"position\":\"万,千,百,十,个\",\"number\":\"总和小\",\"odds\":\"1.97\",\"playerName\":\"4ucmqplxb029\",\"serialNumber\":\"11222163639026769723\",\"merchantCode\":\"PEGA\"}"
                )};
        _ = fakeGameApiClient
            .MerchantQueryBetAsync(
            apiUrl: Arg.Is(query.ApiUrl),
            version: Arg.Is(query.Version),
            merchantId: Arg.Is(query.MerchantId),
            deskey: Arg.Is(query.Deskey),
            isUpdateTime: Arg.Is(true),
            state: Arg.Is(MerchantQueryBetStates.All),
            startTime: Arg.Is(query.StartTime),
            endTime: Arg.Is(query.EndTime),
            recordPage: Arg.Is(query.PageNumber - 1),
            recordCountPerPage: Arg.Is(query.PageSize),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(responses.ToAsyncEnumerable());

        var service = new GameWagerSettledQueryService(
            fakeGameApiClient);

        var actual = await service.QueryAsync(query)
            .ToArrayAsync()
            ?? Enumerable.Empty<GameWagerResult>();

        Assert.True(actual.Any());
        Assert.Equal("20231222305", actual.Single().ExternalRoundId);
        Assert.Equal("11222163639026769723", actual.Single().ExternalWagerId);
        Assert.Equal("142", actual.Single().ExternalGameId);
        Assert.Equal("4ucmqplxb029", actual.Single().PlayerCode);
        Assert.True(actual.Single().IsClosed);
        Assert.False(actual.Single().IsCanceled);
        Assert.False(actual.Single().IsResettle);
        Assert.Equal(DateTimeOffset.Parse("2023-12-22T08:36:39Z"), actual.Single().ExternalCreatedOn);
        Assert.Equal(DateTimeOffset.Parse("2023-12-22T08:38:27Z"), actual.Single().ExternalClosedOn);
        Assert.Equal(DateTimeOffset.UnixEpoch, actual.Single().ExternalResettleOn);
        Assert.Equal(1.0m, actual.Single().Bet);
        Assert.Equal(0.0m, actual.Single().Settle);
        Assert.Equal(-1.0m, actual.Single().WinLoss);
        Assert.Equal(responses.Single().AdditionalData, actual.Single().AdditionalData);
    }

    [Fact]
    public async void Not_winning_prize_changed_to_winning_prize()
    {
        var query = new GameWagerSettledQuery(
            ApiUrl: new Uri("https://fe.vrbetdemo.com/"),
            Version: "1.0",
            MerchantId: "PEGA",
            Deskey: "8VF408JBV2B406488F2VH2X684T6R6DV",
            StartTime: DateTimeOffset.Parse("2023-12-22T08:35:00Z"),
            EndTime: DateTimeOffset.Parse("2023-12-22T08:40:00Z"),
            PageNumber: 1,
            PageSize: 1000);

        var fakeGameApiClient = Substitute.For<IGameApiClient>();

        var responses = new MerchantQueryResponse[] {
            new(
                IssueNumber:"20231222305",
                SerialNumber:"11222163639026769723",
                ChannelId:"142",
                PlayerName:"4ucmqplxb029",
                State:3,
                SubState:8,
                Cost:1.0m,
                PlayerPrize:1.97m,
                LossPrize:0.0m,
                CreateTime:DateTimeOffset.Parse("2023-12-22T08:36:39Z"),
                UpdateTime:DateTimeOffset.Parse("2023-12-22T09:03:17Z"),
                AdditionalData:"{\"cost\":1.0,\"unit\":1.0,\"lossPrize\":0.0,\"playerPrize\":1.97,\"merchantPrize\":1.97,\"state\":3,\"betTypeCode\":146,\"playerOdds\":1980,\"count\":1,\"multiple\":1,\"channelId\":142,\"subState\":8,\"prizeDetail\":[{\"count\":1,\"number\":\"总和小\",\"awardName\":\"五星总和大小单双\"}],\"ip\":\"220.133.218.203\",\"updateTime\":\"2023-12-22T09:03:17Z\",\"createTime\":\"2023-12-22T08:36:39Z\",\"note\":\"\",\"winningNumber\":\"3,1,3,4,2\",\"issueNumber\":\"20231222305\",\"betTypeName\":\"五星总和大小单双\",\"channelName\":\"VR幸运1.5分彩\",\"channelCode\":\"VRSSCG\",\"position\":\"万,千,百,十,个\",\"number\":\"总和小\",\"odds\":\"1.97\",\"playerName\":\"4ucmqplxb029\",\"serialNumber\":\"11222163639026769723\",\"merchantCode\":\"PEGA\"}"
                )};
        _ = fakeGameApiClient
            .MerchantQueryBetAsync(
            apiUrl: Arg.Is(query.ApiUrl),
            version: Arg.Is(query.Version),
            merchantId: Arg.Is(query.MerchantId),
            deskey: Arg.Is(query.Deskey),
            isUpdateTime: Arg.Is(true),
            state: Arg.Is(MerchantQueryBetStates.All),
            startTime: Arg.Is(query.StartTime),
            endTime: Arg.Is(query.EndTime),
            recordPage: Arg.Is(query.PageNumber - 1),
            recordCountPerPage: Arg.Is(query.PageSize),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(responses.ToAsyncEnumerable());

        var service = new GameWagerSettledQueryService(
            fakeGameApiClient);

        var actual = await service.QueryAsync(query)
            .ToArrayAsync()
            ?? Enumerable.Empty<GameWagerResult>();

        Assert.True(actual.Any());
        Assert.Equal("20231222305", actual.Single().ExternalRoundId);
        Assert.Equal("11222163639026769723", actual.Single().ExternalWagerId);
        Assert.Equal("142", actual.Single().ExternalGameId);
        Assert.Equal("4ucmqplxb029", actual.Single().PlayerCode);
        Assert.True(actual.Single().IsClosed);
        Assert.False(actual.Single().IsCanceled);
        Assert.True(actual.Single().IsResettle);
        Assert.Equal(DateTimeOffset.Parse("2023-12-22T08:36:39Z"), actual.Single().ExternalCreatedOn);
        Assert.Equal(DateTimeOffset.Parse("2023-12-22T09:03:17Z"), actual.Single().ExternalClosedOn);
        Assert.Equal(DateTimeOffset.Parse("2023-12-22T09:03:17Z"), actual.Single().ExternalResettleOn);
        Assert.Equal(1.0m, actual.Single().Bet);
        Assert.Equal(1.97m, actual.Single().Settle);
        Assert.Equal(0.97m, actual.Single().WinLoss);
        Assert.Equal(responses.Single().AdditionalData, actual.Single().AdditionalData);
    }

    [Fact]
    public async void Cancel_order()
    {
        var query = new GameWagerSettledQuery(
            ApiUrl: new Uri("https://fe.vrbetdemo.com/"),
            Version: "1.0",
            MerchantId: "PEGA",
            Deskey: "8VF408JBV2B406488F2VH2X684T6R6DV",
            StartTime: DateTimeOffset.Parse("2023-12-22T09:29:00Z"),
            EndTime: DateTimeOffset.Parse("2023-12-22T09:30:00Z"),
            PageNumber: 1,
            PageSize: 1000);

        var fakeGameApiClient = Substitute.For<IGameApiClient>();

        var responses = new MerchantQueryResponse[] {
            new(
                IssueNumber:"20231222340",
                SerialNumber:"11222172915419426252",
                ChannelId:"142",
                PlayerName:"4ucmqplxb029",
                State:1,
                SubState:1,
                Cost:1.0m,
                PlayerPrize:0.0m,
                LossPrize:0.0m,
                CreateTime:DateTimeOffset.Parse("2023-12-22T09:29:15Z"),
                UpdateTime:DateTimeOffset.Parse("2023-12-22T09:29:25Z"),
                AdditionalData:"{\"cost\":1.0,\"unit\":1.0,\"lossPrize\":0.0,\"playerPrize\":0.0,\"merchantPrize\":0.0,\"state\":1,\"betTypeCode\":146,\"playerOdds\":1980,\"count\":1,\"multiple\":1,\"channelId\":142,\"subState\":1,\"prizeDetail\":[],\"ip\":\"220.133.218.203\",\"updateTime\":\"2023-12-22T09:29:25Z\",\"createTime\":\"2023-12-22T09:29:15Z\",\"note\":\"\",\"winningNumber\":null,\"issueNumber\":\"20231222340\",\"betTypeName\":\"五星总和大小单双\",\"channelName\":\"VR幸运1.5分彩\",\"channelCode\":\"VRSSCG\",\"position\":\"万,千,百,十,个\",\"number\":\"总和大\",\"odds\":\"1.97\",\"playerName\":\"4ucmqplxb029\",\"serialNumber\":\"11222172915419426252\",\"merchantCode\":\"PEGA\"}"
                )};
        _ = fakeGameApiClient
            .MerchantQueryBetAsync(
            apiUrl: Arg.Is(query.ApiUrl),
            version: Arg.Is(query.Version),
            merchantId: Arg.Is(query.MerchantId),
            deskey: Arg.Is(query.Deskey),
            isUpdateTime: Arg.Is(true),
            state: Arg.Is(MerchantQueryBetStates.All),
            startTime: Arg.Is(query.StartTime),
            endTime: Arg.Is(query.EndTime),
            recordPage: Arg.Is(query.PageNumber - 1),
            recordCountPerPage: Arg.Is(query.PageSize),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(responses.ToAsyncEnumerable());

        var service = new GameWagerSettledQueryService(
            fakeGameApiClient);

        var actual = await service.QueryAsync(query)
            .ToArrayAsync()
            ?? Enumerable.Empty<GameWagerResult>();

        Assert.True(actual.Any());
        Assert.Equal("20231222340", actual.Single().ExternalRoundId);
        Assert.Equal("11222172915419426252", actual.Single().ExternalWagerId);
        Assert.Equal("142", actual.Single().ExternalGameId);
        Assert.Equal("4ucmqplxb029", actual.Single().PlayerCode);
        Assert.True(actual.Single().IsClosed);
        Assert.True(actual.Single().IsCanceled);
        Assert.False(actual.Single().IsResettle);
        Assert.Equal(DateTimeOffset.Parse("2023-12-22T09:29:15Z"), actual.Single().ExternalCreatedOn);
        Assert.Equal(DateTimeOffset.Parse("2023-12-22T09:29:25Z"), actual.Single().ExternalClosedOn);
        Assert.Equal(DateTimeOffset.UnixEpoch, actual.Single().ExternalResettleOn);
        Assert.Equal(1.0m, actual.Single().Bet);
        Assert.Equal(0.0m, actual.Single().Settle);
        Assert.Equal(0.0m, actual.Single().WinLoss);
        Assert.Equal(responses.Single().AdditionalData, actual.Single().AdditionalData);
    }

    [Fact]
    public async void Winning_the_lottery_becomes_winning_again__The_amount_is_again_higher_than_the_original_winning_amount()
    {
        //中獎 變成 再次中獎(再次中獎的金額高出原本中獎的金額)
        var query = new GameWagerSettledQuery(
            ApiUrl: new Uri("https://fe.vrbetdemo.com/"),
            Version: "1.0",
            MerchantId: "PEGA",
            Deskey: "8VF408JBV2B406488F2VH2X684T6R6DV",
            StartTime: DateTimeOffset.Parse("2023-12-25T02:45:00Z"),
            EndTime: DateTimeOffset.Parse("2023-12-25T02:55:00Z"),
            PageNumber: 1,
            PageSize: 1000);

        var fakeGameApiClient = Substitute.For<IGameApiClient>();

        var responses = new MerchantQueryResponse[] {
            new(
                IssueNumber:"20231225067",
                SerialNumber:"11225104004166432119",
                ChannelId:"142",
                PlayerName:"4ucmqplxb029",
                State:3,
                SubState:8,
                Cost:50.0m,
                PlayerPrize:98.5m,
                LossPrize:0.0m,
                CreateTime:DateTimeOffset.Parse("2023-12-25T02:40:04Z"),
                UpdateTime:DateTimeOffset.Parse("2023-12-25T02:48:31Z"),
                AdditionalData:"{\"cost\":50.0,\"unit\":2.0,\"lossPrize\":0.0,\"playerPrize\":98.5,\"merchantPrize\":98.5,\"state\":3,\"betTypeCode\":21,\"playerOdds\":1980,\"count\":25,\"multiple\":1,\"channelId\":142,\"subState\":8,\"prizeDetail\":[{\"count\":5,\"number\":\"5,5,5,5,5\",\"awardName\":\"定位胆\"}],\"ip\":\"220.133.218.203\",\"updateTime\":\"2023-12-25T02:48:31Z\",\"createTime\":\"2023-12-25T02:40:04Z\",\"note\":\"\",\"winningNumber\":\"5,5,5,5,5\",\"issueNumber\":\"20231225067\",\"betTypeName\":\"定位胆\",\"channelName\":\"VR幸运1.5分彩\",\"channelCode\":\"VRSSCG\",\"position\":\"万,千,百,十,个\",\"number\":\"56789,56789,56789,56789,56789\",\"odds\":\"19.7\",\"playerName\":\"4ucmqplxb029\",\"serialNumber\":\"11225104004166432119\",\"merchantCode\":\"PEGA\"}"
                )};
        _ = fakeGameApiClient
            .MerchantQueryBetAsync(
            apiUrl: Arg.Is(query.ApiUrl),
            version: Arg.Is(query.Version),
            merchantId: Arg.Is(query.MerchantId),
            deskey: Arg.Is(query.Deskey),
            isUpdateTime: Arg.Is(true),
            state: Arg.Is(MerchantQueryBetStates.All),
            startTime: Arg.Is(query.StartTime),
            endTime: Arg.Is(query.EndTime),
            recordPage: Arg.Is(query.PageNumber - 1),
            recordCountPerPage: Arg.Is(query.PageSize),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(responses.ToAsyncEnumerable());

        var service = new GameWagerSettledQueryService(
            fakeGameApiClient);

        var actual = await service.QueryAsync(query)
            .ToArrayAsync()
            ?? Enumerable.Empty<GameWagerResult>();

        Assert.True(actual.Any());
        Assert.Equal("20231225067", actual.Single().ExternalRoundId);
        Assert.Equal("11225104004166432119", actual.Single().ExternalWagerId);
        Assert.Equal("142", actual.Single().ExternalGameId);
        Assert.Equal("4ucmqplxb029", actual.Single().PlayerCode);
        Assert.True(actual.Single().IsClosed);
        Assert.False(actual.Single().IsCanceled);
        Assert.True(actual.Single().IsResettle);
        Assert.Equal(DateTimeOffset.Parse("2023-12-25T02:40:04Z"), actual.Single().ExternalCreatedOn);
        Assert.Equal(DateTimeOffset.Parse("2023-12-25T02:48:31Z"), actual.Single().ExternalClosedOn);
        Assert.Equal(DateTimeOffset.Parse("2023-12-25T02:48:31Z"), actual.Single().ExternalResettleOn);
        Assert.Equal(50.0m, actual.Single().Bet);
        Assert.Equal(98.5m, actual.Single().Settle);
        Assert.Equal(48.5m, actual.Single().WinLoss);
        Assert.Equal(responses.Single().AdditionalData, actual.Single().AdditionalData);
    }

    [Fact]
    public async void Winning_the_lottery_becomes_winning_again__The_amount_won_again_is_lower_than_the_original_winning_amount()
    {
        //中獎 變成 再次中獎(再次中獎的金額低於原本中獎的金額)
        var query = new GameWagerSettledQuery(
            ApiUrl: new Uri("https://fe.vrbetdemo.com/"),
            Version: "1.0",
            MerchantId: "PEGA",
            Deskey: "8VF408JBV2B406488F2VH2X684T6R6DV",
            StartTime: DateTimeOffset.Parse("2023-12-25T03:30:00Z"),
            EndTime: DateTimeOffset.Parse("2023-12-25T03:40:00Z"),
            PageNumber: 1,
            PageSize: 1000);

        var fakeGameApiClient = Substitute.For<IGameApiClient>();

        var responses = new MerchantQueryResponse[] {
            new(
                IssueNumber:"20231225097",
                SerialNumber:"11225112422212738043",
                ChannelId:"142",
                PlayerName:"4ucmqplxb029",
                State:3,
                SubState:8,
                Cost:20.0m,
                PlayerPrize:39.4m,
                LossPrize:19.7m,
                CreateTime:DateTimeOffset.Parse("2023-12-25T03:24:22Z"),
                UpdateTime:DateTimeOffset.Parse("2023-12-25T03:33:35Z"),
                AdditionalData:"{\"cost\":20.0,\"unit\":2.0,\"lossPrize\":19.7,\"playerPrize\":39.4,\"merchantPrize\":39.4,\"state\":3,\"betTypeCode\":21,\"playerOdds\":1980,\"count\":10,\"multiple\":1,\"channelId\":142,\"subState\":8,\"prizeDetail\":[{\"count\":1,\"number\":\",,5,,\",\"awardName\":\"定位胆\"}],\"ip\":\"220.133.218.203\",\"updateTime\":\"2023-12-25T03:33:35Z\",\"createTime\":\"2023-12-25T03:24:22Z\",\"note\":\"\",\"winningNumber\":\"7,8,5,5,9\",\"issueNumber\":\"20231225097\",\"betTypeName\":\"定位胆\",\"channelName\":\"VR幸运1.5分彩\",\"channelCode\":\"VRSSCG\",\"position\":\"千,百\",\"number\":\",13579,13579,,\",\"odds\":\"19.7\",\"playerName\":\"4ucmqplxb029\",\"serialNumber\":\"11225112422212738043\",\"merchantCode\":\"PEGA\"}"
                )};
        _ = fakeGameApiClient
            .MerchantQueryBetAsync(
            apiUrl: Arg.Is(query.ApiUrl),
            version: Arg.Is(query.Version),
            merchantId: Arg.Is(query.MerchantId),
            deskey: Arg.Is(query.Deskey),
            isUpdateTime: Arg.Is(true),
            state: Arg.Is(MerchantQueryBetStates.All),
            startTime: Arg.Is(query.StartTime),
            endTime: Arg.Is(query.EndTime),
            recordPage: Arg.Is(query.PageNumber - 1),
            recordCountPerPage: Arg.Is(query.PageSize),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(responses.ToAsyncEnumerable());

        var service = new GameWagerSettledQueryService(
            fakeGameApiClient);

        var actual = await service.QueryAsync(query)
            .ToArrayAsync()
            ?? Enumerable.Empty<GameWagerResult>();

        Assert.True(actual.Any());
        Assert.Equal("20231225097", actual.Single().ExternalRoundId);
        Assert.Equal("11225112422212738043", actual.Single().ExternalWagerId);
        Assert.Equal("142", actual.Single().ExternalGameId);
        Assert.Equal("4ucmqplxb029", actual.Single().PlayerCode);
        Assert.True(actual.Single().IsClosed);
        Assert.False(actual.Single().IsCanceled);
        Assert.True(actual.Single().IsResettle);
        Assert.Equal(DateTimeOffset.Parse("2023-12-25T03:24:22Z"), actual.Single().ExternalCreatedOn);
        Assert.Equal(DateTimeOffset.Parse("2023-12-25T03:33:35Z"), actual.Single().ExternalClosedOn);
        Assert.Equal(DateTimeOffset.Parse("2023-12-25T03:33:35Z"), actual.Single().ExternalResettleOn);
        Assert.Equal(20.0m, actual.Single().Bet);
        Assert.Equal(19.7m, actual.Single().Settle);
        Assert.Equal(-0.3m, actual.Single().WinLoss);
        Assert.Equal(responses.Single().AdditionalData, actual.Single().AdditionalData);
    }
}
