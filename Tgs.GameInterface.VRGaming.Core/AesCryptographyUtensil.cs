using System.Security.Cryptography;
using System.Text;

namespace Tgs.GameInterface.VRGaming.Core;

internal class AesCryptographyUtensil : IAesCryptographyUtensil
{
    public async ValueTask<string> DecryptAsync(string data, string secretKey, bool isDecodeUrl = true, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(data))
        {
            throw new NullReferenceException("data");
        }

        var dataBytes = isDecodeUrl
            ? Convert.FromBase64String(System.Web.HttpUtility.UrlDecode(data))
            : Convert.FromBase64String(data);

        using var aes = Aes.Create();
        aes.Mode = CipherMode.ECB;
        aes.Padding = PaddingMode.PKCS7;
        aes.Key = Encoding.UTF8.GetBytes(secretKey);
        aes.GenerateIV();
        using ICryptoTransform cryptoTransform = aes.CreateDecryptor();
        using MemoryStream memoryStream = new MemoryStream(dataBytes);
        using CryptoStream csDecrypt = new CryptoStream(memoryStream, cryptoTransform, CryptoStreamMode.Read);
        using StreamReader streamReader = new StreamReader(csDecrypt);
        return await streamReader.ReadToEndAsync(
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }

    public async ValueTask<string> EncryptionAsync(string data, string secretKey, CancellationToken cancellationToken = default)

    {
        if (string.IsNullOrEmpty(data))
        {
            throw new NullReferenceException("data");
        }

        var dataBytes = Encoding.UTF8.GetBytes(data);

        using var aes = Aes.Create();
        aes.Mode = CipherMode.ECB;
        aes.Padding = PaddingMode.PKCS7;
        aes.KeySize = 128;
        aes.BlockSize = 128;
        aes.Key = Encoding.UTF8.GetBytes(secretKey);
        aes.GenerateIV();
        using var cryptoTransform = aes.CreateEncryptor();
        using MemoryStream memoryStream = new MemoryStream();
        using CryptoStream cryptoStream = new CryptoStream(memoryStream, cryptoTransform, CryptoStreamMode.Write);
        await cryptoStream.WriteAsync(
            buffer: dataBytes,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        await cryptoStream.FlushFinalBlockAsync(
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        byte[] encrypted = memoryStream.ToArray();
        return Convert.ToBase64String(encrypted, 0, encrypted.Length);
    }
}
