using Zero.Core.Web.Request;

namespace SportHub.Domain.Admin.Request.Order;

public class OrderPageRequest : PageRequest
{
    public long? Id { get; set; }

    public string? SourceNo { get; set; }
}
