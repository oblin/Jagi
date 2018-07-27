using JagiCore.Interfaces;
using JagiCore.Helpers;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JagiCore.Services
{
    public class AddressService
    {
        private const string NOT_FOUND = "查詢 {0} 的值: {1} 並未找到";

        private IMemoryCache _cache;

        public AddressService(IMemoryCache cache)
        {
            _cache = cache;
        }

        /// <summary>
        /// 提供使用 Repository （資料庫）的內容加入到 Cache 中。
        /// 請注意： Cache 所有的修改機制都僅限於 Cache 內部，無法回寫到資料庫中
        /// 因此如果有修改，必須要資料庫跟 Cache 兩邊都修改；或者重新執行 Create 載入整個內容
        /// </summary>
        /// <param name="repository"></param>
        /// <returns></returns>
        public static AddressService Create(IRepository<Address> repository)
        {
            var service = new AddressService(new MemoryCache(new MemoryCacheOptions()));
            return repository.GetAll()
                .OnSuccess(r => r.ToList().ForEach(item => service.Add(item)))
                .OnBoth(r => r.IsSuccess ? service : null);
        }

        public Result<AddressQueryResult> GetByZip(string zip)
        {
            return GetFromCache(AddressResultType.Zip, zip);
        }

        public Result<AddressQueryResult> GetByCounty(string county)
        {
            return GetFromCache(AddressResultType.County, county);
        }

        public Result<AddressQueryResult> GetByRealm(string realm)
        {
            return GetFromCache(AddressResultType.Realm, realm);
        }

        public void Add(Address address)
        {
            AddAddressByType(AddressResultType.Zip, address.Zip, address);
            AddAddressByType(AddressResultType.County, address.County, address);
            AddAddressByType(AddressResultType.Realm, address.Realm, address);
        }

        public void Remove(Address address)
        {
            RemoveAddressByType(AddressResultType.Zip, address.Zip, address);
            RemoveAddressByType(AddressResultType.County, address.County, address);
            RemoveAddressByType(AddressResultType.Realm, address.Realm, address);
        }

        private void RemoveAddressByType(AddressResultType zip, string keyword, Address address)
        {
            var key = GetKey(zip, keyword);
            AddressQueryResult result = _cache.Get(key) as AddressQueryResult;
            if (result == null)
                return;
            result = RemoveItemInAddressResult(result, address);
            if (result.Counties.Length == 0 && result.Zips.Length == 0)
                _cache.Remove(key);
            else
                _cache.Set(key, result, new MemoryCacheEntryOptions().SetPriority(CacheItemPriority.NeverRemove));
        }

        private AddressQueryResult RemoveItemInAddressResult(AddressQueryResult result, Address address)
        {
            if (!string.IsNullOrEmpty(address.Street))
                result.Streets = result.Streets.RemoveItem(address.Street);
            // 因為 Streets 是最小單位，因此如果沒有 Street 才需要刪除其他內容
            if (result.Streets == null || result.Streets.Length == 0)
            { 
            if (!string.IsNullOrEmpty(address.Zip))
                result.Zips = result.Zips.RemoveItem(address.Zip);

            if (!string.IsNullOrEmpty(address.County))
                result.Counties = result.Counties.RemoveItem(address.County);

            if (!string.IsNullOrEmpty(address.Realm))
                result.Realms = result.Realms.RemoveItem(address.Realm);
            }
            return result;
        }

        private void AddAddressByType(AddressResultType zip, string keyword, Address address)
        {
            var key = GetKey(zip, keyword);
            AddressQueryResult result = _cache.Get(key) as AddressQueryResult;
            _cache.Set(key, ComposeAddressQueryResult(result, address),
                new MemoryCacheEntryOptions().SetPriority(CacheItemPriority.NeverRemove));
        }

        private AddressQueryResult ComposeAddressQueryResult(AddressQueryResult result, Address address)
        {
            if (result == null)
                result = new AddressQueryResult();

            if (!string.IsNullOrEmpty(address.Zip))
                result.Zips = result.Zips.AddItem(address.Zip);

            if (!string.IsNullOrEmpty(address.County))
                result.Counties = result.Counties.AddItem(address.County);

            if (!string.IsNullOrEmpty(address.Realm))
                result.Realms = result.Realms.AddItem(address.Realm);

            if (!string.IsNullOrEmpty(address.Street))
                result.Streets = result.Streets.AddItem(address.Street);

            return result;
        }

        private string GetKey(AddressResultType zip, string key)
        {
            return Enum.GetName(typeof(AddressResultType), zip) + "_" + key;
        }

        private Result<AddressQueryResult> GetFromCache(AddressResultType type, string key)
        {
            AddressQueryResult result;
            var cacheKey = GetKey(type, key);
            if (_cache.TryGetValue(cacheKey, out result))
                return Result.Ok<AddressQueryResult>(result);

            string typeName = Enum.GetName(typeof(AddressResultType), type);
            return Result.Fail<AddressQueryResult>(
                string.Format(NOT_FOUND, typeName, key));
        }
    }

    enum AddressResultType
    {
        Zip,
        County,
        Realm,
    }
}
