using System.Collections.Generic;

using Microsoft.Playwright;

namespace SportHub.Domain.Context;

public class ClientContext
{
    public static IPlaywright? Playwright { get; set; }

    public static IBrowser? EdgeBrowser { get; set; }

    public static Dictionary<long, object> TaskContextArray { get; set; } = [];
}
