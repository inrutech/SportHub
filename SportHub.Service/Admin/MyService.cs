using Microsoft.EntityFrameworkCore;

using OtpNet;

using SportHub.Core;
using SportHub.Domain.Admin.Request.My;
using SportHub.Domain.Admin.Response.My;
using SportHub.Domain.Context;

using Zero.Core.Extensions;
using Zero.Core.Result;
using Zero.Core.Util;

namespace SportHub.Service.Admin;

public class MyService(IServiceProvider provider) : BaseService(provider)
{
    public async Task<SysResult<bool>> SetTokenAsync(SetTokenRequest request)
    {
        var token = await DB.Config.FirstOrDefaultAsync(x => x.GlobalHash == GlobalHash.LoginToken);
        if (token == null)
        {
            return Result(false, ErrorCode.USER_CUSTOM, "Token配置不存在");
        }
        token.Value = request.Token;
        token.UpdatedTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        await DB.SaveChangesAsync();
        ConfigService.CacheRemoveConfigList();
        return Result(true);
    }

    public async Task<SysResult<LoginResponse>> LoginAsync(LoginRequest request)
    {
        LoginResponse r = null;
        var admin = await DB.Admin.FirstOrDefaultAsync(x => x.LoginName == request.LoginName);
        if (admin == null)
        {
            return Result(r, ErrorCode.SYS_RESOURCE_NOT_FOUND);
        }
        var password = CryptoHelper.AES_Decrypt(admin.Password, Params.AES_KEY);
        if (!password!.Equals(request.Password, StringComparison.OrdinalIgnoreCase))
        {
            return Result(r, ErrorCode.USER_CUSTOM, "Password Mismatch");
        }

        if (admin.GoogleAuthSecret.IsNullOrEmpty())
        {
            return Result(r, ErrorCode.USER_CUSTOM, "Google Authenticator Empty");
        }

        var secretBytes = Base32Encoding.ToBytes(admin.GoogleAuthSecret);
        var totp = new Totp(secretBytes);
        var currentCode = totp.ComputeTotp();
        if (!currentCode.Equals(request.GoogleAuthKey, StringComparison.OrdinalIgnoreCase))
        {
            return Result(r, ErrorCode.USER_CUSTOM, "Google Authenticator Failure");
        }

        r = new LoginResponse
        {
            Token = TokenHelper.Serialize(new LoginTokenInfo { Id = admin.Id, Ticks = DateTimeOffset.UtcNow.Ticks },
                Params.TOKEN_KEY)
        };

        return Result(r);
    }
}
