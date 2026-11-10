using System.ComponentModel.DataAnnotations;

namespace SportHub.Domain.Admin.Request.Order;

public class OrderListRequest
{
    // [Required]
    public string? OrderId { get; set; }

}
