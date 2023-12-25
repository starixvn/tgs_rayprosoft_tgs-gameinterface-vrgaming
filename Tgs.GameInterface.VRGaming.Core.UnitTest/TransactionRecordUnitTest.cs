using System.Net.Mime;
using System.Net;
using System.Text;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using RichardSzalay.MockHttp;
using System.Text.Json.Nodes;
using Tgs.GameInterface.VRGaming.Core.Exceptions;
using Microsoft.Extensions.Logging;
using System.Security.Principal;

namespace Tgs.GameInterface.VRGaming.Core.UnitTest;

public class TransactionRecordUnitTest
{
    [Fact]
    public async void This_order_is_established()
    {
        Uri apiUrl = new Uri("https://fe.vrbetdemo.com/");
        var version = "1.0";
        var merchantId = "PEGA";
        var deskey = "8VF408JBV2B406488F2VH2X684T6R6DV";
        var playerName = "4ucmqplxb029";
        var serialNumber = "018c8f8f201f0000000107e7db655fdb";
        var startTime = new DateTimeOffset(2023, 12, 22, 3, 27, 00, TimeSpan.Zero);
        var endTime = new DateTimeOffset(2023, 12, 22, 3, 42, 00, TimeSpan.Zero);

        var fakeCryptography = Substitute.For<IAesCryptographyUtensil>();
        var fakeSystemClock = Substitute.For<ISystemClock>();

        var parameters = new JsonObject
        {
            ["startTime"] = startTime.UtcDateTime.ToString("yyyy-MM-ddTHH:mm:ssZ"),
            ["endTime"] = endTime.UtcDateTime.ToString("yyyy-MM-ddTHH:mm:ssZ"),
            ["serialNumber"] = serialNumber,
            ["playerName"] = playerName,
            ["recordPage"] = 0,
            ["recordCountPerPage"] = 10,
        };
        var dataEncrypt = "cXiY5p1vADpS7ACz/B2Iufdw7JGD3IP95R2UGkWwEfpXs87ITKQGMpD7B6cOmoPxtGr8c/dnNawvd5lUc5DfDWbUROFGskWKj8AAF2xCdEvse/W74DluQs1Ll/X2CB/Ch5snRHS6tZ0e7HV9e\\u002BLk57\\u002BI3B3lVEPxCIHlLA7Cd4qauavCEyFeFaFKW2KKe4ObbWrxbzhps62fBsTeBddUkcItDxuupi8Va09y\\u002BF/gZWJHqLEleWwxDnlzrG58\\u002BhUO";
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
        var uriBuilder = new UriBuilder(new Uri(apiUrl, "/UserWallet/TransactionRecord"));
        var responseContent = "VyECIVf/xUITfjC8xucrWrTenA5g9viX3mMd9+ik8bFIdjn8JOApHuWwsmxpCn4bOYOtCKZTN/QmkiMzKg/j7kHyOFo0kRhxQw7xEy7VwPnse/W74DluQs1Ll/X2CB/Ch5snRHS6tZ0e7HV9e+Lk57+I3B3lVEPxCIHlLA7Cd4qauavCEyFeFaFKW2KKe4Obp7rnqcgQYZlJ32PcJ3qaegJBqVveFwn4A/W3NhXtjt0VRW8vs46CvG702/xf+iDhn+RNPCt30R0N3TFQARUMV9vwHBMWLWHTa6z3aMqXc59cMEqP19w8k8NUXxeNz2Tt";
        var handler = new MockHttpMessageHandler();
        _ = handler.Expect(HttpMethod.Post, uriBuilder.ToString())
            .WithContent(await requestContent.ReadAsStringAsync())
            .Respond(HttpStatusCode.OK, new StringContent(responseContent, Encoding.UTF8, MediaTypeNames.Text.Html));

        var responseDecrypt = "{\"totalRecords\":1,\"recordPage\":0,\"recordCountPerPage\":10,\"records\":[{\"serialNumber\":\"018c8f8f201f0000000107e7db655fdb\",\"playerName\":\"4ucmqplxb029\",\"type\":0,\"amount\":1.00000,\"createTime\":\"2023-12-22T03:27:52Z\",\"state\":0,\"balance\":1.00000}]}";
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

        var actual = await client.TransactionRecordAsync(
            apiUrl: apiUrl,
            version: version,
            merchantId: merchantId,
            deskey: deskey,
            playerName: playerName,
            serialNumber: serialNumber,
            startTime: startTime,
            endTime: endTime);

        Assert.True(actual);
    }

    [Fact]
    public async void This_order_is_not_established()
    {
        Uri apiUrl = new Uri("https://fe.vrbetdemo.com/");
        var version = "1.0";
        var merchantId = "PEGA";
        var deskey = "8VF408JBV2B406488F2VH2X684T6R6DV";
        var playerName = "4ucmqplxb029";
        var serialNumber = "018c8f8ee6a60000000107e7db655fdb";
        var startTime = new DateTimeOffset(2023, 12, 22, 3, 27, 00, TimeSpan.Zero);
        var endTime = new DateTimeOffset(2023, 12, 22, 3, 42, 00, TimeSpan.Zero);

        var fakeCryptography = Substitute.For<IAesCryptographyUtensil>();
        var fakeSystemClock = Substitute.For<ISystemClock>();

        var parameters = new JsonObject
        {
            ["startTime"] = startTime.UtcDateTime.ToString("yyyy-MM-ddTHH:mm:ssZ"),
            ["endTime"] = endTime.UtcDateTime.ToString("yyyy-MM-ddTHH:mm:ssZ"),
            ["serialNumber"] = serialNumber,
            ["playerName"] = playerName,
            ["recordPage"] = 0,
            ["recordCountPerPage"] = 10,
        };
        var dataEncrypt = "cXiY5p1vADpS7ACz/B2Iufdw7JGD3IP95R2UGkWwEfpXs87ITKQGMpD7B6cOmoPxtGr8c/dnNawvd5lUc5DfDWbUROFGskWKj8AAF2xCdEvse/W74DluQs1Ll/X2CB/Ch5snRHS6tZ0e7HV9e\\u002BLk57\\u002BI3B3lVEPxCIHlLA7Cd4qauavCEyFeFaFKW2KKe4ObbWrxbzhps62fBsTeBddUkcItDxuupi8Va09y\\u002BF/gZWJHqLEleWwxDnlzrG58\\u002BhUO";
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
        var uriBuilder = new UriBuilder(new Uri(apiUrl, "/UserWallet/TransactionRecord"));
        var responseContent = "VyECIVf/xUITfjC8xucrWl28MHY/pE7dPebmGFaOLs9Idjn8JOApHuWwsmxpCn4bOYOtCKZTN/QmkiMzKg/j7g9/REuAHeuJBAtF0DrFxHA=";
        var handler = new MockHttpMessageHandler();
        _ = handler.Expect(HttpMethod.Post, uriBuilder.ToString())
            .WithContent(await requestContent.ReadAsStringAsync())
            .Respond(HttpStatusCode.OK, new StringContent(responseContent, Encoding.UTF8, MediaTypeNames.Text.Html));

        var responseDecrypt = "{\"totalRecords\":0,\"recordPage\":0,\"recordCountPerPage\":10,\"records\":[]}";
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

        var actual = await client.TransactionRecordAsync(
            apiUrl: apiUrl,
            version: version,
            merchantId: merchantId,
            deskey: deskey,
            playerName: playerName,
            serialNumber: serialNumber,
            startTime: startTime,
            endTime: endTime);

        Assert.False(actual);
    }

    [Fact]
    public async void Insufficient_balance()
    {
        Uri apiUrl = new Uri("https://fe.vrbetdemo.com/");
        var version = "1.0";
        var merchantId = "PEGA";
        var deskey = "8VF408JBV2B406488F2VH2X684T6R6DV";
        var playerName = "4ucmqplxb029";
        var serialNumber = "018c8f8ee6a60000000107e7db655fdb";
        var startTime = new DateTimeOffset(2023, 12, 22, 3, 27, 00, TimeSpan.Zero);
        var endTime = new DateTimeOffset(2023, 12, 22, 3, 42, 00, TimeSpan.Zero);

        var fakeCryptography = Substitute.For<IAesCryptographyUtensil>();
        var fakeSystemClock = Substitute.For<ISystemClock>();

        var parameters = new JsonObject
        {
            ["startTime"] = startTime.UtcDateTime.ToString("yyyy-MM-ddTHH:mm:ssZ"),
            ["endTime"] = endTime.UtcDateTime.ToString("yyyy-MM-ddTHH:mm:ssZ"),
            ["serialNumber"] = serialNumber,
            ["playerName"] = playerName,
            ["recordPage"] = 0,
            ["recordCountPerPage"] = 10,
        };
        var dataEncrypt = "cXiY5p1vADpS7ACz/B2Iufdw7JGD3IP95R2UGkWwEfpXs87ITKQGMpD7B6cOmoPxtGr8c/dnNawvd5lUc5DfDWbUROFGskWKj8AAF2xCdEvse/W74DluQs1Ll/X2CB/Ch5snRHS6tZ0e7HV9e\\u002BLk57\\u002BI3B3lVEPxCIHlLA7Cd4qauavCEyFeFaFKW2KKe4ObbWrxbzhps62fBsTeBddUkcItDxuupi8Va09y\\u002BF/gZWJHqLEleWwxDnlzrG58\\u002BhUO";
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
        var uriBuilder = new UriBuilder(new Uri(apiUrl, "/UserWallet/TransactionRecord"));
        var responseContent = "VyECIVf/xUITfjC8xucrWrTenA5g9viX3mMd9+ik8bFIdjn8JOApHuWwsmxpCn4bOYOtCKZTN/QmkiMzKg/j7kHyOFo0kRhxQw7xEy7VwPlDv6mGcx8T5XDmiHJ4HOtquWpp7i6cIo+my8/odjbONr+I3B3lVEPxCIHlLA7Cd4qauavCEyFeFaFKW2KKe4Ob4i8TVsasiAVGUXUaaRh6xQJBqVveFwn4A/W3NhXtjt0VRW8vs46CvG702/xf+iDhScD2hGGjaR1TitmL/AZWFkzdWujvDA20jE4WRw4XfzksKDd+doIJLJa4jQ8hCo+BLaoYEYr8/dcE6/xx3klq4w==";
        var handler = new MockHttpMessageHandler();
        _ = handler.Expect(HttpMethod.Post, uriBuilder.ToString())
            .WithContent(await requestContent.ReadAsStringAsync())
            .Respond(HttpStatusCode.OK, new StringContent(responseContent, Encoding.UTF8, MediaTypeNames.Text.Html));

        var responseDecrypt = "{\"totalRecords\":1,\"recordPage\":0,\"recordCountPerPage\":10,\"records\":[{\"serialNumber\":\"018c8f8ee6a60000000107e7db655fdb\",\"playerName\":\"4ucmqplxb029\",\"type\":1,\"amount\":1.00000,\"createTime\":\"2023-12-22T03:59:05Z\",\"state\":10,\"balance\":0.00000}]}";
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

        _ = await Assert.ThrowsAsync<ErrorCodeException>(
            () => client.TransactionRecordAsync(
                apiUrl: apiUrl,
                version: version,
                merchantId: merchantId,
                deskey: deskey,
                playerName: playerName,
                serialNumber: serialNumber,
                startTime: startTime,
                endTime: endTime).AsTask());
    }
}
