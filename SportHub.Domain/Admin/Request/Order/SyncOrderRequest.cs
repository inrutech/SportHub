using System.ComponentModel.DataAnnotations;

namespace SportHub.Domain.Admin.Request.Order;

public class SyncOrderRequest
{
    [Required]
    public string? OrderId { get; set; }

    [Required]
    public string? MatchId { get; set; }
}
