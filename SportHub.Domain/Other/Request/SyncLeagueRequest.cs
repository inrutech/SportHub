namespace SportHub.Domain.Other.Request;

public class SyncLeagueRequest
{
    public string? Token { get; set; }

    public string? Owner { get; set; }

    public string? Repo { get; set; }

    public string Branch { get; set; } = "main";

    public string LeaguePath { get; set; } = "data/leagues.json";

    public string TeamPath { get; set; } = "data/teams.json";

    public string? LeagueUrl { get; set; }

    public string? TeamUrl { get; set; }
}