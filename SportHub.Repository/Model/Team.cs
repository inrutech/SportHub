using System;
using System.Collections.Generic;

namespace SportHub.Repository.Model;

public partial class Team
{
    public long Id { get; set; }

    public string? Name { get; set; }

    public string? Country { get; set; }

    public string? AliasArray { get; set; }

    public long Flag { get; set; }

    public long CreatedTime { get; set; }

    public long UpdatedTime { get; set; }
}
