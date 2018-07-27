using JagiCore;
using JagiCore.Helpers;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace JagiCoreTests
{
    public class ErrorCodesTests
    {
        [Fact]
        public void Create_ErrorCodes_By_Dictionary()
        {
            Dictionary<string, string> codes = new Dictionary<string, string>
            {
                { "NotNull", "不可以是空白" },
                { "LargeThan5", "不可以大於五" },
            };
            var errorCodes = ErrorCodes.Create(codes);
            Assert.Equal("不可以是空白", errorCodes.NotNull);
            Assert.Equal("不可以大於五", errorCodes.LargeThan5);
        }

        [Fact]
        public void Create_ErrorCodes_From_Json_Configuration_File()
        {
            IConfiguration config = new ConfigurationBuilder()
              .SetBasePath(Directory.GetCurrentDirectory())
              .AddJsonFile("error-resource.json", optional: true, reloadOnChange: true)
              .Build();

            var maps = ErrorCodes.Create(config);

            Assert.Equal("不可以是 Null", maps.NotNull);
            // 提供 Get() method 用來取得對應的錯誤訊息
            Assert.Equal("不可以是 Null", maps.Get("NotNull"));
            // Dynamic 不可以使用 Dictionary 的表達方式
            Assert.Throws<RuntimeBinderException>(() => "不可以是 Null" == maps["NotNull"]);

            Assert.Equal("必須要是 Null", maps.Null);
        }

        [Fact]
        public void Error_Code_With_Arguments_Format()
        {
            IConfiguration config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "NotNull", "不可以是空值" },
                    { "IdNotFound", "找不到 Id: {0} 的病人資料" },
                    { "IdNameNotFound", "找不到 Id: {0} 與姓名是 {1} 的病人資料" }
                })
                .Build();

            var errorCodes = ErrorCodes.Create(config);

            Assert.Equal("不可以是空值", errorCodes.NotNull);
            Assert.Equal("找不到 Id: 1 的病人資料", string.Format(errorCodes.IdNotFound, 1));
            // 可以使用 errorCodes.FormatWith("Key", params object[]) 取得可以 format 的資料
            Assert.Equal("找不到 Id: 1 的病人資料", errorCodes.FormatWith("IdNotFound", 1));
            Assert.Equal("找不到 Id: 2 與姓名是 yiming 的病人資料", errorCodes.FormatWith("IdNameNotFound", 2, "yiming"));
        }
    }
}
