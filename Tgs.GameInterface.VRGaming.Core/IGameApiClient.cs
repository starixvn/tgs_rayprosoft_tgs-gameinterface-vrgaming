using Tgs.GameInterface.VRGaming.Core.Models;

namespace Tgs.GameInterface.VRGaming.Core;

public interface IGameApiClient
{
    ValueTask<bool> TransactionRecordAsync(Uri apiUrl, string version, string merchantid, string deskey, string playerName, string serialNumber, DateTimeOffset startTime, DateTimeOffset endTime, CancellationToken cancellationToken = default);

    ValueTask<TransactionResponse> WithdrawAsync(Uri apiUrl, string version, string merchantid, string deskey, string playerName, string serialNumber, decimal amount, CancellationToken cancellationToken = default);

    ValueTask<TransactionResponse> DepositAsync(Uri apiUrl, string version, string merchantid, string deskey, string playerName, string serialNumber, decimal amount, CancellationToken cancellationToken = default);

    ValueTask<BalanceResponse> BalanceAsync(Uri apiUrl, string version, string merchantid, string deskey, string playerName, CancellationToken cancellationToken = default);

    ValueTask<string> LoginValidateAsync(Uri apiUrl, string version, string merchantid, string deskey, string playerName, int channelId = 0, CancellationToken cancellationToken = default);

    ValueTask CreateUserAsync(Uri apiUrl, string version, string merchantid, string deskey, string playerName, CancellationToken cancellationToken = default);

    ValueTask KickUserAsync(Uri apiUrl, string version, string merchantid, string deskey, string playerName, CancellationToken cancellationToken = default);

    IAsyncEnumerable<MerchantQueryResponse> MerchantQueryBetAsync(Uri apiUrl, string version, string merchantid, string deskey, DateTimeOffset startTime, DateTimeOffset endTime, int recordPage = 0, int recordCountPerPage = 1000, CancellationToken cancellationToken = default);
}
