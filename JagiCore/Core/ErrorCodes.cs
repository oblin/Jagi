using JagiCore.Helpers;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace JagiCore
{
    public class ConfigurationDictionaryBinder
    {
        public Dictionary<string, string> ConfigurationDictionary { get; set; }
    }

    /// <summary>
    /// 提供處理 錯誤代碼與錯誤訊息對應的簡單機制，可以從 JSON Configuration File 定義錯誤訊息，傳入到 ErrorCodes.Create()
    /// 就會產生一個 dynamic object 可以直接使用如： error.NotNull => "不可以是 null" 這樣的錯誤訊息內容
    /// 額外提供 error.Get("ErrorCode") 與 error.FormatWith("ErrorCode", args) 可以用來輸入產生錯誤的字串
    /// 詳細參閱 ErrorCodesTests.cs 中的寫法
    /// </summary>
    public class ErrorCodes : DynamicObject
    {
        private readonly Dictionary<string, string> _errorDictionary = null;
        private ErrorCodes(Dictionary<string, string> errorDictionary)
        {
            _errorDictionary = errorDictionary;
        }

        private ErrorCodes(IConfiguration configuration)
        {
            _errorDictionary = new Dictionary<string, string>();
            foreach(var item in configuration.AsEnumerable())
            {
                _errorDictionary.Add(item.Key, item.Value);
            }
        }

        private static dynamic _errors;

        public static dynamic Instance = _errors;

        public static dynamic Create(Dictionary<string, string> errorDictionary)
        {
            _errors = new ErrorCodes(errorDictionary);

            return _errors;
        }

        public static dynamic Create(IConfiguration configuration)
        {
            var errorDictionary = new Dictionary<string, string>();
            foreach (var item in configuration.AsEnumerable())
            {
                errorDictionary.Add(item.Key, item.Value);
            }

            return Create(errorDictionary);
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = null;
            if (_errorDictionary.ContainsKey(binder.Name))
            {
                result = _errorDictionary[binder.Name];
                return true;
            }

            return false;
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            switch (binder.Name)
            {
                case "FormatWith":
                    string key = args[0].ToString();
                    if (_errorDictionary.ContainsKey(key))
                    {
                        object[] restArgs = args.Skip(1).ToArray();
                        result = _errorDictionary[key].FormatWith(restArgs);
                        return true;
                    }
                    break;
                case "Get":
                    key = args[0].ToString();
                    if (_errorDictionary.ContainsKey(key))
                    {
                        result = _errorDictionary[key];
                        return true;
                    }
                    break;
            }
            result = null;
            return false;
        }
    }
}
