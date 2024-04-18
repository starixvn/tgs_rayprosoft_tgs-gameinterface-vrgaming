namespace Tgs.GameInterface.VRGaming.Core.Models;

public record MerchantQueryResponse(
	string IssueNumber,
	string SerialNumber,
	string ChannelId,
	string PlayerName,
	int State,
	int SubState,
	decimal Cost,
	decimal PlayerPrize,
	decimal LossPrize,
	DateTimeOffset CreateTime,
	DateTimeOffset UpdateTime,
	string AdditionalData);
