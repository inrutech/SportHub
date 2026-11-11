using Microsoft.EntityFrameworkCore;

using OtpNet;

using SportHub.Core;
using SportHub.Domain.Admin.Request.Admin;
using SportHub.Domain.Admin.Response.Admin;

using System;
using System.Net;

using Zero.Core.Extensions;
using Zero.Core.Result;
using Zero.Core.Util;
using Zero.Core.Web;
using Zero.Core.Web.Request;


namespace SportHub.Service.Admin;

public class AdminService(IServiceProvider provider) : BaseService(provider)
{
    public async Task<SysResult<PageList<AdminPageResponse>>> PageAsync(AdminPageRequest request)
    {
        PageList<AdminPageResponse>? r = null;
        var query = DB.Admin.AsNoTracking();

        if (request.Id > 0)
        {
            query = query.Where(x => x.Id == request.Id);
        }
        if (!string.IsNullOrEmpty(request.Alias))
        {
            query = query.Where(x => x.Alias.Equals(request.Alias));
        }
        if (!string.IsNullOrEmpty(request.LoginName))
        {
            query = query.Where(x => x.LoginName.Equals(request.LoginName));
        }

        var list = await query.OrderByDescending(x => x.CreatedTime).Skip(request.PageSkip).Take(request.PageSize).ToListAsync();
        var count = await query.CountAsync();


        r = new PageList<AdminPageResponse>(list.Select(x => new AdminPageResponse
        {
            AdminInfo = AdminService.RenderAdminInfo(x),
        }).ToList(), request, count);
        return Result(r);
    }


    public async Task<SysResult<GetGoogleAuthSecretResponse>> GetGoogleAuthSecretAsync(GetGoogleAuthSecretRequest request)
    {
        var admin = await DB.Admin.FindAsync(request.AdminId);
        if (admin == null)
        {
            return Result(new GetGoogleAuthSecretResponse(), ErrorCode.SYS_RESOURCE_NOT_FOUND);
        }

        var secretKey = KeyGeneration.GenerateRandomKey(20);
        var base32Secret = Base32Encoding.ToString(secretKey);
        var r = new GetGoogleAuthSecretResponse
        {
            NewGoogleAuthSecret = base32Secret,
            NewGoogleAuthLink = $"otpauth://totp/UPIBee:{admin.LoginName}?secret={base32Secret}&issuer={Params.APP_KEY}"
        };

        return Result(r);
    }


    public async Task<SysResult<bool>> ResetPasswordAsync(ResetPasswordRequest request)
    {
        var admin = await DB.Admin.FindAsync(request.AdminId);
        if (admin == null)
        {
            return Result(false, ErrorCode.SYS_RESOURCE_NOT_FOUND);
        }

        var password = CryptoHelper.AES_Encrypt(CryptoHelper.MD5_Encrypt("123456"), Params.AES_KEY);
        admin.Password = password;
        admin.UpdatedTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        await DB.SaveChangesAsync();
        return Result(true);
    }

    public async Task<SysResult<bool>> SetGoogleAuthSecretAsync(SetGoogleAuthSecretRequest request)
    {
        var admin = await DB.Admin.FindAsync(request.AdminId);
        if (admin == null)
        {
            return Result(false, ErrorCode.SYS_RESOURCE_NOT_FOUND);
        }
        var secretBytes = Base32Encoding.ToBytes(request.NewGoogleAuthSecret);
        var totp = new Totp(secretBytes);
        var currentCode = totp.ComputeTotp();

        if (!currentCode.Equals(request.NewGoogleAuthKey, StringComparison.OrdinalIgnoreCase))
        {
            return Result(false, ErrorCode.USER_CUSTOM, "Google Authenticator Failure");
        }

        admin.GoogleAuthSecret = request.NewGoogleAuthSecret;
        admin.UpdatedTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        await DB.SaveChangesAsync();

        return Result(true);
    }

    public async Task<SysResult<bool>> SetAsync(SetRequest request)
    {
        Repository.Model.Admin? admin = null;
        if (request.AdminId > 0)
        {
            admin = await DB.Admin.FindAsync(request.AdminId);
            if (admin == null)
            {
                return Result(false, ErrorCode.SYS_RESOURCE_NOT_FOUND);
            }
        }
        else
        {

            var exists = await DB.Admin.AnyAsync(x => x.LoginName == request.LoginName);
            if (exists)
            {
                return Result(false, ErrorCode.USER_CUSTOM, "Login name already exists");
            }
            admin = new Repository.Model.Admin
            {
                CreatedTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                Password = CryptoHelper.AES_Encrypt(CryptoHelper.MD5_Encrypt("123456"), Params.AES_KEY),
                Flag = 0,
                Id = Snowflake.NewID(),
            };
            DB.Admin.Add(admin);
        }

        admin.Alias = request.Alias;
        admin.LoginName = request.LoginName;

        admin.UpdatedTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        await DB.SaveChangesAsync();
        return Result(true);
    }

}
