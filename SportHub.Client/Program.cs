
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using SportHub.Client;

using Zero.Core.Extensions;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

Startup.ConfigureServices(builder);

using var host = builder.Build();
var logger = host.Services.GetRequiredService<ILogger<Program>>();

logger.LogCustom("Start.", "Program");

Console.CancelKeyPress += (_, _) => logger.LogCustom("End.", "Program");

await host.RunAsync();
