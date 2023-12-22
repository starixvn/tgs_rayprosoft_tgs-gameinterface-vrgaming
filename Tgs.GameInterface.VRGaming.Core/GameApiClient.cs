using System.Diagnostics;
using System.Net;
using System.Text.Json;
using System.Text;
using System;
using Microsoft.Extensions.Logging;
using Tgs.GameInterface.VRGaming.Core.Extensions;
using Tgs.GameInterface.VRGaming.Core.Models;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Channels;
using System.IO;
using System.Net.Http;
using System.Web;
using System.Net.Mime;
using System.Diagnostics.Metrics;
using System.Runtime.CompilerServices;
using Tgs.GameInterface.VRGaming.Core.Exceptions;
using NATS.Client.JetStream;
using System.Reflection.Metadata;

namespace Tgs.GameInterface.VRGaming.Core;

internal class GameApiClient(
    ILogger<GameApiClient> logger,
    ISystemClock systemClock,
    IAesCryptographyUtensil cryptographyUtensil,
    HttpClient client) : IGameApiClient
{
    public async ValueTask<BalanceResponse> BalanceAsync(Uri apiUrl, string version, string merchantid, string deskey, string playerName, CancellationToken cancellationToken = default)
    {
        var parameters = new JsonObject
        {
            ["playerName"] = playerName
        };

        apiUrl = new Uri(apiUrl, "/UserWallet/Balance");

        var jsonNode = await PostAsync(
            apiUrl: apiUrl,
            logPrefix: "UserWallet.Balance",
            version: version,
            merchantid: merchantid,
            deskey: deskey,
            parameters: parameters,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var balance = jsonNode["balance"]!.GetValue<decimal>();

        return new BalanceResponse(
            IsPlayerExist: balance is not -1,
            Balance: balance);
    }

    public async ValueTask CreateUserAsync(Uri apiUrl, string version, string merchantid, string deskey, string playerName, CancellationToken cancellationToken = default)
    {
        var parameters = new JsonObject
        {
            ["playerName"] = playerName
        };

        apiUrl = new Uri(apiUrl, "/Account/CreateUser");

        var jsonNode = await PostAsync(
            apiUrl: apiUrl,
            logPrefix: "Account.CreateUser",
            version: version,
            merchantid: merchantid,
            deskey: deskey,
            parameters: parameters,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        if (jsonNode["errorCode"]!.GetValue<int>() is not 0 and not 18)
        {
            throw new ErrorCodeException(jsonNode["errorCode"]!.GetValue<int>());
        }
    }

    public async ValueTask<TransactionResponse> DepositAsync(Uri apiUrl, string version, string merchantid, string deskey, string playerName, string serialNumber, decimal amount, CancellationToken cancellationToken = default)
    {
        var parameters = new JsonObject
        {
            ["serialNumber"] = serialNumber,
            ["playerName"] = playerName,
            ["type"] = 0,
            ["amount"] = amount,
            ["createTime"] = systemClock.CurrentTime.ToDateTimeOffsettString()
        };

        apiUrl = new Uri(apiUrl, "/UserWallet/Transaction");

        var jsonNode = await PostAsync(
            apiUrl: apiUrl,
            logPrefix: "UserWallet.Transaction.Deposit",
            version: version,
            merchantid: merchantid,
            deskey: deskey,
            parameters: parameters,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return jsonNode["state"]!.GetValue<int>() is not 0
            ? throw new ErrorCodeException(jsonNode["state"]!.GetValue<int>())
            : new TransactionResponse(
            jsonNode["serialNumber"]!.ToString(),
            jsonNode.ToJsonString());
    }

    public async ValueTask KickUserAsync(Uri apiUrl, string version, string merchantid, string deskey, string playerName, CancellationToken cancellationToken = default)
    {
        var parameters = new JsonObject
        {
            ["playerName"] = playerName
        };

        apiUrl = new Uri(apiUrl, "/Account/KickUser");

        var jsonNode = await PostAsync(
            apiUrl: apiUrl,
            logPrefix: "Account.KickUser",
            version: version,
            merchantid: merchantid,
            deskey: deskey,
            parameters: parameters,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        if (jsonNode["errorCode"]!.GetValue<int>() is not 0 and not 8)
        {
            throw new ErrorCodeException(jsonNode["errorCode"]!.GetValue<int>());
        }
    }

    public async ValueTask<string> LoginValidateAsync(Uri apiUrl, string version, string merchantid, string deskey, string playerName, int channelId = 0, CancellationToken cancellationToken = default)
    {
        var parameters = new JsonObject
        {
            ["playerName"] = playerName,
            ["loginTime"] = systemClock.CurrentTime.ToDateTimeOffsettString(),
            ["channelId"] = channelId.ToString("D"),
            ["isCreateUser"] = "true",
            ["channelType"] = 0
        };
        var path = string.Join("&", values: JsonSerializer.Deserialize<Dictionary<string, object>>(parameters.ToJsonString())!.Where(predicate: predicate => predicate.Value is not null).Select(selector: selector => $"{selector.Key}={selector.Value}"));

        var dataEncrypt = await cryptographyUtensil.EncryptionAsync(
            data: path,
            secretKey: deskey,
            cancellationToken: cancellationToken)
        .ConfigureAwait(false);

        //HttpUtility產生網址會進行URL ENCODE動作
        var gameUrl = new Uri(apiUrl, $"Account/LoginValidate");
        var builder = new UriBuilder(gameUrl);
        var query = HttpUtility.ParseQueryString(builder.Query);
        query["version"] = version;
        query["id"] = merchantid;
        query["data"] = dataEncrypt;
        builder.Query = query.ToString();

        var logPrefix = $"{nameof(GameApiClient)}.Account/LoginValidate";

        var apiLogEvent = new JsonObject
        {
            ["Path"] = builder.ToString(),
            ["Prefix"] = logPrefix,
            ["ServerName"] = Environment.MachineName.NoLongerThan(128),
            ["RequestData"] = parameters.ToJsonString(),
            ["RequestContent"] = dataEncrypt,
            ["RecordedOn"] = DateTimeOffset.UtcNow
        };

        logger.LogInformation("{apiLogEvent}", apiLogEvent.ToJsonString());

        return builder.ToString();
    }

    public async IAsyncEnumerable<MerchantQueryResponse> MerchantQueryBetAsync(Uri apiUrl, string version, string merchantid, string deskey, DateTimeOffset startTime, DateTimeOffset endTime, int recordPage = 0, int recordCountPerPage = 1000, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var parameters = new JsonObject
        {
            ["startTime"] = startTime.ToDateTimeOffsettString(),
            ["endTime"] = endTime.ToDateTimeOffsettString(),
            ["channelId"] = -1,
            ["isUpdateTime"] = true,
            ["state"] = -1,
            ["betNumberFormat"] = 0,
            ["recordPage"] = recordPage,
            ["recordCountPerPage"] = recordCountPerPage,
        };

        apiUrl = new Uri(apiUrl, "/MerchantQuery/Bet");

        var jsonNode = await PostAsync(
            apiUrl: apiUrl,
            logPrefix: "MerchantQuery.Bet",
            version: version,
            merchantid: merchantid,
            deskey: deskey,
            parameters: parameters,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var betRecords = jsonNode["betRecords"]!.AsArray()!.ToArray()!
            ?? Enumerable.Empty<JsonNode>();

        if (!betRecords.Any())
        {
            yield break;
        }

        foreach (var betRecord in betRecords)
        {
            if (betRecord is null)
            {
                continue;
            }

            yield return new MerchantQueryResponse(
                IssueNumber: betRecord["issueNumber"]!.ToString(),
                SerialNumber: betRecord["serialNumber"]!.ToString(),
                ChannelId: betRecord["channelId"]!.ToString(),
                PlayerName: betRecord["playerName"]!.ToString(),
                State: betRecord["state"]!.GetValue<int>(),
                SubState: betRecord["subState"]!.GetValue<int>(),
                Cost: betRecord["cost"]!.GetValue<decimal>(),
                PlayerPrize: betRecord["playerPrize"]!.GetValue<decimal>(),
                LossPrize: betRecord["lossPrize"]!.GetValue<decimal>(),
                CreateTime: betRecord["createTime"]!.GetValue<DateTimeOffset>(),
                UpdateTime: betRecord["updateTime"]!.GetValue<DateTimeOffset>(),
                AdditionalData: betRecord.ToJsonString());
        }
    }

    public async ValueTask<bool> TransactionRecordAsync(Uri apiUrl, string version, string merchantid, string deskey, string playerName, string serialNumber, DateTimeOffset startTime, DateTimeOffset endTime, CancellationToken cancellationToken = default)
    {
        var parameters = new JsonObject
        {
            ["startTime"] = startTime.ToDateTimeOffsettString(),
            ["endTime"] = endTime.ToDateTimeOffsettString(),
            ["serialNumber"] = serialNumber,
            ["playerName"] = playerName,
            ["recordPage"] = 0,
            ["recordCountPerPage"] = 10,
        };

        apiUrl = new Uri(apiUrl, "/UserWallet/TransactionRecord");

        var jsonNode = await PostAsync(
            apiUrl: apiUrl,
            logPrefix: "UserWallet.TransactionRecord",
            version: version,
            merchantid: merchantid,
            deskey: deskey,
            parameters: parameters,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var records = jsonNode["records"]!.AsArray()!.ToArray()!
            ?? Enumerable.Empty<JsonNode>();

        var record = records.Where(predicate: predicate => predicate["serialNumber"]!.ToString() == serialNumber).FirstOrDefault();

        return record is not null && (record["state"]!.GetValue<int>() is not 0
            ? throw new ErrorCodeException(record["state"]!.GetValue<int>())
            : true);
    }

    public async ValueTask<TransactionResponse> WithdrawAsync(Uri apiUrl, string version, string merchantid, string deskey, string playerName, string serialNumber, decimal amount, CancellationToken cancellationToken = default)
    {
        var parameters = new JsonObject
        {
            ["serialNumber"] = serialNumber,
            ["playerName"] = playerName,
            ["type"] = 1,
            ["amount"] = amount,
            ["createTime"] = systemClock.CurrentTime.ToDateTimeOffsettString()
        };

        apiUrl = new Uri(apiUrl, "/UserWallet/Transaction");

        var jsonNode = await PostAsync(
            apiUrl: apiUrl,
            logPrefix: "UserWallet.Transaction.Withdraw",
            version: version,
            merchantid: merchantid,
            deskey: deskey,
            parameters: parameters,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return jsonNode["state"]!.GetValue<int>() is not 0
            ? throw new ErrorCodeException(jsonNode["state"]!.GetValue<int>())
            : new TransactionResponse(
            jsonNode["serialNumber"]!.ToString(),
            jsonNode.ToJsonString());
    }

    private async ValueTask<JsonNode> PostAsync(Uri apiUrl, string logPrefix, string version, string merchantid, string deskey, JsonNode parameters, CancellationToken cancellationToken = default)
    {
        logPrefix = $"{nameof(GameApiClient)}.{logPrefix}";

        var dataEncrypt = await cryptographyUtensil.EncryptionAsync(
            data: parameters.ToJsonString(),
            secretKey: deskey,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var data = new JsonObject
        {
            ["version"] = version,
            ["id"] = merchantid,
            ["data"] = dataEncrypt,
        };

        var sw = Stopwatch.StartNew();

        var content = new StringContent(data.ToJsonString(), Encoding.UTF8, MediaTypeNames.Application.Json);

        var httpResponseMessage = await client.PostAsync(
            requestUri: apiUrl,
            content: content,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        sw.Stop();

        var responseMessage = default(string);
        using (var sr = new StreamReader(httpResponseMessage.Content.ReadAsStreamAsync(cancellationToken).Result))
        {
            responseMessage = await sr.ReadToEndAsync(
                cancellationToken: cancellationToken)
                .ConfigureAwait(false)
                ?? string.Empty;
        }

        var contentDecrypt = string.Empty;
        try
        {
            contentDecrypt = await cryptographyUtensil.DecryptAsync(
                data: responseMessage,
                secretKey: deskey,
                isDecodeUrl: false,
                cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }
        catch
        {

            contentDecrypt = string.Empty;
        }

        var apiLogEvent = new JsonObject
        {
            ["Path"] = httpResponseMessage.RequestMessage?.RequestUri?.AbsoluteUri.NoLongerThan(512) ?? string.Empty,
            ["Method"] = httpResponseMessage.RequestMessage?.Method.ToString().NoLongerThan(8) ?? string.Empty,
            ["Prefix"] = logPrefix,
            ["ServerName"] = Environment.MachineName.NoLongerThan(128),
            ["RequestHeaders"] = httpResponseMessage.RequestMessage?.Headers + httpResponseMessage.RequestMessage?.Content?.Headers?.ToString(),
            ["RequestData"] = parameters.ToJsonString(),
            ["RequestContent"] = (await content.ReadAsStringAsync(cancellationToken: cancellationToken).ConfigureAwait(false)).NoLongerThan(4096),
            ["ResponseHeaders"] = httpResponseMessage.Headers.ToString(),
            ["ResponseContent"] = responseMessage.NoLongerThan(4096),
            ["ResponseDecrypt"] = contentDecrypt.NoLongerThan(4096),
            ["ResponseCode"] = (short)httpResponseMessage.StatusCode,
            ["ExecutionTimeMs"] = (int)sw.ElapsedMilliseconds,
            ["RecordedOn"] = DateTimeOffset.UtcNow
        };

        logger.LogInformation("{apiLogEvent}", apiLogEvent.ToJsonString());

        _ = httpResponseMessage.EnsureSuccessStatusCode();

        return string.IsNullOrEmpty(contentDecrypt)
            ? throw new Exception("Game provider response content is empty.")
            : JsonNode.Parse(contentDecrypt)!;
    }
}
