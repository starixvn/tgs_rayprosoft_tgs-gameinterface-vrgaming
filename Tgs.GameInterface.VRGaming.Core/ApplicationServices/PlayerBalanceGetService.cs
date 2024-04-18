using Tgs.ServiceUtility;

namespace Tgs.GameInterface.VRGaming.Core.ApplicationServices;

internal class PlayerBalanceGetService(
	IGameApiClient client) : IGetService<PlayerBalanceQuery, PlayerBalanceResult>
{
	public async ValueTask<PlayerBalanceResult> GetAsync(PlayerBalanceQuery query, CancellationToken cancellationToken = default)
	{
		var response = await client.BalanceAsync(
			apiUrl: query.ApiUrl,
			version: query.Version,
			merchantId: query.MerchantId,
			deskey: query.Deskey,
			playerName: query.PlayerCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return new PlayerBalanceResult(
			IsPlayerExist: response.IsPlayerExist,
			Balance: response.Balance);
	}
}
