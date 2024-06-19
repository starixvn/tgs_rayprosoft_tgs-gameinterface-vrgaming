using System.Text.Json;
using System.Text.Json.Nodes;

namespace Tgs.GameInterface.VRGaming.Core.UnitTest;

public class AesCryptographyUtensilUnitTest
{
	[Fact]
	public async Task UserWallet_Balance_Encryption()
	{
		var data = new JsonObject
		{
			["playerName"] = "4ucmqplxb029"
		};
		var secretKey = "8VF408JBV2B406488F2VH2X684T6R6DV";
		var utensil = new AesCryptographyUtensil();

		var actual = await utensil.EncryptionAsync(
			data: data.ToJsonString(),
			secretKey: secretKey);

		Assert.Equal("WK7HCiRT7YbaumsZuZp7dFee8OB106fIb6BPo8mu2a4=", actual);
	}

	[Fact]
	public async Task Account_CreateUser_Encryption()
	{
		var data = new JsonObject
		{
			["playerName"] = "4ucmqplxb029"
		};
		var secretKey = "8VF408JBV2B406488F2VH2X684T6R6DV";
		var utensil = new AesCryptographyUtensil();

		var actual = await utensil.EncryptionAsync(
			data: data.ToJsonString(),
			secretKey: secretKey);

		Assert.Equal("WK7HCiRT7YbaumsZuZp7dFee8OB106fIb6BPo8mu2a4=", actual);
	}

	[Fact]
	public async Task Login_Validate_Encryption()
	{
		var data = new JsonObject
		{
			["playerName"] = "4ucmqplxb029",
			["loginTime"] = new DateTimeOffset(2023, 12, 22, 06, 10, 07, TimeSpan.Zero).UtcDateTime.ToString("yyyy-MM-ddTHH:mm:ssZ"),
			["channelId"] = 0.ToString("D"),
			["isCreateUser"] = "true",
			["channelType"] = 0
		};
		var secretKey = "8VF408JBV2B406488F2VH2X684T6R6DV";
		var utensil = new AesCryptographyUtensil();
		var path = string.Join("&", values: JsonSerializer.Deserialize<Dictionary<string, object>>(data.ToJsonString())!.Where(predicate: predicate => predicate.Value is not null).Select(selector: selector => $"{selector.Key}={selector.Value}"));
		var actual = await utensil.EncryptionAsync(
			data: path,
			secretKey: secretKey);

		Assert.Equal("Vkdj0OWkkC1EkWeN2V8MHHeNHTrRTJwKsD1khnT6esg4v4/MXvuo+GMJFr8DiwxQsTfx9G3UGDhwyVVI78vGhDRDoZJk+IsHehTDI+0aM+cwCQPJUccwHFJHSvgPSYlGs4NLINGdnpB4mxx10vkQRg==", actual);
	}

	[Fact]
	public async Task Account_CreateUser_Decrypt()
	{
		var data = "0pbZCPdihj4cMF7Gdjw5sg==";
		var secretKey = "8VF408JBV2B406488F2VH2X684T6R6DV";
		var utensil = new AesCryptographyUtensil();

		var actual = await utensil.DecryptAsync(
			data: data,
			secretKey: secretKey,
			isDecodeUrl: false);

		Assert.Equal("{\"errorCode\":0}", actual);
	}
}
