using System.Net.Mime;
using System.Net;
using System.Text;
using System.Text.Json.Nodes;
using NSubstitute;
using RichardSzalay.MockHttp;
using Microsoft.Extensions.Logging.Abstractions;
using System.Numerics;

namespace Tgs.GameInterface.VRGaming.Core.UnitTest;

public class BalanceUnitTest
{
    [Fact]
    public async void Normal_Process()
    {
        Uri apiUrl = new Uri("https://fe.vrbetdemo.com/");
        var version = "1.0";
        var merchantid = "PEGA";
        var deskey = "8VF408JBV2B406488F2VH2X684T6R6DV";
        var playerName = "4ucmqplxb029";

        var fakeCryptography = Substitute.For<IAesCryptographyUtensil>();
        var fakeSystemClock = Substitute.For<ISystemClock>();

        var parameters = new JsonObject
        {
            ["playerName"] = playerName
        };
        var dataEncrypt = "WK7HCiRT7YbaumsZuZp7dFee8OB106fIb6BPo8mu2a4=";
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
        var uriBuilder = new UriBuilder(new Uri(apiUrl, "/UserWallet/Balance"));
        var responseContent = "WK7HCiRT7YbaumsZuZp7dFJ+5nwAQaP7ha9961v/xSyclkPFanSzRwpHFvWagQ++D39ES4Ad64kEC0XQOsXEcA==";
        var handler = new MockHttpMessageHandler();
        _ = handler.Expect(HttpMethod.Post, uriBuilder.ToString())
            .WithContent(await requestContent.ReadAsStringAsync())
            .Respond(HttpStatusCode.OK, new StringContent(responseContent, Encoding.UTF8, MediaTypeNames.Text.Html));

        var responseDecrypt = "{\"playerName\":\"4ucmqplxb029\",\"balance\":0.0,\"games\":[]}";
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

        var actual = await client.BalanceAsync(
            apiUrl: apiUrl,
            version: version,
            merchantid: merchantid,
            deskey: deskey,
            playerName: playerName);

        Assert.True(actual.IsPlayerExist);
        Assert.Equal(0.0m, actual.Balance);
    }
    [Fact]
    public async void This_Player_Does_Not_Exist()
    {
        Uri apiUrl = new Uri("https://fe.vrbetdemo.com/");
        var version = "1.0";
        var merchantid = "PEGA";
        var deskey = "8VF408JBV2B406488F2VH2X684T6R6DV";
        var playerName = "4ucmqplxb029";

        var fakeCryptography = Substitute.For<IAesCryptographyUtensil>();
        var fakeSystemClock = Substitute.For<ISystemClock>();

        var parameters = new JsonObject
        {
            ["playerName"] = playerName
        };

        var dataEncrypt = "WK7HCiRT7YbaumsZuZp7dFee8OB106fIb6BPo8mu2a4=";
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
        var uriBuilder = new UriBuilder(new Uri(apiUrl, "/UserWallet/Balance"));
        var responseContent = "WK7HCiRT7YbaumsZuZp7dFJ+5nwAQaP7ha9961v/xSygk9OJkB8dSFSb2zIZ3nz++E67qaQ4j4njI5F9sTA/dA==";
        var handler = new MockHttpMessageHandler();
        _ = handler.Expect(HttpMethod.Post, uriBuilder.ToString())
            .WithContent(await requestContent.ReadAsStringAsync())
            .Respond(HttpStatusCode.OK, new StringContent(responseContent, Encoding.UTF8, MediaTypeNames.Text.Html));

        var responseDecrypt = "{\"playerName\":\"4ucmqplxb029\",\"balance\":-1.0,\"games\":[]}";
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

        var actual = await client.BalanceAsync(
            apiUrl: apiUrl,
            version: version,
            merchantid: merchantid,
            deskey: deskey,
            playerName: playerName);

        Assert.False(actual.IsPlayerExist);
        Assert.Equal(-1.0m, actual.Balance);

    }
}
