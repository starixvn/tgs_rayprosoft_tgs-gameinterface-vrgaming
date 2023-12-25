using Microsoft.AspNetCore.Mvc;
using Tgs.GameInterface.VRGaming.Core.ApplicationServices;
using Tgs.GameInterface.VRGaming.ViewModels;
using Tgs.ServiceUtility;

namespace Tgs.GameInterface.VRGaming.Controllers;

[Route("[controller]")]
[ApiController]
public class VRController : ControllerBase
{
    /// <summary>
    /// Funds the in asynchronous.
    /// </summary>
    /// <param name="service">The service.</param>
    /// <param name="source">The source.</param>
    /// <returns></returns>
    /// <remarks>
    /// <b>Example</b><br />
    ///
    ///     Post
    ///
    ///         {
    ///             "apiUrl": "https://fe.vrbetdemo.com/",
    ///             "version": "1.0",
    ///             "merchantid": "PEGA",
    ///             "deskey": "8VF408JBV2B406488F2VH2X684T6R6DV",
    ///             "playerCode":"4ucmqplxb029",
    ///             "orderId": "018c8f8ee6a60000000107e7db655fdb",
    ///             "amount": 1
    ///         }
    /// </remarks>
    [HttpPost("fund/in", Name = "FundIn")]
    public async ValueTask<FundResultViewModel> FundInAsync(
        [FromServices] IGetService<FundInQuery, FundResult> service,
        FundViewModel source)
    {
        var query = new FundInQuery(
            ApiUrl: source.ApiUrl,
            Version: source.Version,
            MerchantId: source.Merchantid,
            Deskey: source.Deskey,
            PlayerCode: source.PlayerCode,
            OrderId: source.OrderId,
            Amount: source.Amount);

        var result = await service.GetAsync(
            query: query,
            cancellationToken: HttpContext.RequestAborted)
            .ConfigureAwait(false);

        return new FundResultViewModel(
            OrderId: result.OrderId,
            Resp: result.Resp);
    }

    /// <summary>
    /// Funds the out asynchronous.
    /// </summary>
    /// <param name="service">The service.</param>
    /// <param name="source">The source.</param>
    /// <returns></returns>
    /// <remarks>
    /// <b>Example</b><br />
    ///
    ///     Post
    ///
    ///         {
    ///             "apiUrl": "https://fe.vrbetdemo.com/",
    ///             "version": "1.0",
    ///             "merchantid": "PEGA",
    ///             "deskey": "8VF408JBV2B406488F2VH2X684T6R6DV",
    ///             "playerCode":"4ucmqplxb029",
    ///             "orderId": "018c8f8f201f0000000107e7db655fdb",
    ///             "amount": 1
    ///         }
    /// </remarks>
    [HttpPost("fund/out", Name = "FundOut")]
    public async ValueTask<FundResultViewModel> FundOutAsync(
        [FromServices] IGetService<FundOutQuery, FundResult> service,
        FundViewModel source)
    {
        var query = new FundOutQuery(
            ApiUrl: source.ApiUrl,
            Version: source.Version,
            MerchantId: source.Merchantid,
            Deskey: source.Deskey,
            PlayerCode: source.PlayerCode,
            OrderId: source.OrderId,
            Amount: source.Amount);

        var result = await service.GetAsync(
            query: query,
            cancellationToken: HttpContext.RequestAborted)
            .ConfigureAwait(false);

        return new FundResultViewModel(
            OrderId: result.OrderId,
            Resp: result.Resp);
    }

    /// <summary>
    /// Funds the status asynchronous.
    /// </summary>
    /// <param name="service">The service.</param>
    /// <param name="source">The source.</param>
    /// <returns></returns>
    /// <remarks>
    /// <b>Example</b><br />
    ///
    ///     Post
    ///
    ///         {
    ///             "apiUrl": "https://fe.vrbetdemo.com/",
    ///             "version": "1.0",
    ///             "merchantid": "PEGA",
    ///             "deskey": "8VF408JBV2B406488F2VH2X684T6R6DV",
    ///             "playerCode":"4ucmqplxb029",
    ///             "orderId": "018c8f8f201f0000000107e7db655fdb",
    ///             "startTime": "2023-12-22T11:27:00+08:00",
    ///             "endTime": "2023-12-22T11:42:00+08:00"
    ///         }
    /// </remarks>
    [HttpPost("fund/Status", Name = "FundStatus")]
    public async ValueTask<bool> FundStatusAsync(
        [FromServices] IGetService<FundStatusQuery, bool> service,
        FundStatusViewModel source)
    {
        var query = new FundStatusQuery(
            ApiUrl: source.ApiUrl,
            Version: source.Version,
            MerchantId: source.Merchantid,
            Deskey: source.Deskey,
            PlayerCode: source.PlayerCode,
            OrderId: source.OrderId,
            StartTime: source.StartTime,
            EndTime: source.EndTime);

        return await service.GetAsync(
            query: query,
            cancellationToken: HttpContext.RequestAborted)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Games the wager settled asynchronous.
    /// </summary>
    /// <param name="service">The service.</param>
    /// <param name="source">The source.</param>
    /// <returns></returns>
    /// <remarks>
    /// <b>Example</b><br />
    ///
    ///     Post
    ///
    ///         {
    ///             "apiUrl": "https://fe.vrbetdemo.com/",
    ///             "version": "1.0",
    ///             "merchantid": "PEGA",
    ///             "deskey": "8VF408JBV2B406488F2VH2X684T6R6DV",
    ///             "startTime": "2023-12-25T11:00:00+08:00",
    ///             "endTime": "2023-12-25T11:10:00+08:00",
    ///             "pageNumber": 1,
    ///             "pageSize": 1000
    ///         }
    /// </remarks>
    [HttpPost("game/wager/settled")]
    public async IAsyncEnumerable<GameWagerResultViewModel> GameWagerSettledAsync(
        [FromServices] IQueryService<GameWagerSettledQuery, GameWagerResult> service,
        GameWagerViewModel source)
    {
        var query = new GameWagerSettledQuery(
            ApiUrl: source.ApiUrl,
            Version: source.Version,
            MerchantId: source.Merchantid,
            Deskey: source.Deskey,
            StartTime: source.StartTime,
            EndTime: source.EndTime,
            PageNumber: source.PageNumber,
            PageSize: source.PageSize);

        var results = await service.QueryAsync(
            query: query,
            cancellationToken: HttpContext.RequestAborted)
            .ToArrayAsync(
            cancellationToken: HttpContext.RequestAborted)
            .ConfigureAwait(false)
            ?? Enumerable.Empty<GameWagerResult>();

        if (!results.Any())
        {
            yield break;
        }

        foreach (var result in results)
        {
            yield return new GameWagerResultViewModel(
                ExternalRoundId: result.ExternalRoundId,
                ExternalWagerId: result.ExternalWagerId,
                ExternalGameId: result.ExternalGameId,
                PlayerCode: result.PlayerCode,
                IsClosed: result.IsClosed,
                IsCanceled: result.IsCanceled,
                IsResettle: result.IsResettle,
                ExternalCreatedOn: result.ExternalCreatedOn,
                ExternalClosedOn: result.ExternalClosedOn,
                ExternalResettleOn: result.ExternalResettleOn,
                Bet: result.Bet,
                Settle: result.Settle,
                WinLoss: result.WinLoss,
                AdditionalData: result.AdditionalData);
        }
    }

    /// <summary>
    /// Games the wager unsettled asynchronous.
    /// </summary>
    /// <param name="service">The service.</param>
    /// <param name="source">The source.</param>
    /// <returns></returns>
    /// <remarks>
    /// <b>Example</b><br />
    ///
    ///     Post
    ///
    ///         {
    ///             "apiUrl": "https://fe.vrbetdemo.com/",
    ///             "version": "1.0",
    ///             "merchantid": "PEGA",
    ///             "deskey": "8VF408JBV2B406488F2VH2X684T6R6DV",
    ///             "startTime": "2023-12-25T09:20:00+08:00",
    ///             "endTime": "2023-12-25T09:30:00+08:00",
    ///             "pageNumber": 1,
    ///             "pageSize": 1000
    ///         }
    /// </remarks>
    [HttpPost("game/wager/unsettled")]
    public async IAsyncEnumerable<GameWagerResultViewModel> GameWagerUnsettledAsync(
        [FromServices] IQueryService<GameWagerUnsettledQuery, GameWagerResult> service,
        GameWagerViewModel source)
    {
        var query = new GameWagerUnsettledQuery(
            ApiUrl: source.ApiUrl,
            Version: source.Version,
            MerchantId: source.Merchantid,
            Deskey: source.Deskey,
            StartTime: source.StartTime,
            EndTime: source.EndTime,
            PageNumber: source.PageNumber,
            PageSize: source.PageSize);

        var results = await service.QueryAsync(
            query: query,
            cancellationToken: HttpContext.RequestAborted)
            .ToArrayAsync(
            cancellationToken: HttpContext.RequestAborted)
            .ConfigureAwait(false)
            ?? Enumerable.Empty<GameWagerResult>();

        if (!results.Any())
        {
            yield break;
        }

        foreach (var result in results)
        {
            yield return new GameWagerResultViewModel(
                ExternalRoundId: result.ExternalRoundId,
                ExternalWagerId: result.ExternalWagerId,
                ExternalGameId: result.ExternalGameId,
                PlayerCode: result.PlayerCode,
                IsClosed: result.IsClosed,
                IsCanceled: result.IsCanceled,
                IsResettle: result.IsResettle,
                ExternalCreatedOn: result.ExternalCreatedOn,
                ExternalClosedOn: result.ExternalClosedOn,
                ExternalResettleOn: result.ExternalResettleOn,
                Bet: result.Bet,
                Settle: result.Settle,
                WinLoss: result.WinLoss,
                AdditionalData: result.AdditionalData);
        }
    }

    /// <summary>
    /// Games the URL asynchronous.
    /// </summary>
    /// <param name="service">The service.</param>
    /// <param name="source">The source.</param>
    /// <returns></returns>
    /// <remarks>
    /// <b>Example</b><br />
    ///
    ///     Post
    ///
    ///         {
    ///             "apiUrl": "https://fe.vrbetdemo.com/",
    ///             "version": "1.0",
    ///             "merchantid": "PEGA",
    ///             "deskey": "8VF408JBV2B406488F2VH2X684T6R6DV",
    ///             "playerCode":"4ucmqplxb029",
    ///             "externalGameId": 0
    ///         }
    /// </remarks>
    [HttpPost("gameurl")]
    public ValueTask<string> GameUrlAsync(
        [FromServices] IGetService<GameUrlQuery, string> service,
        GameUrlViewModel source)
    {
        var query = new GameUrlQuery(
            ApiUrl: source.ApiUrl,
            Version: source.Version,
            MerchantId: source.Merchantid,
            Deskey: source.Deskey,
            PlayerCode: source.PlayerCode,
            ExternalGameId: source.ExternalGameId);

        return service
            .GetAsync(
            query: query,
            cancellationToken: HttpContext.RequestAborted);
    }

    /// <summary>
    /// Players the balance asynchronous.
    /// </summary>
    /// <param name="service">The service.</param>
    /// <param name="source">The source.</param>
    /// <returns></returns>
    /// <remarks>
    /// <b>Example</b><br />
    ///
    ///     Post
    ///
    ///         {
    ///             "apiUrl": "https://fe.vrbetdemo.com/",
    ///             "version": "1.0",
    ///             "merchantid": "PEGA",
    ///             "deskey": "8VF408JBV2B406488F2VH2X684T6R6DV",
    ///             "playerCode":"4ucmqplxb029"
    ///         }
    /// </remarks>
    [HttpPost("player/balance", Name = "PlayerBalance")]
    public async ValueTask<PlayerBalanceResultViewModel> PlayerBalanceAsync(
        [FromServices] IGetService<PlayerBalanceQuery, PlayerBalanceResult> service,
        PlayerBalanceViewModel source)
    {
        var query = new PlayerBalanceQuery(
            ApiUrl: source.ApiUrl,
            Version: source.Version,
            MerchantId: source.Merchantid,
            Deskey: source.Deskey,
            PlayerCode: source.PlayerCode);

        var result = await service
            .GetAsync(
            query: query,
            cancellationToken: HttpContext.RequestAborted)
            .ConfigureAwait(false);

        return new PlayerBalanceResultViewModel(
            IsPlayerExist: result.IsPlayerExist,
            Balance: result.Balance);
    }

    /// <summary>
    /// Players the logout asynchronous.
    /// </summary>
    /// <param name="service">The service.</param>
    /// <param name="source">The source.</param>
    /// <returns></returns>
    /// <remarks>
    /// <b>Example</b><br />
    ///
    ///     Post
    ///
    ///         {
    ///             "apiUrl": "https://fe.vrbetdemo.com/",
    ///             "version": "1.0",
    ///             "merchantid": "PEGA",
    ///             "deskey": "8VF408JBV2B406488F2VH2X684T6R6DV",
    ///             "playerCode":"4ucmqplxb029"
    ///         }
    /// </remarks>
    [HttpPost("player/logout", Name = "PlayerLogout")]
    public ValueTask PlayerLogoutAsync(
        [FromServices] ICommandService<PlayerLogoutCommand> service,
        PlayerLogoutViewModel source)
    {
        var command = new PlayerLogoutCommand(
            ApiUrl: source.ApiUrl,
            Version: source.Version,
            MerchantId: source.Merchantid,
            Deskey: source.Deskey,
            PlayerCode: source.PlayerCode);

        return service
            .ExecuteAsync(
            command: command,
            cancellationToken: HttpContext.RequestAborted);
    }

    /// <summary>
    /// Players the logout asynchronous.
    /// </summary>
    /// <param name="service">The service.</param>
    /// <param name="source">The source.</param>
    /// <returns></returns>
    /// <remarks>
    /// <b>Example</b><br />
    ///
    ///     Post
    ///
    ///         {
    ///             "apiUrl": "https://fe.vrbetdemo.com/",
    ///             "version": "1.0",
    ///             "merchantid": "PEGA",
    ///             "deskey": "8VF408JBV2B406488F2VH2X684T6R6DV",
    ///             "playerCode":"4ucmqplxb029"
    ///         }
    /// </remarks>
    [HttpPost("player/create", Name = "PlayerCreate")]
    public ValueTask PlayerCreateAsync(
        [FromServices] ICommandService<PlayerCreateCommand> service,
        PlayerCreateViewModel source)
    {
        var command = new PlayerCreateCommand(
            ApiUrl: source.ApiUrl,
            Version: source.Version,
            MerchantId: source.Merchantid,
            Deskey: source.Deskey,
            PlayerCode: source.PlayerCode);

        return service
            .ExecuteAsync(
            command: command,
            cancellationToken: HttpContext.RequestAborted);
    }
}
