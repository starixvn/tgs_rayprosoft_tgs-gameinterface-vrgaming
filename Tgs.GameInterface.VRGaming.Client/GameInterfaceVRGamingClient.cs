using System.Runtime.CompilerServices;
using Google.Protobuf.WellKnownTypes;
using Tgs.GameInterface.VRGaming.Protos;

namespace Tgs.GameInterface.VRGaming.Client;

internal class GameInterfaceVRGamingClient(
    GameInterfaceVRGaming.GameInterfaceVRGamingClient client) : IGameInterfaceVRGamingClient
{
    public async ValueTask<string> CheckingServiceAsync(string machineName, CancellationToken cancellationToken = default)
    {
        var request = new CheckingServiceRequest
        {
            MachineName = machineName
        };

        var reply = await client.CheckingServiceAsync(
            request: request,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return reply.MachineName;
    }

    public async ValueTask<FundReply> FundInAsync(Uri apiUrl, string version, string merchantId, string deskey, string playerCode, string orderId, decimal amount, CancellationToken cancellationToken = default)
    {
        var request = new FundRequest
        {
            ApiUrl = apiUrl.ToString(),
            Version = version,
            MerchantId = merchantId,
            Deskey = deskey,
            PlayerCode = playerCode,
            OrderId = orderId,
            Amount = amount
        };

        return await client.FundInAsync(
            request: request,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }

    public async ValueTask<FundReply> FundOutAsync(Uri apiUrl, string version, string merchantId, string deskey, string playerCode, string orderId, decimal amount, CancellationToken cancellationToken = default)
    {
        var request = new FundRequest
        {
            ApiUrl = apiUrl.ToString(),
            Version = version,
            MerchantId = merchantId,
            Deskey = deskey,
            PlayerCode = playerCode,
            OrderId = orderId,
            Amount = amount
        };

        return await client.FundOutAsync(
            request: request,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }

    public async ValueTask<bool> FundStatusAsync(Uri apiUrl, string version, string merchantId, string deskey, string playerCode, string orderId, DateTimeOffset startTime, DateTimeOffset endTime, CancellationToken cancellationToken = default)
    {
        var request = new FundStatusRequest
        {
            ApiUrl = apiUrl.ToString(),
            Version = version,
            MerchantId = merchantId,
            Deskey = deskey,
            PlayerCode = playerCode,
            OrderId = orderId,
            StartTime = startTime.ToTimestamp(),
            EndTime = endTime.ToTimestamp()
        };

        var reply = await client.FundStatusAsync(
            request: request,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return reply.State;
    }

    public async ValueTask<string> GameUrlAsync(Uri apiUrl, string version, string merchantId, string deskey, string playerCode, int externalGameId, CancellationToken cancellationToken = default)
    {
        var request = new GameUrlRequest
        {
            ApiUrl = apiUrl.ToString(),
            Version = version,
            MerchantId = merchantId,
            Deskey = deskey,
            PlayerCode = playerCode,
            ExternalGameId = externalGameId
        };

        var gameUrlReply = await client.GameUrlAsync(
            request: request,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return gameUrlReply.GameUrl;
    }

    public async IAsyncEnumerable<GameWagerInfo> GameWagerSettledAsync(Uri apiUrl, string version, string merchantId, string deskey, DateTimeOffset startTime, DateTimeOffset endTime, int pageNumber = 1, int pageSize = 1000, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var request = new GameWagerRequest
        {
            ApiUrl = apiUrl.ToString(),
            Version = version,
            MerchantId = merchantId,
            Deskey = deskey,
            StartTime = startTime.ToTimestamp(),
            EndTime = endTime.ToTimestamp(),
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var reply = await client.GameWagerSettledAsync(
            request: request,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        if (!(reply.Items ?? Enumerable.Empty<GameWagerInfo>()).Any())
        {
            yield break;
        }

        foreach (var item in reply.Items ?? Enumerable.Empty<GameWagerInfo>())
        {
            yield return item;
        }
    }

    public async IAsyncEnumerable<GameWagerInfo> GameWagerUnsettledAsync(Uri apiUrl, string version, string merchantId, string deskey, DateTimeOffset startTime, DateTimeOffset endTime, int pageNumber = 1, int pageSize = 1000, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var request = new GameWagerRequest
        {
            ApiUrl = apiUrl.ToString(),
            Version = version,
            MerchantId = merchantId,
            Deskey = deskey,
            StartTime = startTime.ToTimestamp(),
            EndTime = endTime.ToTimestamp(),
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var reply = await client.GameWagerUnsettledAsync(
            request: request,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        if (!(reply.Items ?? Enumerable.Empty<GameWagerInfo>()).Any())
        {
            yield break;
        }

        foreach (var item in reply.Items ?? Enumerable.Empty<GameWagerInfo>())
        {
            yield return item;
        }
    }

    public async ValueTask<PlayerBalanceReply> PlayerBalanceAsync(Uri apiUrl, string version, string merchantId, string deskey, string playerCode, CancellationToken cancellationToken = default)
    {
        var request = new PlayerBalanceRequest
        {
            ApiUrl = apiUrl.ToString(),
            Version = version,
            MerchantId = merchantId,
            Deskey = deskey,
            PlayerCode = playerCode
        };

        return await client.PlayerBalanceAsync(
            request: request,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }

    public async ValueTask PlayerCreateAsync(Uri apiUrl, string version, string merchantId, string deskey, string playerCode, CancellationToken cancellationToken = default)
    {
        var request = new PlayerCreateRequest
        {
            ApiUrl = apiUrl.ToString(),
            Version = version,
            MerchantId = merchantId,
            Deskey = deskey,
            PlayerCode = playerCode
        };

        _ = await client.PlayerCreateAsync(
            request: request,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }

    public async ValueTask PlayerLogoutAsync(Uri apiUrl, string version, string merchantId, string deskey, string playerCode, CancellationToken cancellationToken = default)
    {
        var request = new PlayerLogoutRequest
        {
            ApiUrl = apiUrl.ToString(),
            Version = version,
            MerchantId = merchantId,
            Deskey = deskey,
            PlayerCode = playerCode
        };

        _ = await client.PlayerLogoutAsync(
            request: request,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }
}
