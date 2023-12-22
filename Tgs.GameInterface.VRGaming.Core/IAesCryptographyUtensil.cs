namespace Tgs.GameInterface.VRGaming.Core;

public interface IAesCryptographyUtensil
{
    ValueTask<string> DecryptAsync(string data, string secretKey, bool isDecodeUrl = true, CancellationToken cancellationToken = default);

    ValueTask<string> EncryptionAsync(string data, string secretKey, CancellationToken cancellationToken = default);
}
