using System;
using System.Collections.Generic;

namespace SportHub.Repository.Model;

public partial class Team
{
    public long Id { get; set; }

    public string? Abbr { get; set; }

    public string? Name { get; set; }

    public string? ShortName { get; set; }

    public string? EnglishName { get; set; }

    public string? CountryCode { get; set; }

    public string? Country { get; set; }

    public string? City { get; set; }

    public int? FoundedYear { get; set; }

    public string? Stadium { get; set; }

    public string? AliasArray { get; set; }

    public string? MetaJson { get; set; }

    public long Flag { get; set; }

    public long CreatedTime { get; set; }

    public long UpdatedTime { get; set; }
}
