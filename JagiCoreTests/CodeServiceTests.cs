using JagiCore.Interfaces;
using JagiCore.Services;
using Microsoft.Extensions.Caching.Memory;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using JagiCore;

namespace JagiCoreTests
{
    public class CodeServiceTests
    {
        [Fact]
        public void Set_CodeFile_To_Service()
        {
            IMemoryCache memoryCache = new MemoryCache(new MemoryCacheOptions());
            CodeFile codeFile = new CodeFile
            {
                ItemType = "County",
                CodeDetails = new List<CodeDetail>
                {
                    new CodeDetail { ItemCode = "00", Description = "00 台北市" },
                    new CodeDetail { ItemCode = "01", Description = "01 新北市" }
                }
            };

            var codeService = new CodeService(memoryCache);
            codeService.Add(codeFile);

            var result = codeService.GetDescription("County", "00");
            Assert.True(result.IsSuccess);
            Assert.Equal("00 台北市", result.Value);
        }

        [Fact]
        public void Get_Code_Descriptioin_By_ItemType_ItemCode()
        {
            IMemoryCache memoryCache = new MemoryCache(new MemoryCacheOptions());
            CodeFile codeFile = new CodeFile
            {
                ItemType = "County",
                CodeDetails = new List<CodeDetail>
                {
                    new CodeDetail { ItemCode = "00", Description = "00 台北市" },
                    new CodeDetail { ItemCode = "01", Description = "01 新北市" }
                }
            };

            var codeService = new CodeService(memoryCache);
            codeService.Add(codeFile);

            var result = codeService.GetDescription("County", "01");
            Assert.True(result.IsSuccess);
            Assert.Equal("01 新北市", result.Value);
        }

        [Fact]
        public void Code_Descriptioin_By_ItemType_ItemCode_Not_Found()
        {
            IMemoryCache memoryCache = new MemoryCache(new MemoryCacheOptions());
            var codeService = new CodeService(memoryCache);

            var result = codeService.GetDescription("County", "01");
            Assert.True(result.IsFailure);
            Assert.Equal("依據條件 item type.parent code.item code: COUNTY..01 無法找到對應的代碼", result.Error);
        }

        [Fact]
        public void Add_Duplicate_Code_Cause_Exception()
        {
            IMemoryCache memoryCache = new MemoryCache(new MemoryCacheOptions());
            var codeService = new CodeService(memoryCache);

            CodeFile codeFile = new CodeFile
            {
                ItemType = "County",
                CodeDetails = new List<CodeDetail>
                {
                    new CodeDetail { ItemCode = "00", Description = "00 台北市" },
                    new CodeDetail { ItemCode = "00", Description = "01 新北市" }
                }
            };
            Assert.Throws<ArgumentOutOfRangeException>(() => codeService.Add(codeFile));

            codeFile = new CodeFile
            {
                ItemType = "Hospital",
                ParentCode = "00",
                CodeDetails = new List<CodeDetail>
                {
                    new CodeDetail { ItemCode = "0001", Description = "0001 台大醫院" },
                    new CodeDetail { ItemCode = "0001", Description = "0002 台北榮民總醫院" }
                }
            };
            Assert.Throws<ArgumentOutOfRangeException>(() => codeService.Add(codeFile));
        }

        [Fact]
        public void Get_Code_Descriptioin_By_ItemType_ParentCode_ItemCode()
        {
            IMemoryCache memoryCache = new MemoryCache(new MemoryCacheOptions());
            var codeService = new CodeService(memoryCache);

            CodeFile codeFile = new CodeFile
            {
                ItemType = "County",
                CodeDetails = new List<CodeDetail>
                {
                    new CodeDetail { ItemCode = "00", Description = "00 台北市" },
                    new CodeDetail { ItemCode = "01", Description = "01 新北市" }
                }
            };
            codeService.Add(codeFile);

            codeFile = new CodeFile
            {
                ItemType = "Hospital",
                ParentCode = "00",
                CodeDetails = new List<CodeDetail>
                {
                    new CodeDetail { ItemCode = "0001", Description = "0001 台大醫院" },
                    new CodeDetail { ItemCode = "0002", Description = "0002 台北榮民總醫院" }
                }
            };
            codeService.Add(codeFile);

            var result = codeService.GetDescription("County", "01");
            Assert.True(result.IsSuccess);
            Assert.Equal("01 新北市", result.Value);

            result = codeService.GetDescription("Hospital", "0001", "00");
            Assert.True(result.IsSuccess);
            Assert.Equal("0001 台大醫院", result.Value);
        }

        [Fact]
        public void Code_Descriptioin_By_ItemType_ParentCode_ItemCode_Not_Found()
        {
            IMemoryCache memoryCache = new MemoryCache(new MemoryCacheOptions());
            var codeService = new CodeService(memoryCache);

            var codeFile = new CodeFile
            {
                ItemType = "Hospital",
                ParentCode = "00",
                CodeDetails = new List<CodeDetail>
                {
                    new CodeDetail { ItemCode = "0001", Description = "0001 台大醫院" },
                    new CodeDetail { ItemCode = "0002", Description = "0002 台北榮民總醫院" }
                }
            };
            codeService.Add(codeFile);

            var result = codeService.GetDescription("Hospital", "0003", "00");
            Assert.True(result.IsFailure);
            // All Code will be Upper Case in Cache
            Assert.Equal("依據條件 item type.parent code.item code: HOSPITAL.00.0003 無法找到對應的代碼", result.Error);
        }

        [Fact]
        public void Get_Codes_By_ItemType()
        {
            IMemoryCache memoryCache = new MemoryCache(new MemoryCacheOptions());
            var codeService = new CodeService(memoryCache);

            CodeFile codeFile = new CodeFile
            {
                ItemType = "County",
                CodeDetails = new List<CodeDetail>
                {
                    new CodeDetail { ItemCode = "00", Description = "00 台北市" },
                    new CodeDetail { ItemCode = "01", Description = "01 新北市" }
                }
            };
            codeService.Add(codeFile);

            var result = codeService.GetCodeDetails("County");
            Assert.True(result.IsSuccess);
            Assert.Equal(2, result.Value.Count);
            Assert.Equal("01 新北市", result.Value[1].Description);
        }

        [Fact]
        public void Get_Codes_By_ItemType_ParentCode()
        {
            IMemoryCache memoryCache = new MemoryCache(new MemoryCacheOptions());
            var codeService = new CodeService(memoryCache);

            CodeFile codeFile = new CodeFile
            {
                ItemType = "County",
                CodeDetails = new List<CodeDetail>
                {
                    new CodeDetail { ItemCode = "00", Description = "00 台北市" },
                    new CodeDetail { ItemCode = "01", Description = "01 新北市" }
                }
            };
            codeService.Add(codeFile);

            codeFile = new CodeFile
            {
                ItemType = "Hospital",
                ParentCode = "00",
                CodeDetails = new List<CodeDetail>
                {
                    new CodeDetail { ItemCode = "0001", Description = "0001 台大醫院" },
                    new CodeDetail { ItemCode = "0002", Description = "0002 台北榮民總醫院" },
                    new CodeDetail { ItemCode = "0003", Description = "0003 三軍總醫院" }
                }
            };
            codeService.Add(codeFile);

            var result = codeService.GetCodeDetails("County");
            Assert.True(result.IsSuccess);
            Assert.Equal(2, result.Value.Count);
            Assert.Equal("01 新北市", result.Value[1].Description);

            result = codeService.GetCodeDetails("Hospital", "00");
            Assert.True(result.IsSuccess);
            Assert.Equal(3, result.Value.Count);
            Assert.Equal("0002 台北榮民總醫院", result.Value[1].Description);
        }

        [Fact]
        public void Get_Dictionary_By_ItemType()
        {
            IMemoryCache memoryCache = new MemoryCache(new MemoryCacheOptions());
            var codeService = new CodeService(memoryCache);

            CodeFile codeFile = new CodeFile
            {
                ItemType = "County",
                CodeDetails = new List<CodeDetail>
                {
                    new CodeDetail { ItemCode = "00", Description = "00 台北市" },
                    new CodeDetail { ItemCode = "01", Description = "01 新北市" }
                }
            };
            codeService.Add(codeFile);

            var result = codeService.GetDetails("County");
            Assert.True(result.IsSuccess);
            Assert.Equal(2, result.Value.Count);
            Assert.True(result.Value.ContainsKey("00"));
            Assert.Equal("01 新北市", result.Value["01"]);

            Assert.False(result.Value.ContainsKey("03"));
        }

        [Fact]
        public void Get_Dictionary_By_ItemType_ParentCode()
        {
            IMemoryCache memoryCache = new MemoryCache(new MemoryCacheOptions());
            var codeService = new CodeService(memoryCache);

            CodeFile codeFile = new CodeFile
            {
                ItemType = "County",
                CodeDetails = new List<CodeDetail>
                {
                    new CodeDetail { ItemCode = "00", Description = "00 台北市" },
                    new CodeDetail { ItemCode = "01", Description = "01 新北市" }
                }
            };
            codeService.Add(codeFile);

            codeFile = new CodeFile
            {
                ItemType = "Hospital",
                ParentCode = "00",
                CodeDetails = new List<CodeDetail>
                {
                    new CodeDetail { ItemCode = "0001", Description = "0001 台大醫院" },
                    new CodeDetail { ItemCode = "0002", Description = "0002 台北榮民總醫院" },
                    new CodeDetail { ItemCode = "0003", Description = "0003 三軍總醫院" }
                }
            };
            codeService.Add(codeFile);

            var result = codeService.GetDetails("County");
            Assert.True(result.IsSuccess);
            Assert.Equal(2, result.Value.Count);
            Assert.True(result.Value.ContainsKey("00"));
            Assert.Equal("01 新北市", result.Value["01"]);

            Assert.False(result.Value.ContainsKey("03"));

            result = codeService.GetDetails("Hospital", "00");
            Assert.True(result.IsSuccess);
            Assert.Equal(3, result.Value.Count);
            Assert.True(result.Value.ContainsKey("0002"));
            Assert.Equal("0003 三軍總醫院", result.Value["0003"]);

            Assert.False(result.Value.ContainsKey("0004"));
        }

        [Fact]
        public void Can_Delete_Codes_By_CodeFile()
        {
            IMemoryCache memoryCache = new MemoryCache(new MemoryCacheOptions());
            var codeService = new CodeService(memoryCache);

            CodeFile codeFile = new CodeFile
            {
                ItemType = "County",
                CodeDetails = new List<CodeDetail>
                {
                    new CodeDetail { ItemCode = "00", Description = "00 台北市" },
                    new CodeDetail { ItemCode = "01", Description = "01 新北市" }
                }
            };
            codeService.Add(codeFile);

            codeService.Remove(codeFile);

            var result = codeService.GetCodeDetails("County");
            Assert.True(result.IsFailure);
        }

        [Fact]
        public void Can_Delete_Codes_By_ItemType_ParentCode()
        {
            IMemoryCache memoryCache = new MemoryCache(new MemoryCacheOptions());
            var codeService = new CodeService(memoryCache);

            var codeFile = new CodeFile
            {
                ItemType = "Hospital",
                ParentCode = "00",
                CodeDetails = new List<CodeDetail>
                {
                    new CodeDetail { ItemCode = "0001", Description = "0001 台大醫院" },
                    new CodeDetail { ItemCode = "0002", Description = "0002 台北榮民總醫院" },
                    new CodeDetail { ItemCode = "0003", Description = "0003 三軍總醫院" }
                }
            };
            codeService.Add(codeFile);

            codeFile = new CodeFile
            {
                ItemType = "Hospital",
                ParentCode = "01",
                CodeDetails = new List<CodeDetail>
                {
                    new CodeDetail { ItemCode = "0001", Description = "0001 台大醫院" },
                    new CodeDetail { ItemCode = "0002", Description = "0002 台北榮民總醫院" },
                    new CodeDetail { ItemCode = "0003", Description = "0003 三軍總醫院" }
                }
            };
            codeService.Add(codeFile);

            var result = codeService.GetDetails("Hospital", "00");
            Assert.True(result.IsSuccess);

            codeService.Remove("Hospital", "00");

            result = codeService.GetDetails("Hospital", "00");
            Assert.True(result.IsFailure);

            result = codeService.GetDetails("Hospital", "01");
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public void Delete_Codes_By_Wrong_ItemType_ParentCode()
        {
            IMemoryCache memoryCache = new MemoryCache(new MemoryCacheOptions());
            var codeService = new CodeService(memoryCache);

            var codeFile = new CodeFile
            {
                ItemType = "Hospital",
                ParentCode = "00",
                CodeDetails = new List<CodeDetail>
                {
                    new CodeDetail { ItemCode = "0001", Description = "0001 台大醫院" },
                    new CodeDetail { ItemCode = "0002", Description = "0002 台北榮民總醫院" },
                    new CodeDetail { ItemCode = "0003", Description = "0003 三軍總醫院" }
                }
            };
            codeService.Add(codeFile);

            codeFile = new CodeFile
            {
                ItemType = "Hospital",
                ParentCode = "01",
                CodeDetails = new List<CodeDetail>
                {
                    new CodeDetail { ItemCode = "0001", Description = "0001 台大醫院" },
                    new CodeDetail { ItemCode = "0002", Description = "0002 台北榮民總醫院" },
                    new CodeDetail { ItemCode = "0003", Description = "0003 三軍總醫院" }
                }
            };
            codeService.Add(codeFile);

            var result = codeService.GetDetails("Hospital", "00");
            Assert.True(result.IsSuccess);

            codeService.Remove("Hospital", "00");

            result = codeService.GetDetails("Hospital", "00");
            Assert.True(result.IsFailure);

            codeService.Remove("Hospital", "02"); // 沒有資料不會造成錯誤， silence failed

            result = codeService.GetDetails("Hospital", "01");  // 驗證 Cache 還是有資料
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public void Can_Update_Codes_By_CodeFile()
        {
            IMemoryCache memoryCache = new MemoryCache(new MemoryCacheOptions());
            var codeService = new CodeService(memoryCache);

            CodeFile codeFile = new CodeFile
            {
                ItemType = "County",
                CodeDetails = new List<CodeDetail>
                {
                    new CodeDetail { ItemCode = "00", Description = "00 台北市" },
                    new CodeDetail { ItemCode = "01", Description = "01 新北市" }
                }
            };
            codeService.Add(codeFile);
            var result = codeService.GetCodeDetails("County");
            Assert.Equal(2, result.Value.Count);

            codeFile = new CodeFile
            {
                ItemType = "County",
                CodeDetails = new List<CodeDetail>
                {
                    new CodeDetail { ItemCode = "00", Description = "00 台北市" },
                    new CodeDetail { ItemCode = "01", Description = "01 台中市" },
                    new CodeDetail { ItemCode = "02", Description = "02 台南市" }
                }
            };
            codeService.Update(codeFile);

            result = codeService.GetCodeDetails("County");
            Assert.True(result.IsSuccess);
            Assert.Equal(3, result.Value.Count);
            Assert.Equal("02 台南市", result.Value[2].Description);
        }

        [Fact]
        public void Can_Add_Repository_To_CodeService()
        {
            var repo = Substitute.For<IRepository<CodeFile>>();
            repo.GetAll().Returns(CodeSample.FakeCodes());

            var service = CodeService.Create(repo);

            var result = service.GetCodeDetails("County");
            Assert.True(result.IsSuccess);
            Assert.Equal(3, result.Value.Count);
            Assert.Equal("02 台南市", result.Value[2].Description);

            result = service.GetCodeDetails("Hospital", "00");
            Assert.True(result.IsFailure);

            result = service.GetCodeDetails("Hospital", "01");
            Assert.Equal(4, result.Value.Count);
            Assert.Equal("0002 台北榮民總醫院", result.Value[1].Description);
        }

        [Fact]
        public void Repository_To_CodeService_Test_Get_Functions()
        {
            var repo = Substitute.For<IRepository<CodeFile>>();
            repo.GetAll().Returns(CodeSample.FakeCodes());

            var service = CodeService.Create(repo);

            var result = service.GetDescription("County", "02");
            Assert.True(result.IsSuccess);
            Assert.Equal("02 台南市", result.Value);

            var detailResult = service.GetDetails("Hospital", "01");
            Assert.True(detailResult.IsSuccess);
            Assert.Equal(4, detailResult.Value.Count);
            Assert.Equal("0002 台北榮民總醫院", detailResult.Value["0002"]);
        }
    }
}
