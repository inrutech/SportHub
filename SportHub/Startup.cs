using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.OpenApi;

using NLog.Web;

using Scalar.AspNetCore;

using SportHub.Repository.Model;

using System;

using Zero.Core.Extensions;
using Zero.Core.Util;

namespace SportHub;

/// <summary>
/// StartUp
/// </summary>
public class Startup
{
    private static readonly Dictionary<string, string> ApiArray = new()
    {
        ["remote"] = "远程 API",
        ["admin"] = "后台 API",
        ["client"] = "终端 API",
    };

    private static void ConfigureBaseServices(WebApplicationBuilder builder)
    {
        builder.Host.UseNLog();

        builder.Services
            .AddResponseCaching()
            .AddMemoryCache()
            .AddHttpClient()
            .AddHttpContextAccessor()
            .AddSingleton<FileExtensionContentTypeProvider>();

        builder.Services.Configure<ApiBehaviorOptions>(options =>
            options.SuppressModelStateInvalidFilter = true);

        builder.Services.Configure<FormOptions>(options =>
        {
            options.ValueLengthLimit = int.MaxValue;
            options.MultipartBodyLengthLimit = int.MaxValue;
        });

        builder.Services.AddCors(option => option.AddPolicy("cors", policy => policy
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
            .SetIsOriginAllowed(_ => true)));
    }

    private static void ConfigureDataAccess(WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<DBContext>(options =>
        {
            options.UseNpgsql(builder.Configuration.GetConnectionString("db"),
                sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(30), null);
                    sqlOptions.CommandTimeout(30);
                });
        });

    }

    private static void ConfigureApiServices(WebApplicationBuilder builder)
    {
        StartupHelper.AddApiFilter(builder);
        StartupHelper.AddSwaggerGen(builder, ApiArray);
    }

    private static void RegisterApplicationModules(WebApplicationBuilder builder)
    {
        builder.Services.AddZeroNetCoreAssembly();

        builder.Services.AddAssembly("SportHub.Domain");

        builder.Services.AddAssembly("SportHub.Service");

        // #if !DEBUG
        //         #region Credit
        //         builder.Services.AddHostedService<CreditMatchWorker>();
        //         builder.Services.AddHostedService<CreditAwardWorker>();
        //         builder.Services.AddHostedService<CreditCallbackWorker>();
        //         builder.Services.AddHostedService<CreditReleaseWorker>();
        //         builder.Services.AddHostedService<FreezeExpireWorker>();
        //         // builder.Services.AddHostedService<CreditReservedWorker>();
        //         #endregion

        //         #region Debit
        //         builder.Services.AddHostedService<DebitMatchWorker>();
        //         builder.Services.AddHostedService<DebitAwardWorker>();
        //         builder.Services.AddHostedService<DebitCallbackWorker>();
        //         builder.Services.AddHostedService<DebitExpireWorker>();
        //         builder.Services.AddHostedService<LockExpireWorker>();
        //         builder.Services.AddHostedService<DebitWarnWorker>();
        //         #endregion

        //         #region Admin
        //         builder.Services.AddHostedService<AdminLogWorker>();
        //         #endregion

        //         #region Merchant
        //         builder.Services.AddHostedService<MerchantLogWorker>();
        //         builder.Services.AddHostedService<NoteWorker>();
        //         builder.Services.AddHostedService<FloatingWorker>();
        //         builder.Services.AddHostedService<MerchantReportWorker>();
        //         #endregion

        //         #region User
        //         builder.Services.AddHostedService<AccountWorker>();
        //         builder.Services.AddHostedService<UserLogWorker>();
        //         builder.Services.AddHostedService<TreeWorker>();
        //         builder.Services.AddHostedService<PaymentWorker>();
        //         builder.Services.AddHostedService<PaymentDetailWorker>();
        //         builder.Services.AddHostedService<DepositExpireWorker>();
        //         builder.Services.AddHostedService<PushWorker>();
        //         builder.Services.AddHostedService<UserReportWorker>();
        //         builder.Services.AddHostedService<ActivityWorker>();
        //         builder.Services.AddHostedService<UserFundsWorker>();
        //         builder.Services.AddHostedService<UserStatWorker>();
        //         #endregion

        //         #region Chain
        //         builder.Services.AddHostedService<BscWorker>();
        //         builder.Services.AddHostedService<TronWorker>();
        //         #endregion
        // #endif
    }

    /// <summary>
    ///     注册服务
    /// </summary>
    /// <param name="builder"></param>
    public static void ConfigureServices(WebApplicationBuilder builder)
    {
        // 基础服务配置
        ConfigureBaseServices(builder);

        // 数据访问层配置
        ConfigureDataAccess(builder);

        // API相关配置
        ConfigureApiServices(builder);

        // 应用模块注册
        RegisterApplicationModules(builder);
    }

    /// <summary>
    /// </summary>
    /// <param name="app"></param>
    public static void ConfigureApplication(WebApplication app)
    {
        var supportedCultures = new[] { "en", "zh-hant" };

        app.UseRequestLocalization(cultureOptions =>
        {
            cultureOptions.AddSupportedCultures(supportedCultures)
                .AddSupportedUICultures(supportedCultures)
                .SetDefaultCulture(supportedCultures[0]);
            cultureOptions.FallBackToParentCultures = true;
        });

        app.UseForwardedHeaders(new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.All
        });

        app.UseCors("cors");
        app.UseResponseCaching();

        app.UseExceptionHandler(StartupHelper.HandlerException);
        app.UseStatusCodePages(StartupHelper.HandlerStatusCode);

        if (app.Environment.IsStaging() || app.Environment.IsDevelopment())
        {
            app.UseSwagger(c =>
            {
                c.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
                {
                    // 优先取 CF-Visitor 头，否则用 httpReq.Scheme
                    var scheme = httpReq.Scheme;
                    if (httpReq.Headers.TryGetValue("CF-Visitor", out var proto))
                    {
                        var array = proto.ToString().DeserializeJson<Dictionary<string, string>>();
                        scheme = array.GetValueOrDefault("scheme");
                    }
                    var baseUrl = $"{scheme}://{httpReq.Host}";
                    swaggerDoc.Servers =
                    [
                        new OpenApiServer { Url = baseUrl, Description = "当前服务器" }
                    ];
                });
            });

            app.MapScalarApiReference(options =>
            {
                options.AddDocuments(ApiArray.Select(x =>
                    new ScalarDocument(x.Key, x.Value, $"{x.Key}/api.json")
                ));
            });
        }

        app.UseDefaultFiles(new DefaultFilesOptions
        {
            DefaultFileNames = { "index.html" }
        });

        app.UseStaticFiles();
        app.UseRouting();
        app.MapDefaultControllerRoute();
    }


}