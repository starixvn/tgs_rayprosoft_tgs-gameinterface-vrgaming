using Tgs.ServiceUtility;

namespace Tgs.GameInterface.VRGaming.Core.ApplicationServices;

internal class FundOutGetService(
	IGameApiClient client) : IGetService<FundOutQuery, FundResult>
{
	public async ValueTask<FundResult> GetAsync(FundOutQuery query, CancellationToken cancellationToken = default)
	{
		var response = await client.DepositAsync(
			apiUrl: query.ApiUrl,
			version: query.Version,
			merchantId: query.MerchantId,
			deskey: query.Deskey,
			playerName: query.PlayerCode,
			serialNumber: query.OrderId,
			amount: query.Amount,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return new FundResult(
			OrderId: response.SerialNumber,
			Resp: response.Resp);
	}
}
