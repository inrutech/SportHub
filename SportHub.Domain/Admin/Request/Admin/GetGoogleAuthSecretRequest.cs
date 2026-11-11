using System.ComponentModel.DataAnnotations;

namespace SportHub.Domain.Admin.Request.Admin;

public class GetGoogleAuthSecretRequest
{

    [Required]
    public long? AdminId { get; set; }
}
