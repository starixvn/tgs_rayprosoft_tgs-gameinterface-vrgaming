using System.Net;
using System.Net.Mime;
using System.Text;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using RichardSzalay.MockHttp;

namespace Tgs.GameInterface.VRGaming.Core.UnitTest;

public class WithdrawUnitTest
{
	[Fact]
	public async void Normal_Process()
	{
		Uri apiUrl = new Uri("https://fe.vrbetdemo.com/");
		var version = "1.0";
		var merchantId = "PEGA";
		var deskey = "8VF408JBV2B406488F2VH2X684T6R6DV";
		var playerName = "4ucmqplxb029";
		var serialNumber = "018c8f8ee6a60000000107e7db655fdb";
		var amount = 1.0m;

		var fakeCryptography = Substitute.For<IAesCryptographyUtensil>();
		var fakeSystemClock = Substitute.For<ISystemClock>();

		var currentTime = new DateTimeOffset(2023, 12, 22, 3, 33, 53, TimeSpan.Zero);
		_ = fakeSystemClock.CurrentTime.Returns(currentTime);

		var parameters = new JsonObject
		{
			["serialNumber"] = serialNumber,
			["playerName"] = playerName,
			["type"] = 0,
			["amount"] = amount,
			["createTime"] = currentTime.UtcDateTime.ToString("yyyy-MM-ddTHH:mm:ssZ")
		};
		var dataEncrypt = "3wNqIsKZT/kMmnFXN91o0Ca5c2UwxltQzquSULmdn\\u002BuLYK/\\u002B/DC4FtEnllpzyT68hk2WTEqme\\u002Bzz7r02xh4AQf\\u002BLjaKbjtjPrBYZc3BZFLTVIFjMdJL1WrqygUIquf5Vy5ibJtdImzxXctwOnpzXJpJV4vjpOAP59Kzbd6C\\u002Bi9W\\u002BL54C5Y\\u002BQF42MAMigSFAp";
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
		var uriBuilder = new UriBuilder(new Uri(apiUrl, "/UserWallet/Transaction"));
		var responseContent = "3wNqIsKZT/kMmnFXN91o0Ca5c2UwxltQzquSULmdn+uLYK/+/DC4FtEnllpzyT68hk2WTEqme+zz7r02xh4AQf+LjaKbjtjPrBYZc3BZFLTVIFjMdJL1WrqygUIquf5V8qMqHGbLAYJEtXDXQmLx3BId7VN7JrXB53vP6tbH+MR1/NJKSnjGMM93rO+wivMfZjlbnO3RSxE5cugPR3UgxomPT4faz8Cj0iW2/LHJQu4=";
		var handler = new MockHttpMessageHandler();
		_ = handler.Expect(HttpMethod.Post, uriBuilder.ToString())
			.WithContent(await requestContent.ReadAsStringAsync())
			.Respond(HttpStatusCode.OK, new StringContent(responseContent, Encoding.UTF8, MediaTypeNames.Text.Html));

		var responseDecrypt = "{\"serialNumber\":\"018c8f8ee6a60000000107e7db655fdb\",\"playerName\":\"4ucmqplxb029\",\"type\":1,\"amount\":1.0,\"createTime\":\"2023-12-22T03:33:53Z\",\"state\":0,\"balance\":0.000}";
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
			merchantId: merchantId,
			deskey: deskey,
			playerName: playerName,
			serialNumber: serialNumber,
			amount: amount);

		Assert.Equal("018c8f8ee6a60000000107e7db655fdb", actual.SerialNumber);
		Assert.Equal(responseDecrypt, actual.Resp);
	}
}
