namespace SportHub.Domain.Client.VIC.Request;

public class VicLeagueListRequest
{
    public required string Token { get; set; }

    public string? Cookie { get; set; }
}
