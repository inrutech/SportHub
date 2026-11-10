using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using SportHub.Repository.Model;

using System.ComponentModel;

using Zero.Core.Extensions;

namespace SportHub.Client;

public static class Startup
{
    public static void ConfigureServices(HostApplicationBuilder builder)
    {
        builder.Services.AddLogging();
        builder.Services.AddMemoryCache();
        builder.Services.AddHttpClient();
        builder.Services.AddHttpContextAccessor();

        builder.Services.AddZeroNetCoreAssembly();
        builder.Services.AddAssembly("SportHub.Domain");
        builder.Services.AddAssembly("SportHub.Service");

        builder.Services.AddHostedService<InitWorker>();

    }
}

public sealed class SyncOrderOptions
{
    public string? OrderId { get; set; }
}
