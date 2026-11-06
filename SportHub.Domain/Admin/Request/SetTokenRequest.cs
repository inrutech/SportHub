using System.ComponentModel.DataAnnotations;

namespace SportHub.Domain.Admin.Request;

public class SetTokenRequest
{
    [Required]
    public string? Token { get; set; }
}
