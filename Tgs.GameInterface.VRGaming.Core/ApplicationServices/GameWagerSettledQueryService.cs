using System.Runtime.CompilerServices;
using Tgs.GameInterface.VRGaming.Core.Models;
using Tgs.ServiceUtility;

namespace Tgs.GameInterface.VRGaming.Core.ApplicationServices;

internal class GameWagerSettledQueryService(
    IGameApiClient client) : IQueryService<GameWagerSettledQuery, GameWagerResult>
{
    public async IAsyncEnumerable<GameWagerResult> QueryAsync(
        GameWagerSettledQuery query,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var responses = await client
            .MerchantQueryBetAsync(
            apiUrl: query.ApiUrl,
            version: query.Version,
            merchantId: query.MerchantId,
            deskey: query.Deskey,
            isUpdateTime: true,
            state: MerchantQueryBetStates.All,
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
            var isClosed = false;
            var isCanceled = false;
            var isResettle = false;
            var externalCreatedOn = response.CreateTime;
            var externalClosedOn = DateTimeOffset.UnixEpoch;
            var externalResettleOn = DateTimeOffset.UnixEpoch;
            var bet = response.Cost;
            var settle = 0.0m;
            var winLoss = 0.0m;
            if (response.State != 0)
            {
                isClosed = true;
                externalClosedOn = response.UpdateTime;
                settle = response.PlayerPrize;
                winLoss = settle - bet;
                if (response.SubState != 0)
                {
                    if ((response.SubState & 1) == 1 || (response.SubState & 2) == 2 || (response.SubState & 4) == 4)
                    {
                        isCanceled = true;
                        winLoss = 0;
                    }
                    if ((response.SubState & 8) == 8)
                    {
                        isResettle = true;
                        externalResettleOn = response.UpdateTime;
                        settle = response.PlayerPrize - response.LossPrize;
                        winLoss = settle - bet;
                    }
                }
            }
            yield return new GameWagerResult(
                ExternalRoundId: response.IssueNumber,
                ExternalWagerId: response.SerialNumber,
                ExternalGameId: response.ChannelId,
                PlayerCode: response.PlayerName,
                IsClosed: isClosed,
                IsCanceled: isCanceled,
                IsResettle: isResettle,
                ExternalCreatedOn: externalCreatedOn,
                ExternalClosedOn: externalClosedOn,
                ExternalResettleOn: externalResettleOn,
                Bet: bet,
                Settle: settle,
                WinLoss: winLoss,
                AdditionalData: response.AdditionalData);
        }
    }
}
