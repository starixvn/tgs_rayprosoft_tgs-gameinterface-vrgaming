using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Tgs.GameInterface.VRGaming.Core.ApplicationServices;
using Tgs.GameInterface.VRGaming.Protos;
using Tgs.ServiceUtility;

namespace Tgs.GameInterface.VRGaming.GrpcServices;

public class GameInterfaceVRGamingGrpcService(
    ILogger<GameInterfaceVRGamingGrpcService> logger,
    IGetService<FundInQuery, FundResult> fundInService,
    IGetService<FundOutQuery, FundResult> fundOutService,
    IGetService<FundStatusQuery, bool> fundStatusService,
    IGetService<GameUrlQuery, string> gameUrlService,
    IQueryService<GameWagerSettledQuery, GameWagerResult> gameWagerSettledService,
    IQueryService<GameWagerUnsettledQuery, GameWagerResult> gameWagerUnsettledService,
    IGetService<PlayerBalanceQuery, PlayerBalanceResult> playerBalanceService,
    ICommandService<PlayerCreateCommand> playerCreateService,
    ICommandService<PlayerLogoutCommand> playerLogoutService) : GameInterfaceVRGaming.GameInterfaceVRGamingBase
{
    public override Task<CheckingServiceReply> CheckingService(CheckingServiceRequest request, ServerCallContext context)
    {
        logger.LogWarning("LogOn:{logOn} Message: Create a check connection.({machineName})", DateTime.Now.ToString("o"), request.MachineName);

        return Task.FromResult(new CheckingServiceReply
        {
            MachineName = Environment.MachineName
        });
    }

    public override async Task<FundReply> FundIn(FundRequest request, ServerCallContext context)
    {
        var query = new FundInQuery(
            ApiUrl: new(request.ApiUrl),
            Version: request.Version,
            MerchantId: request.MerchantId,
            Deskey: request.Deskey,
            PlayerCode: request.PlayerCode,
            OrderId: request.OrderId,
            Amount: request.Amount);

        var result = await fundInService.GetAsync(
            query: query,
            cancellationToken: context.CancellationToken)
            .ConfigureAwait(false);

        return new FundReply
        {
            OrderId = result.OrderId,
            Resp = result.Resp
        };
    }

    public override async Task<FundReply> FundOut(FundRequest request, ServerCallContext context)
    {
        var query = new FundOutQuery(
            ApiUrl: new(request.ApiUrl),
            Version: request.Version,
            MerchantId: request.MerchantId,
            Deskey: request.Deskey,
            PlayerCode: request.PlayerCode,
            OrderId: request.OrderId,
            Amount: request.Amount);

        var result = await fundOutService.GetAsync(
            query: query,
            cancellationToken: context.CancellationToken)
            .ConfigureAwait(false);

        return new FundReply
        {
            OrderId = result.OrderId,
            Resp = result.Resp
        };
    }

    public override async Task<BaseReply> FundStatus(FundStatusRequest request, ServerCallContext context)
    {
        var query = new FundStatusQuery(
            ApiUrl: new(request.ApiUrl),
            Version: request.Version,
            MerchantId: request.MerchantId,
            Deskey: request.Deskey,
            PlayerCode: request.PlayerCode,
            OrderId: request.OrderId,
            StartTime: request.StartTime.ToDateTimeOffset(),
            EndTime: request.EndTime.ToDateTimeOffset());

        var result = await fundStatusService.GetAsync(
            query: query,
            cancellationToken: context.CancellationToken)
            .ConfigureAwait(false);

        return new BaseReply
        {
            State = result
        };
    }

    public override async Task<GameUrlReply> GameUrl(GameUrlRequest request, ServerCallContext context)
    {
        var query = new GameUrlQuery(
            ApiUrl: new(request.ApiUrl),
            Version: request.Version,
            MerchantId: request.MerchantId,
            Deskey: request.Deskey,
            PlayerCode: request.PlayerCode,
            ExternalGameId: request.ExternalGameId);

        var result = await gameUrlService.GetAsync(
            query: query,
            cancellationToken: context.CancellationToken)
            .ConfigureAwait(false);

        return new GameUrlReply
        {
            GameUrl = result
        };
    }

    public override async Task<GameWagerReply> GameWagerSettled(GameWagerRequest request, ServerCallContext context)
    {
        var query = new GameWagerSettledQuery(
            ApiUrl: new(request.ApiUrl),
            Version: request.Version,
            MerchantId: request.MerchantId,
            Deskey: request.Deskey,
            StartTime: request.StartTime.ToDateTimeOffset(),
            EndTime: request.EndTime.ToDateTimeOffset(),
            PageNumber: request.PageNumber,
            PageSize: request.PageSize);

        var results = await gameWagerSettledService.QueryAsync(
            query: query,
            cancellationToken: context.CancellationToken)
            .ToArrayAsync(
            cancellationToken: context.CancellationToken)
            .ConfigureAwait(false)
            ?? Enumerable.Empty<GameWagerResult>();

        var reply = new GameWagerReply();
        if (results.Any())
        {
            reply.Items.AddRange(results.Select(selector: selector => new GameWagerInfo
            {
                ExternalRoundId = selector.ExternalRoundId,
                ExternalWagerId = selector.ExternalWagerId,
                ExternalGameId = selector.ExternalGameId,
                PlayerCode = selector.PlayerCode,
                IsClosed = selector.IsClosed,
                IsCanceled = selector.IsCanceled,
                IsResettle = selector.IsResettle,
                ExternalCreatedOn = selector.ExternalCreatedOn.ToTimestamp(),
                ExternalClosedOn = selector.ExternalClosedOn.ToTimestamp(),
                ExternalResettleOn = selector.ExternalResettleOn.ToTimestamp(),
                Bet = selector.Bet,
                Settle = selector.Settle,
                PlayerWinLoss = selector.WinLoss,
                AdditionalData = selector.AdditionalData
            }));
        }

        return reply;
    }

    public override async Task<GameWagerReply> GameWagerUnsettled(GameWagerRequest request, ServerCallContext context)
    {
        var query = new GameWagerUnsettledQuery(
            ApiUrl: new(request.ApiUrl),
            Version: request.Version,
            MerchantId: request.MerchantId,
            Deskey: request.Deskey,
            StartTime: request.StartTime.ToDateTimeOffset(),
            EndTime: request.EndTime.ToDateTimeOffset(),
            PageNumber: request.PageNumber,
            PageSize: request.PageSize);

        var results = await gameWagerUnsettledService.QueryAsync(
            query: query,
            cancellationToken: context.CancellationToken)
            .ToArrayAsync(
            cancellationToken: context.CancellationToken)
            .ConfigureAwait(false)
            ?? Enumerable.Empty<GameWagerResult>();

        var reply = new GameWagerReply();
        if (results.Any())
        {
            reply.Items.AddRange(results.Select(selector: selector => new GameWagerInfo
            {
                ExternalRoundId = selector.ExternalRoundId,
                ExternalWagerId = selector.ExternalWagerId,
                ExternalGameId = selector.ExternalGameId,
                PlayerCode = selector.PlayerCode,
                IsClosed = selector.IsClosed,
                IsCanceled = selector.IsCanceled,
                IsResettle = selector.IsResettle,
                ExternalCreatedOn = selector.ExternalCreatedOn.ToTimestamp(),
                ExternalClosedOn = selector.ExternalClosedOn.ToTimestamp(),
                ExternalResettleOn = selector.ExternalResettleOn.ToTimestamp(),
                Bet = selector.Bet,
                Settle = selector.Settle,
                PlayerWinLoss = selector.WinLoss,
                AdditionalData = selector.AdditionalData
            }));
        }

        return reply;
    }

    public override async Task<PlayerBalanceReply> PlayerBalance(PlayerBalanceRequest request, ServerCallContext context)
    {
        var query = new PlayerBalanceQuery(
            ApiUrl: new(request.ApiUrl),
            Version: request.Version,
            MerchantId: request.MerchantId,
            Deskey: request.Deskey,
            PlayerCode: request.PlayerCode);

        var result = await playerBalanceService.GetAsync(
            query: query,
            cancellationToken: context.CancellationToken)
            .ConfigureAwait(false);

        return new PlayerBalanceReply
        {
            IsPlayerExist = result.IsPlayerExist,
            Balance = result.Balance
        };
    }

    public override async Task<BaseReply> PlayerCreate(PlayerCreateRequest request, ServerCallContext context)
    {
        var command = new PlayerCreateCommand(
            ApiUrl: new(request.ApiUrl),
            Version: request.Version,
            MerchantId: request.MerchantId,
            Deskey: request.Deskey,
            PlayerCode: request.PlayerCode);

        await playerCreateService.ExecuteAsync(
            command: command,
            cancellationToken: context.CancellationToken)
            .ConfigureAwait(false);

        return new BaseReply
        {
            State = true
        };
    }

    public override async Task<BaseReply> PlayerLogout(PlayerLogoutRequest request, ServerCallContext context)
    {
        var command = new PlayerLogoutCommand(
            ApiUrl: new(request.ApiUrl),
            Version: request.Version,
            MerchantId: request.MerchantId,
            Deskey: request.Deskey,
            PlayerCode: request.PlayerCode);

        await playerLogoutService.ExecuteAsync(
            command: command,
            cancellationToken: context.CancellationToken)
            .ConfigureAwait(false);

        return new BaseReply
        {
            State = true
        };
    }
}
