using System.ComponentModel.DataAnnotations;

namespace Tgs.GameInterface.VRGaming.ViewModels;

public record FundStatusViewModel
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
    [Required]
    public string OrderId { get; init; } = default!;
    [Required]
    [DataType(DataType.DateTime)]
    public DateTimeOffset StartTime { get; init; } = default!;
    [Required]
    [DataType(DataType.DateTime)]
    public DateTimeOffset EndTime { get; init; } = default!;
}
