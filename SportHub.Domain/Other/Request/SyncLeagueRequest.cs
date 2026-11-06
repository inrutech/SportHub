using System.ComponentModel.DataAnnotations;

namespace SportHub.Domain.Other.Request;

public class SyncLeagueRequest
{
    [Required]
    public string? Token { get; set; }
}