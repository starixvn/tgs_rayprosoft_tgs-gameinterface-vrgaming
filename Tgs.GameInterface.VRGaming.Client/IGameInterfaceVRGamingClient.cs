using Tgs.GameInterface.VRGaming.Protos;

namespace Tgs.GameInterface.VRGaming.Client;

public interface IGameInterfaceVRGamingClient
{
	ValueTask<string> CheckingServiceAsync(string machineName, CancellationToken cancellationToken = default);

	ValueTask<FundReply> FundInAsync(Uri apiUrl, string version, string merchantId, string deskey, string playerCode, string orderId, decimal amount, CancellationToken cancellationToken = default);

	ValueTask<FundReply> FundOutAsync(Uri apiUrl, string version, string merchantId, string deskey, string playerCode, string orderId, decimal amount, CancellationToken cancellationToken = default);

	ValueTask<bool> FundStatusAsync(Uri apiUrl, string version, string merchantId, string deskey, string playerCode, string orderId, DateTimeOffset startTime, DateTimeOffset endTime, CancellationToken cancellationToken = default);

	IAsyncEnumerable<GameWagerInfo> GameWagerUnsettledAsync(Uri apiUrl, string version, string merchantId, string deskey, DateTimeOffset startTime, DateTimeOffset endTime, int pageNumber = 1, int pageSize = 1000, CancellationToken cancellationToken = default);

	IAsyncEnumerable<GameWagerInfo> GameWagerSettledAsync(Uri apiUrl, string version, string merchantId, string deskey, DateTimeOffset startTime, DateTimeOffset endTime, int pageNumber = 1, int pageSize = 1000, CancellationToken cancellationToken = default);

	ValueTask<string> GameUrlAsync(Uri apiUrl, string version, string merchantId, string deskey, string playerCode, int externalGameId, CancellationToken cancellationToken = default);

	ValueTask<PlayerBalanceReply> PlayerBalanceAsync(Uri apiUrl, string version, string merchantId, string deskey, string playerCode, CancellationToken cancellationToken = default);

	ValueTask PlayerLogoutAsync(Uri apiUrl, string version, string merchantId, string deskey, string playerCode, CancellationToken cancellationToken = default);

	ValueTask PlayerCreateAsync(Uri apiUrl, string version, string merchantId, string deskey, string playerCode, CancellationToken cancellationToken = default);
}
