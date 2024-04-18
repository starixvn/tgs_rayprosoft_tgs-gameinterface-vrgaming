using Tgs.ServiceUtility;

namespace Tgs.GameInterface.VRGaming.Core.ApplicationServices;

public class FundStatusGetService(IGameApiClient client) : IGetService<FundStatusQuery, bool>
{
	public ValueTask<bool> GetAsync(
		FundStatusQuery query,
		CancellationToken cancellationToken = default)
		=> client.TransactionRecordAsync(
			apiUrl: query.ApiUrl,
			version: query.Version,
			merchantId: query.MerchantId,
			deskey: query.Deskey,
			playerName: query.PlayerCode,
			serialNumber: query.OrderId,
			startTime: query.StartTime,
			endTime: query.EndTime,
			cancellationToken: cancellationToken);
}
