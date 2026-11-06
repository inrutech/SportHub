using SportHub.Core;
using SportHub.Core.Code;
using SportHub.Domain.Model;
using SportHub.Repository.Model;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using System.Net;

using Zero.Core.Extensions;
using Zero.Core.Result;
using Zero.Core.Util;
using SportHub.Domain.Context;

namespace SportHub.Service.Common
{
    /// <summary>
    /// 管理员服务
    /// </summary>
    public class AdminService(IServiceProvider provider) : BaseService(provider)
    {
        /// <summary>
        /// 校验管理员权限
        /// </summary>
        /// <returns>校验结果</returns>
        public async Task<SysResult<bool>> ValidAuthAsync()
        {
            if (!WebClient.Request.Headers.TryGetValue("Authorization", out var auth) ||
                auth.ToString().IsNullOrEmpty())
            {
                return Result(false, ErrorCode.USER_CUSTOM, "Authorization Failure");
            }

            string bearer = auth.ToString().Replace("Bearer", string.Empty).Trim();

            if (bearer.IsNullOrEmpty())
            {
                return Result(false, ErrorCode.USER_CUSTOM, "Authorization Empty");
            }

            string? key = WebCache.RenderKey(Params.APP_KEY, "Admin", "Token", bearer);

            var admin = await WebCache.Get(key, async () =>
            {
                var token = TokenHelper.Deserialize<LoginTokenInfo>(bearer, Params.TOKEN_KEY);

                if (token == null || token.Id == 0)
                {
                    return default;
                }

                return await DB.Admin.FindAsync(token.Id);
            }, 1);


            if (admin == null)
            {
                return Result(false, ErrorCode.USER_CUSTOM, "Authorization Failure");
            }

            var ip = WebClient.IP;
            if (WebClient.Request.Headers.TryGetValue("CF-Connecting-IP", out var cfConnectingIp))
            {
                ip = cfConnectingIp;
            }
            if (admin.HostWhiteList?.Count > 0 && !admin.HostWhiteList.Contains(IPAddress.Parse(ip)))
            {
                return Result(false, ErrorCode.USER_CUSTOM, "IP not allowed");
            }

            AdminContext.LoginInfo = admin;
            return Result(true);
        }

        /// <summary>
        /// 获取管理员列表（带缓存）
        /// </summary>
        /// <returns>管理员列表</returns>
        public Task<List<Repository.Model.Admin>> CacheGetAdminListAsync()
        {
            var key = WebCache.RenderKey(Params.APP_KEY, "AdminList");
            return WebCache.Get(key, async () => { return await DB.Admin.AsNoTracking().ToListAsync(); });
        }

        /// <summary>
        /// 移除管理员列表缓存
        /// </summary>
        public void CacheRemoveAdminList()
        {
            var key = WebCache.RenderKey(Params.APP_KEY, "AdminList");
            WebCache.Remove(key);
        }

        /// <summary>
        /// 完整映射 Admin 实体的所有属性到 AdminInfo
        /// </summary>
        /// <param name="info">Admin 实体</param>
        /// <returns>AdminInfo DTO</returns>
        public AdminInfo? RenderAdminInfo(Repository.Model.Admin? info)
        {
            if (info == null) return null;
            return new AdminInfo
            {
                Id = info.Id,
                Alias = info.Alias,
                LoginName = info.LoginName,
                HostWhiteList = info.HostWhiteList?.Select(x => x.ToString()).ToList(),
                CreatedTime = info.CreatedTime,
                UpdatedTime = info.UpdatedTime,
                Disabled = (info.Flag & (long)CommonCode.Disabled) > 0,
            };
        }

    }
}
