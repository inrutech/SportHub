using System.ComponentModel.DataAnnotations;

namespace SportHub.Domain.Admin.Request.My;

public class SetTokenRequest
{
    [Required]
    public string? Token { get; set; }
}
