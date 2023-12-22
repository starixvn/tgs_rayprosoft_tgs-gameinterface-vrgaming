using System.ComponentModel.DataAnnotations;

namespace Tgs.GameInterface.VRGaming.ViewModels;

public record PlayerLogoutViewModel
{
    [Required]
    [DataType(DataType.Url)]
    public Uri ApiUrl { get; init; } = default!;
    [Required]
    public string Version { get; init; } = default!;
    [Required]
    public string Merchantid { get; init; } = default!;
    [Required]
    public string Deskey { get; init; } = default!;
    [Required]
    public string PlayerCode { get; init; } = default!;
}
