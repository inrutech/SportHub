namespace SportHub.Domain.Other.Request;

using System.ComponentModel.DataAnnotations;

public class GetOrderDetailRequest
{
    [Required]
    public string? Token { get; set; }

    [Required]
    public string? OrderId { get; set; }

    [Required]
    public string? MatchIdList { get; set; }
}