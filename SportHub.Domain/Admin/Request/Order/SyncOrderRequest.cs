using System.ComponentModel.DataAnnotations;

namespace SportHub.Domain.Admin.Request.Order;

public class SyncOrderRequest
{
    public string? StartTime { get; set; }

    public string? EndTime { get; set; }

    public int Settled { get; set; }

    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 100;

    public string? OrderId { get; set; }

    public string? UserId { get; set; }

    public string? BetType { get; set; }
}
