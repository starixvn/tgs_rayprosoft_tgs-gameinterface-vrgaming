using System.Net.Mime;
using System.Net;
using System.Text;
using System.Text.Json.Nodes;
using NSubstitute;
using RichardSzalay.MockHttp;
using Microsoft.Extensions.Logging.Abstractions;
using System.Numerics;

namespace Tgs.GameInterface.VRGaming.Core.UnitTest;

public class DepositUnitTest
{
    [Fact]
    public async void Normal_Process()
    {
        Uri apiUrl = new Uri("https://fe.vrbetdemo.com/");
        var version = "1.0";
        var merchantid = "PEGA";
        var deskey = "8VF408JBV2B406488F2VH2X684T6R6DV";
        var playerName = "4ucmqplxb029";
        var serialNumber = "018c8f8f201f0000000107e7db655fdb";
        var amount = 1.0m;

        var fakeCryptography = Substitute.For<IAesCryptographyUtensil>();
        var fakeSystemClock = Substitute.For<ISystemClock>();

        var currentTime = new DateTimeOffset(2023, 12, 22, 3, 27, 52, TimeSpan.Zero);
        _ = fakeSystemClock.CurrentTime.Returns(currentTime);

        var parameters = new JsonObject
        {
            ["serialNumber"] = "018c8f8f201f0000000107e7db655fdb",
            ["playerName"] = playerName,
            ["type"] = 0,
            ["amount"] = amount,
            ["createTime"] = currentTime.UtcDateTime.ToString("yyyy-MM-ddTHH:mm:ssZ")
        };
        var dataEncrypt = "3wNqIsKZT/kMmnFXN91o0AoTKznyJWigCBlnWLCywlSLYK/\\u002B/DC4FtEnllpzyT68hk2WTEqme\\u002Bzz7r02xh4AQf\\u002BLjaKbjtjPrBYZc3BZFLRbRx3/YDieeq7fdPE107Ojy5ibJtdImzxXctwOnpzXJjyiVIt1vFWFkhUul9xkUN8Ip2uuF1YTXwSx/v1njxeD";
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
        var uriBuilder = new UriBuilder(new Uri(apiUrl, "/UserWallet/Transaction"));
        var responseContent = "3wNqIsKZT/kMmnFXN91o0AoTKznyJWigCBlnWLCywlSLYK/+/DC4FtEnllpzyT68hk2WTEqme+zz7r02xh4AQf+LjaKbjtjPrBYZc3BZFLRbRx3/YDieeq7fdPE107Oj8qMqHGbLAYJEtXDXQmLx3BId7VN7JrXB53vP6tbH+MRy1WjpAdgTqxjllogfffuCLK4T77tJDVSKjMCPnzEOjImPT4faz8Cj0iW2/LHJQu4=";
        var handler = new MockHttpMessageHandler();
        _ = handler.Expect(HttpMethod.Post, uriBuilder.ToString())
            .WithContent(await requestContent.ReadAsStringAsync())
            .Respond(HttpStatusCode.OK, new StringContent(responseContent, Encoding.UTF8, MediaTypeNames.Text.Html));

        var responseDecrypt = "{\"serialNumber\":\"018c8f8f201f0000000107e7db655fdb\",\"playerName\":\"4ucmqplxb029\",\"type\":0,\"amount\":1.0,\"createTime\":\"2023-12-22T03:27:52Z\",\"state\":0,\"balance\":1.000}";
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

        var actual = await client.DepositAsync(
            apiUrl: apiUrl,
            version: version,
            merchantid: merchantid,
            deskey: deskey,
            playerName: playerName,
            serialNumber: serialNumber,
            amount: amount);

        Assert.Equal("018c8f8f201f0000000107e7db655fdb", actual.SerialNumber);
        Assert.Equal(responseDecrypt, actual.Resp);
    }
}
