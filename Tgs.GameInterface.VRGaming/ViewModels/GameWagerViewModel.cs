using System.ComponentModel.DataAnnotations;

namespace Tgs.GameInterface.VRGaming.ViewModels;

public record GameWagerViewModel
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
    [DataType(DataType.DateTime)]
    public DateTimeOffset StartTime { get; init; }
    [Required]
    [DataType(DataType.DateTime)]
    public DateTimeOffset EndTime { get; init; }
    [Required]
    [Range(0, int.MaxValue)]
    public int PageNumber { get; init; } = 1;
    [Required]
    [Range(0, 1000)]
    public int PageSize { get; init; } = 1000;
}
