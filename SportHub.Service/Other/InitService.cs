using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using OtpNet;

using SportHub.Core;
using SportHub.Core.Code;
using SportHub.Domain.Client.IM;
using SportHub.Domain.Client.IM.Response;
using SportHub.Domain.Other.Request;
using SportHub.Repository.Model;

using Zero.Core.Result;
using Zero.Core.Util;

namespace SportHub.Service.Other;

public class InitService(IServiceProvider provider) : BaseService(provider)
{
    public async Task<SysResult<bool>> InitConfigAsync(bool isReset = false)
    {
        //if (HostEnvironment.IsProduction())
        //{
        //    return Result(false);
        //}
        if (!isReset)
        {
            var exists = await DB.Config.AnyAsync();
            if (exists)
            {
                return Result(false, ErrorCode.USER_CUSTOM, "已初始化");
            }
        }
        else
        {
            await DB.Config.ExecuteDeleteAsync();
            await DB.SaveChangesAsync();
        }

        Dictionary<ConfigCode, List<(long hash, DataCode dateCode, string value)>> array = new()
        {
            [ConfigCode.Base] =
            [
                (GlobalHash.LoginToken, DataCode.String, "24e26089-0397-436a-a8c3-593f9d46be2a"),
                (GlobalHash.IMApiKey, DataCode.String, "qfikVgt8xiRiLFD4hOkx")
            ]
        };

        var configList = new List<Config>();
        foreach (var (configCode, list) in array)
        {
            var index = 1;
            foreach (var (hash, dataCode, value) in list)
            {
                var config = new Config
                {
                    Id = Snowflake.NewID(),
                    ConfigCode = (long)configCode,
                    DataCode = (long)dataCode,
                    GlobalHash = hash,
                    Value = value,
                    Sort = index++,
                    Flag = 0,
                    CreatedTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                    UpdatedTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                };

                configList.Add(config);
            }
        }

        await DB.Config.AddRangeAsync(configList);
        await DB.SaveChangesAsync();

        return Result(true);
    }

    public async Task<SysResult<bool>> InitAdminAsync()
    {
        var exists = await DB.Admin.AnyAsync();
        if (exists)
        {
            return Result(false, ErrorCode.USER_CUSTOM, "已初始化");
        }
        var secretKey = KeyGeneration.GenerateRandomKey(20);
        var base32Secret = Base32Encoding.ToString(secretKey);
        var admin = new Repository.Model.Admin
        {
            Id = Snowflake.NewID(),
            Alias = "默认管理员",
            CreatedTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            Flag = 0,
            LoginName = "admin",
            Password = CryptoHelper.AES_Encrypt(CryptoHelper.MD5_Encrypt("000000"), Params.AES_KEY),
            GoogleAuthSecret = base32Secret,
            UpdatedTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
        };
        await DB.Admin.AddAsync(admin);
        await DB.SaveChangesAsync();

        return Result(true);
    }
}