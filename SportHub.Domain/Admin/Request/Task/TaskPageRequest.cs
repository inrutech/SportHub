using System.ComponentModel.DataAnnotations;

namespace SportHub.Domain.Admin.Request.Task;

public class TaskPageRequest
{
    [Required]
    public string? OrderId { get; set; }

}