using JagiCore;
using JagiCore.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JagiCoreTests
{
    public class CodeSample
    {
        public static Result<IEnumerable<CodeFile>> FakeCodes()
        {
            List<CodeFile> codes = new List<CodeFile>
            {
                new CodeFile
                {
                    Id = 1,
                    ItemType = "County",
                    CodeDetails = new List<CodeDetail>
                    {
                        new CodeDetail { ItemCode = "00", Description = "00 台北市" },
                        new CodeDetail { ItemCode = "01", Description = "01 台中市" },
                        new CodeDetail { ItemCode = "02", Description = "02 台南市" }
                    }
                },
                new CodeFile
                {
                    Id = 2,
                    ItemType = "Hospital",
                    ParentCode = "01",
                    CodeDetails = new List<CodeDetail>
                    {
                        new CodeDetail { ItemCode = "0001", Description = "0001 台大醫院" },
                        new CodeDetail { ItemCode = "0002", Description = "0002 台北榮民總醫院" },
                        new CodeDetail { ItemCode = "0003", Description = "0003 三軍總醫院" },
                        new CodeDetail { ItemCode = "0004", Description = "0004 台北榮民總醫院" }
                    }
                }
            };

            return codes.ToResult<IEnumerable<CodeFile>>("OK");
        }
    }
}
