using System.ComponentModel.DataAnnotations;

namespace SportHub.Domain.Admin.Request.Task;

public class SetRequest
{
    [Required]
    public long? OrderId { get; set; }

    [Required]
    public List<long>? OrderIdList { get; set; }

    [Required]
    public SourceCode? TargetSourceCode { get; set; }

    [Required]
    public decimal? Ratio { get; set; }

}