namespace SportHub.Repository.Model;

using System.ComponentModel.DataAnnotations;

public partial class AliasInfo
{
    public long SourceCode { get; set; }

    public string? Alias { get; set; }
}