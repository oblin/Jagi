using JagiCore.Helpers;
using JagiCore.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JagiCore.Services
{
    public class CodeService
    {
        private const string KEY_OUT_OF_RANGE = "依據條件 item type.parent code.item code: {0} 無法找到對應的代碼，或者超過一個以上的代碼";
        private const string KEY_NOT_FOUND = "依據條件 item type.parent code.item code: {0} 無法找到對應的代碼";

        private IMemoryCache _cache;

        public CodeService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public static CodeService Create(IRepository<CodeFile> repository)
        {
            var service = new CodeService(new MemoryCache(new MemoryCacheOptions()));

            return repository.GetAll()
                .OnSuccess(r => r.ToList().ForEach(item => service.Add(item)))
                .OnBoth(r => r.IsSuccess ? service : null);
        }

        public Result<string> GetDescription(string itemType, string itemCode)
        {
            return GetDescription(itemType, itemCode, string.Empty);
        }

        public Result<string> GetDescription(string itemType, string itemCode, string parentCode)
        {
            var key = GetKey(itemType, parentCode, itemCode);
            IEnumerable<Code> codes;
            if (_cache.TryGetValue(key, out codes))
            {
                if (codes.Count() != 1)
                    throw new ArgumentOutOfRangeException(KEY_OUT_OF_RANGE.FormatWith(itemType, parentCode, itemCode));

                return Result.Ok(codes.First().Description);
            }
            return Result.Fail<string>(KEY_NOT_FOUND.FormatWith(key));
        }


        public Result<List<Code>> GetCodeDetails(string itemType)
        {
            return GetCodeDetails(itemType, string.Empty);
        }

        public Result<Dictionary<string, string>> GetDetails(string itemType)
        {
            return GetDetails(itemType, string.Empty);
        }

        public Result<Dictionary<string, string>> GetDetails(string itemType, string parentCode)
        {
            //return GetCodeDetails(itemType, parentCode)
            //        .OnSuccess(codes => 
            //            codes.Select(c => new { Key = c.ItemCode, Value = c.Description })
            //                .ToDictionary(k => k.Key, v => v.Value));

            return GetCodeDetails(itemType, parentCode)
                    .OnSuccess(codes =>
                        codes.ToDictionary(k => k.ItemCode, v => v.Description));
        }

        public Result<List<Code>> GetCodeDetails(string itemType, string parentCode)
        {
            List<Code> codes;
            var key = GetKey(itemType, parentCode, string.Empty);
            return _cache.TryGetValue(key, out codes)
                ? Result.Ok(codes.OrderBy(c => c.ItemCode).ToList())
                : Result.Fail<List<Code>>(KEY_NOT_FOUND.FormatWith(key));
        }

        public void Add(CodeFile codeFile)
        {
            List<Code> codes = ConvertCodeFileToCodes(codeFile);
            string key = GetKey(codeFile.ItemType, codeFile.ParentCode, string.Empty);
            AddByCodeFile(key, codes);
            foreach(var item in codeFile.CodeDetails)
            {
                key = GetKey(codeFile.ItemType, codeFile.ParentCode, item.ItemCode);
                var detailCode = codes.Where(c => c.ItemCode == item.ItemCode);
                if (detailCode.Count() != 1)
                    throw new ArgumentOutOfRangeException(KEY_OUT_OF_RANGE.FormatWith(codeFile.ItemType, codeFile.ParentCode, item.ItemCode));

                AddByCodeFile(key, detailCode.ToList());
            }
        }

        public void Remove(CodeFile codeFile)
        {
            Remove(codeFile.ItemType, codeFile.ParentCode);
        }

        public void Remove(string itemType, string parentCode = null)
        {
            string key = GetKey(itemType, parentCode, string.Empty);
            _cache.Remove(key);
            GetCodeDetails(itemType, parentCode)
                .OnSuccess(codes => codes.ForEach(code => _cache.Remove(GetKey(itemType, parentCode, code.ItemCode))));
        }

        public void Update(CodeFile codeFile)
        {
            Remove(codeFile);
            Add(codeFile);
        }

        private void AddByCodeFile(string key, List<Code> codes)
        {
            _cache.Set(key, codes, new MemoryCacheEntryOptions().SetPriority(CacheItemPriority.NeverRemove));
        }

        private List<Code> ConvertCodeFileToCodes(CodeFile codeFile)
        {
            List<Code> result = new List<Code>();
            codeFile.CodeDetails.ToList().ForEach(c => result.Add(new Code
            {
                ItemType = codeFile.ItemType,
                ItemCode = c.ItemCode,
                ParentType = codeFile.ParentType,
                ParentCode = codeFile.ParentCode,
                Description = c.Description
            }));
            return result;
        }

        private string GetKey(string itemType, string parentCode, string itemCode)
        {
            // 一律改為大寫
            return itemType?.ToUpper() + "." + parentCode?.ToUpper() + "." + itemCode?.ToUpper();
        }
    }
}
