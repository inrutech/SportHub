using SportHub.Core;
using SportHub.Core.Code;
using SportHub.Domain.Model;
using SportHub.Repository.Model;

using Microsoft.EntityFrameworkCore;

using Zero.Core.Extensions;

namespace SportHub.Service.Common
{
    public class ConfigService(IServiceProvider provider) : BaseService(provider)
    {
        // 获取配置列表（带缓存）
        public Task<List<Config>> CacheGetConfigListAsync()
        {
            var key = WebCache.RenderKey(Params.APP_KEY, "ConfigList");
            return WebCache.Get(key, async () => { return await DB.Config.AsNoTracking().ToListAsync(); });
        }

        // 移除配置列表缓存
        public void CacheRemoveConfigList()
        {
            var key = WebCache.RenderKey(Params.APP_KEY, "ConfigList");
            WebCache.Remove(key);
        }

        // 渲染ConfigInfo
        public ConfigInfo? RenderConfigInfo(Config? info)
        {
            if (info == null) return null;
            return new ConfigInfo
            {
                Id = info.Id,
                ConfigCode = (info.ConfigCode ?? 0).ToEnum(ConfigCode.None),
                DataCode = (info.DataCode ?? 0).ToEnum(DataCode.None),
                GlabalHash = info.GlobalHash,
                Sort = info.Sort,
                Value = info.Value,
                CreatedTime = info.CreatedTime,
                UpdatedTime = info.UpdatedTime
            };
        }

        public async Task<Dictionary<long, string>> GetConfigAsync(ConfigCode configCode)
        {
            var configList = await CacheGetConfigListAsync();
            if (configList == null || configList.Count == 0)
            {
                return [];
            }

            return configList.Where(x => x.ConfigCode == (long)configCode).ToDictionary(x => x.GlobalHash ?? 0, x => x.Value!);
        }

        public async Task<Dictionary<long, bool>> GetBooleanConfigAsync(ConfigCode configCode)
        {
            var configList = await CacheGetConfigListAsync();
            if (configList == null || configList.Count == 0)
            {
                return [];
            }

            return configList.Where(x => x.ConfigCode == (long)configCode).ToDictionary(x => x.GlobalHash ?? 0, x => x.Value!.ToBoolean());
        }

        public async Task<Dictionary<long, long>> GetIntegerConfigAsync(ConfigCode configCode)
        {
            var configList = await CacheGetConfigListAsync();
            if (configList == null || configList.Count == 0)
            {
                return [];
            }

            return configList.Where(x => x.ConfigCode == (long)configCode).ToDictionary(x => x.GlobalHash ?? 0, x => x.Value!.ToInt64(0));
        }

        public async Task<Dictionary<long, decimal>> GetDecimalConfigAsync(ConfigCode configCode)
        {
            var configList = await CacheGetConfigListAsync();
            if (configList == null || configList.Count == 0)
            {
                return [];
            }

            return configList.Where(x => x.ConfigCode == (long)configCode).ToDictionary(x => x.GlobalHash ?? 0, x => x.Value!.ToDecimal(0));
        }


        public async Task<string> GetValueAsync(long globalHash)
        {
            var configList = await CacheGetConfigListAsync();
            if (configList == null || configList.Count == 0)
            {
                return string.Empty;
            }
            var config = configList.Find(c => c.GlobalHash == globalHash);
            if (config == null)
            {
                return string.Empty;
            }
            return config.Value ?? string.Empty;
        }

        public async Task<bool?> GetBooleanValueAsync(long globalHash)
        {
            var configList = await CacheGetConfigListAsync();
            if (configList == null || configList.Count == 0)
            {
                return null;
            }
            var config = configList.Find(c => c.GlobalHash == globalHash);
            if (config == null || config.DataCode != (long)DataCode.Boolean)
            {
                return null;
            }

            return config.Value?.ToBoolean();
        }

        public async Task<long?> GetIntegerValueAsync(long globalHash)
        {
            var configList = await CacheGetConfigListAsync();
            if (configList == null || configList.Count == 0)
            {
                return null;
            }
            var config = configList.Find(c => c.GlobalHash == globalHash);
            if (config == null || config.DataCode != (long)DataCode.Integer)
            {
                return null;
            }

            return config.Value?.ToInt64(0);
        }

        public async Task<decimal?> GetDecimalValueAsync(long globalHash)
        {
            var configList = await CacheGetConfigListAsync();
            if (configList == null || configList.Count == 0)
            {
                return null;
            }
            var config = configList.Find(c => c.GlobalHash == globalHash);
            if (config == null || config.DataCode != (long)DataCode.Decimal)
            {
                return null;
            }

            return config.Value?.ToDecimal(0);
        }
    }
}