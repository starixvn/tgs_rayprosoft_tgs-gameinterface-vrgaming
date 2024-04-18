using System.Runtime.CompilerServices;
using Tgs.GameInterface.VRGaming.Core.Models;
using Tgs.ServiceUtility;

namespace Tgs.GameInterface.VRGaming.Core.ApplicationServices;

internal class GameWagerUnsettledQueryService(
	IGameApiClient client) : IQueryService<GameWagerUnsettledQuery, GameWagerResult>
{
	public async IAsyncEnumerable<GameWagerResult> QueryAsync(
		GameWagerUnsettledQuery query,
		[EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		var responses = await client
			.MerchantQueryBetAsync(
			apiUrl: query.ApiUrl,
			version: query.Version,
			merchantId: query.MerchantId,
			deskey: query.Deskey,
			isUpdateTime: false,
			state: MerchantQueryBetStates.Unsettled,
			startTime: query.StartTime,
			endTime: query.EndTime,
			recordPage: query.PageNumber - 1,
			recordCountPerPage: query.PageSize,
			cancellationToken: cancellationToken)
			.ToArrayAsync(
			cancellationToken: cancellationToken)
			.ConfigureAwait(false)
			?? Enumerable.Empty<MerchantQueryResponse>();

		if (!responses.Any())
		{
			yield break;
		}

		foreach (var response in responses)
		{
			yield return new GameWagerResult(
				ExternalRoundId: response.IssueNumber,
				ExternalWagerId: response.SerialNumber,
				ExternalGameId: response.ChannelId,
				PlayerCode: response.PlayerName,
				IsClosed: false,
				IsCanceled: false,
				IsResettle: false,
				ExternalCreatedOn: response.CreateTime,
				ExternalClosedOn: DateTimeOffset.UnixEpoch,
				ExternalResettleOn: DateTimeOffset.UnixEpoch,
				Bet: response.Cost,
				Settle: 0.0m,
				WinLoss: 0.0m,
				AdditionalData: response.AdditionalData);
		}
	}
}
