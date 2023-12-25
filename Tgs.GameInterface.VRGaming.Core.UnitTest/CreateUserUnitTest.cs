using System.Net;

using System.Net.Mime;
using System.Text;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging.Abstractions;

using NSubstitute;
using RichardSzalay.MockHttp;

namespace Tgs.GameInterface.VRGaming.Core.UnitTest;

public class CreateUserUnitTest
{
    [Fact]
    public async void Normal_Process()
    {
        Uri apiUrl = new Uri("https://fe.vrbetdemo.com/");
        var version = "1.0";
        var merchantId = "PEGA";
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
            ["id"] = merchantId,
            ["data"] = dataEncrypt,
        };
        var requestContent = new StringContent(data.ToJsonString(), Encoding.UTF8, MediaTypeNames.Application.Json);
        var uriBuilder = new UriBuilder(new Uri(apiUrl, "/Account/CreateUser"));
        var responseContent = "0pbZCPdihj4cMF7Gdjw5sg==";
        var handler = new MockHttpMessageHandler();
        _ = handler.Expect(HttpMethod.Post, uriBuilder.ToString())
            .WithContent(await requestContent.ReadAsStringAsync())
            .Respond(HttpStatusCode.OK, new StringContent(responseContent, Encoding.UTF8, MediaTypeNames.Text.Html));

        var responseDecrypt = "{\"errorCode\":0}";
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

        await client.CreateUserAsync(
            apiUrl: apiUrl,
            version: version,
            merchantId: merchantId,
            deskey: deskey,
            playerName: playerName);
    }

    [Fact]
    public async void This_Player_Already_Exists()
    {
        Uri apiUrl = new Uri("https://fe.vrbetdemo.com/");
        var version = "1.0";
        var merchantId = "PEGA";
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
            ["id"] = merchantId,
            ["data"] = dataEncrypt,
        };
        var requestContent = new StringContent(data.ToJsonString(), Encoding.UTF8, MediaTypeNames.Application.Json);
        var uriBuilder = new UriBuilder(new Uri(apiUrl, "/Account/CreateUser"));
        var responseContent = "H86BLwu1nNkZIZFx2QAWvS2qGBGK/P3XBOv8cd5JauM=";
        var handler = new MockHttpMessageHandler();
        _ = handler.Expect(HttpMethod.Post, uriBuilder.ToString())
            .WithContent(await requestContent.ReadAsStringAsync())
            .Respond(HttpStatusCode.OK, new StringContent(responseContent, Encoding.UTF8, MediaTypeNames.Text.Html));

        var responseDecrypt = "{\"errorCode\":18}";
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

        await client.CreateUserAsync(
            apiUrl: apiUrl,
            version: version,
            merchantId: merchantId,
            deskey: deskey,
            playerName: playerName);
    }
}
