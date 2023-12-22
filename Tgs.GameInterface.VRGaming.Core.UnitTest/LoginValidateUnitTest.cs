using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using RichardSzalay.MockHttp;
using System.Text.Json.Nodes;
using System.Text.Json;

namespace Tgs.GameInterface.VRGaming.Core.UnitTest;

public class LoginValidateUnitTest
{
    [Fact]
    public async void Normal_Process()
    {
        Uri apiUrl = new Uri("https://fe.vrbetdemo.com/");
        var version = "1.0";
        var merchantid = "PEGA";
        var deskey = "8VF408JBV2B406488F2VH2X684T6R6DV";
        var playerName = "4ucmqplxb029";
        var channelId = 0;

        var fakeCryptography = Substitute.For<IAesCryptographyUtensil>();
        var fakeSystemClock = Substitute.For<ISystemClock>();

        var currentTime = new DateTimeOffset(2023, 12, 22, 06, 10, 07, TimeSpan.Zero);
        _ = fakeSystemClock.CurrentTime.Returns(currentTime);

        var parameters = new JsonObject
        {
            ["playerName"] = playerName,
            ["loginTime"] = currentTime.UtcDateTime.ToString("yyyy-MM-ddTHH:mm:ssZ"),
            ["channelId"] = channelId.ToString("D"),
            ["isCreateUser"] = "true",
            ["channelType"] = 0
        };
        var path = string.Join("&", values: JsonSerializer.Deserialize<Dictionary<string, object>>(parameters.ToJsonString())!.Where(predicate: predicate => predicate.Value is not null).Select(selector: selector => $"{selector.Key}={selector.Value}"));
        var dataEncrypt = "Vkdj0OWkkC1EkWeN2V8MHHeNHTrRTJwKsD1khnT6esg4v4/MXvuo+GMJFr8DiwxQsTfx9G3UGDhwyVVI78vGhDRDoZJk+IsHehTDI+0aM+cwCQPJUccwHFJHSvgPSYlGs4NLINGdnpB4mxx10vkQRg==";
        _ = fakeCryptography.EncryptionAsync(
            data: Arg.Is(path),
            secretKey: Arg.Is(deskey),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(dataEncrypt);

        var handler = new MockHttpMessageHandler();
        var fakeHttpClient = handler.ToHttpClient();
        var fakeLog = NullLogger<GameApiClient>.Instance;
        var client = new GameApiClient(
            fakeLog,
            fakeSystemClock,
            fakeCryptography,
            fakeHttpClient);

        var actual = await client.LoginValidateAsync(
            apiUrl: apiUrl,
            version: version,
            merchantid: merchantid,
            deskey: deskey,
            playerName: playerName,
            channelId: channelId);

        var expectedUrl = new Uri("https://fe.vrbetdemo.com:443/Account/LoginValidate?version=1.0&id=PEGA&data=Vkdj0OWkkC1EkWeN2V8MHHeNHTrRTJwKsD1khnT6esg4v4%2fMXvuo%2bGMJFr8DiwxQsTfx9G3UGDhwyVVI78vGhDRDoZJk%2bIsHehTDI%2b0aM%2bcwCQPJUccwHFJHSvgPSYlGs4NLINGdnpB4mxx10vkQRg%3d%3d");
        var actualUrl = new Uri(actual);

        Assert.Equal(expectedUrl.Scheme, actualUrl.Scheme);
        Assert.Equal(expectedUrl.Host, actualUrl.Host);
        Assert.Equal(expectedUrl.AbsolutePath, actualUrl.AbsolutePath);
        Assert.Equal(expectedUrl.Query, actualUrl.Query);
    }
}
