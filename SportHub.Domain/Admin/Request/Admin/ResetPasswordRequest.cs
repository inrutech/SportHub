using System;
using System.ComponentModel.DataAnnotations;

namespace SportHub.Domain.Admin.Request.Admin;

public class ResetPasswordRequest
{
    [Required]
    public long? AdminId { get; set; }
}
