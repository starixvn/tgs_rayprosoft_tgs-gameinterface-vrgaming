using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using RichardSzalay.MockHttp;
using System.Text.Json.Nodes;
using Tgs.GameInterface.VRGaming.Core.Extensions;
using Tgs.GameInterface.VRGaming.Core.Models;

namespace Tgs.GameInterface.VRGaming.Core.UnitTest;

public class MerchantQueryBetUnitTest
{
    [Fact]
    public async void Normal_Process()
    {
        Uri apiUrl = new Uri("https://fe.vrbetdemo.com/");
        var version = "1.0";
        var merchantid = "PEGA";
        var deskey = "8VF408JBV2B406488F2VH2X684T6R6DV";
        var startTime = new DateTimeOffset(2023, 12, 22, 07, 08, 00, TimeSpan.Zero);
        var endTime = new DateTimeOffset(2023, 12, 22, 07, 13, 00, TimeSpan.Zero);
        var recordPage = 0;
        var recordCountPerPage = 1000;

        var fakeCryptography = Substitute.For<IAesCryptographyUtensil>();
        var fakeSystemClock = Substitute.For<ISystemClock>();

        var parameters = new JsonObject
        {
            ["startTime"] = startTime.UtcDateTime.ToString("yyyy-MM-ddTHH:mm:ssZ"),
            ["endTime"] = endTime.UtcDateTime.ToString("yyyy-MM-ddTHH:mm:ssZ"),
            ["channelId"] = -1,
            ["isUpdateTime"] = true,
            ["state"] = -1,
            ["betNumberFormat"] = 0,
            ["recordPage"] = recordPage,
            ["recordCountPerPage"] = recordCountPerPage,
        };
        var dataEncrypt = "cXiY5p1vADpS7ACz/B2IuRAPLWuofQjB3pDPOgll8A9Xs87ITKQGMpD7B6cOmoPxuNprCq0DG1YiIeEVORMyXVBCiqF9DNmUBuItqhXm6jyg44hZNW1AtmzjVLG5p8sbdqy8qMCO9itF3eZn0BnszbZPEEoAWjZmZZ9TpnaI0ySTCZ8F5zxcY50tU0yTxUzmqkmKpL8iE2Tvx6/oDa551bt7adN/KstaaOIUpqQLWYYtqhgRivz91wTr/HHeSWrj";
        _ = fakeCryptography.EncryptionAsync(
            data: Arg.Is(parameters.ToJsonString()),
            secretKey: Arg.Is(deskey),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(dataEncrypt);

        var data = new JsonObject
        {
            ["version"] = version,
            ["id"] = merchantid,
            ["data"] = dataEncrypt,
        };
        var requestContent = new StringContent(data.ToJsonString(), Encoding.UTF8, MediaTypeNames.Application.Json);
        var uriBuilder = new UriBuilder(new Uri(apiUrl, "/MerchantQuery/Bet"));
        var responseContent = "AwgB7Fh2xGt4lK/WHLk6OsG6q+iYawoNkrtmA+bzMlNlQLkS2cGm1+tCYgBKgVHH6T0QAficMy36m5WLvOwieLAuoDcLuCdZEXYhXtnzCCpumkwVwIBvKQFNzjKq6BxF8Hu4kSwn6ojqaIP+BM+V2AJM0nxlNeMeVlaJV9GPYdH8Iy9eGscjV75agpCcmIZCo3i1iP3tl0hlWMrt+HcEt6/Xj0iH6zt2m21dENYf6EAN6pWPp/aykwD2FiZiv4CvABRmO81bYT704ALFWGbOuI94lPMmLIqsickVBdOOU5/tpQzASSpXk+eSH388eo91KeUuH8JHlvzJ9W9Jv8Ipztq/McGb2/AOCswrDIzB2/mqZDXplRCbEePyTqAv7YbOTD8ykOKu3c2BZHqjUW2G5wBbg0KCo1lhMxKzhHz+8O+1Msnq0ejdx92NxDbG4Mo8nTKIqAlhwtb3IozLpPfABj+VaYcatbAP0+GfC662QBt6cKEzt4CSCEwbwQFgVdFxLZvwMtmmPNsIEgOD6vJHCTr2wE4LIV1ZwiyENBViODX6I+0Cp3ehV0Y3OWFraOK1hU8CaO4hAFgRvuFXueTO2E4yX6VGyEN81eUEjwYFqjc6/3vRuG0KleGo7ChThud43ic0Yo4biHDSkFVzMs43FZt93CPNZ7NXXcjANWEMU77+6dARqoYta65a+UUX5wuPCtZA60/PWFrvadZ0wv2b2IvTD2tBRe3s4v1pSGtJTiJ4JKajuwobM6FkUjE8Uvf+9Zzi31l5lSHiN6GzO1CFOqkDAuRZQhJOzOv3r6bqheidhPFE1tCPQk/mKqdY69UWbTdR4qwbairHjy+WS5SiTWmgIQreduCwmGuxZ3pJuebpLFBjT2ckIsyBD6xwV6Sfly3IVS2NLvZ7YGk1kLrjw3xdNtZmI6wgtt8Z1DVMF7+nGeo/rFfK1evjntvb86TZFt4mwF3HJeGUmXLU29NqlyMwoXrzVrU9oKwJnKSX1ecjyUTKmlYw+8ZUtJccCX4rYhmTVIQJk5MkQFth3TtbOw==";
        var handler = new MockHttpMessageHandler();
        _ = handler.Expect(HttpMethod.Post, uriBuilder.ToString())
            .WithContent(await requestContent.ReadAsStringAsync())
            .Respond(HttpStatusCode.OK, new StringContent(responseContent, Encoding.UTF8, MediaTypeNames.Text.Html));

        var responseDecrypt = "{\"recordCountPerPage\":1000,\"recordPage\":0,\"totalRecords\":1,\"betRecords\":[{\"cost\":1.00000,\"unit\":1.00000,\"lossPrize\":0.00000,\"playerPrize\":1.97000,\"merchantPrize\":1.97000,\"state\":3,\"betTypeCode\":146,\"playerOdds\":1980,\"count\":1,\"multiple\":1,\"channelId\":142,\"subState\":0,\"prizeDetail\":[{\"count\":1,\"number\":\"总和大\",\"awardName\":\"五星总和大小单双\"}],\"ip\":\"220.133.218.203\",\"updateTime\":\"2023-12-22T07:09:59Z\",\"createTime\":\"2023-12-22T07:08:35Z\",\"note\":\"\",\"winningNumber\":\"2,6,7,6,6\",\"issueNumber\":\"20231222246\",\"betTypeName\":\"五星总和大小单双\",\"channelName\":\"VR幸运1.5分彩\",\"channelCode\":\"VRSSCG\",\"position\":\"万,千,百,十,个\",\"number\":\"总和大\",\"odds\":\"1.97\",\"playerName\":\"4ucmqplxb029\",\"serialNumber\":\"11222150835182930660\",\"merchantCode\":\"PEGA\"}]}";
        _ = fakeCryptography.DecryptAsync(
            data: Arg.Is(responseContent),
            secretKey: Arg.Is(deskey),
            isDecodeUrl: Arg.Is(false),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(responseDecrypt);

        var fakeHttpClient = handler.ToHttpClient();
        var fakeLog = NullLogger<GameApiClient>.Instance;
        var client = new GameApiClient(
            fakeLog,
            fakeSystemClock,
            fakeCryptography,
            fakeHttpClient);

        var actual = await client.MerchantQueryBetAsync(
            apiUrl: apiUrl,
            version: version,
            merchantid: merchantid,
            deskey: deskey,
            startTime: startTime,
            endTime: endTime,
            recordPage: recordPage,
            recordCountPerPage: recordCountPerPage)
            .ToArrayAsync()
            ?? Enumerable.Empty<MerchantQueryResponse>();

        Assert.True(actual.Any());
        Assert.Equal("20231222246", actual.Single().IssueNumber);
        Assert.Equal("11222150835182930660", actual.Single().SerialNumber);
        Assert.Equal("142", actual.Single().ChannelId);
        Assert.Equal("4ucmqplxb029", actual.Single().PlayerName);
        Assert.Equal(3, actual.Single().State);
        Assert.Equal(0, actual.Single().SubState);
        Assert.Equal(1.0m, actual.Single().Cost);
        Assert.Equal(1.97m, actual.Single().PlayerPrize);
        Assert.Equal(0.0m, actual.Single().LossPrize);
        Assert.Equal(DateTimeOffset.Parse("2023-12-22T07:08:35Z"), actual.Single().CreateTime);
        Assert.Equal(DateTimeOffset.Parse("2023-12-22T07:09:59Z"), actual.Single().UpdateTime);
    }

    [Fact]
    public async void No_data()
    {
        Uri apiUrl = new Uri("https://fe.vrbetdemo.com/");
        var version = "1.0";
        var merchantid = "PEGA";
        var deskey = "8VF408JBV2B406488F2VH2X684T6R6DV";
        var startTime = new DateTimeOffset(2023, 12, 22, 07, 08, 00, TimeSpan.Zero);
        var endTime = new DateTimeOffset(2023, 12, 22, 07, 13, 00, TimeSpan.Zero);
        var recordPage = 0;
        var recordCountPerPage = 1000;

        var fakeCryptography = Substitute.For<IAesCryptographyUtensil>();
        var fakeSystemClock = Substitute.For<ISystemClock>();

        var parameters = new JsonObject
        {
            ["startTime"] = startTime.UtcDateTime.ToString("yyyy-MM-ddTHH:mm:ssZ"),
            ["endTime"] = endTime.UtcDateTime.ToString("yyyy-MM-ddTHH:mm:ssZ"),
            ["channelId"] = -1,
            ["isUpdateTime"] = true,
            ["state"] = -1,
            ["betNumberFormat"] = 0,
            ["recordPage"] = recordPage,
            ["recordCountPerPage"] = recordCountPerPage,
        };
        var dataEncrypt = "cXiY5p1vADpS7ACz/B2IuRAPLWuofQjB3pDPOgll8A9Xs87ITKQGMpD7B6cOmoPxuNprCq0DG1YiIeEVORMyXVBCiqF9DNmUBuItqhXm6jyg44hZNW1AtmzjVLG5p8sbdqy8qMCO9itF3eZn0BnszbZPEEoAWjZmZZ9TpnaI0ySTCZ8F5zxcY50tU0yTxUzmqkmKpL8iE2Tvx6/oDa551bt7adN/KstaaOIUpqQLWYYtqhgRivz91wTr/HHeSWrj";
        _ = fakeCryptography.EncryptionAsync(
            data: Arg.Is(parameters.ToJsonString()),
            secretKey: Arg.Is(deskey),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(dataEncrypt);

        var data = new JsonObject
        {
            ["version"] = version,
            ["id"] = merchantid,
            ["data"] = dataEncrypt,
        };
        var requestContent = new StringContent(data.ToJsonString(), Encoding.UTF8, MediaTypeNames.Application.Json);
        var uriBuilder = new UriBuilder(new Uri(apiUrl, "/MerchantQuery/Bet"));
        var responseContent = "AwgB7Fh2xGt4lK/WHLk6OsG6q+iYawoNkrtmA+bzMlNlQLkS2cGm1+tCYgBKgVHHVy5aQwN56kr1KV9LF0a2igqshBLTh2zt/kP+WcSozoM=";
        var handler = new MockHttpMessageHandler();
        _ = handler.Expect(HttpMethod.Post, uriBuilder.ToString())
            .WithContent(await requestContent.ReadAsStringAsync())
            .Respond(HttpStatusCode.OK, new StringContent(responseContent, Encoding.UTF8, MediaTypeNames.Text.Html));

        var responseDecrypt = "{\"recordCountPerPage\":1000,\"recordPage\":0,\"totalRecords\":0,\"betRecords\":[]}";
        _ = fakeCryptography.DecryptAsync(
            data: Arg.Is(responseContent),
            secretKey: Arg.Is(deskey),
            isDecodeUrl: Arg.Is(false),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(responseDecrypt);

        var fakeHttpClient = handler.ToHttpClient();
        var fakeLog = NullLogger<GameApiClient>.Instance;
        var client = new GameApiClient(
            fakeLog,
            fakeSystemClock,
            fakeCryptography,
            fakeHttpClient);

        var actual = await client.MerchantQueryBetAsync(
            apiUrl: apiUrl,
            version: version,
            merchantid: merchantid,
            deskey: deskey,
            startTime: startTime,
            endTime: endTime,
            recordPage: recordPage,
            recordCountPerPage: recordCountPerPage)
            .ToArrayAsync()
            ?? Enumerable.Empty<MerchantQueryResponse>();

        Assert.False(actual.Any());
    }
}
