using JagiCore.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace JagiCoreTests
{
    public class DateTimeHelperTests
    {
        [Fact]
        public void Convert_Westen_Date_To_Chinese_Date_String()
        {
            var date = new DateTime(2016, 1, 29);
            var chineseDateString = date.ToTaiwanString();

            Assert.Equal("105/01/29", chineseDateString);
        }

        [Fact]
        public void Exception_If_Year_Less_Than_1911()
        {
            var date = new DateTime(1910, 12, 31);
            Assert.Throws<ArgumentOutOfRangeException>(() => date.ToTaiwanString());
        }

        [Fact]
        public void 測試閏月也可以正常顯示()
        {
            var date = new DateTime(2016, 2, 29);
            var chineseDateString = date.ToTaiwanString();

            Assert.Equal("105/02/29", chineseDateString);
        }

        [Fact]
        public void Convert_DateTime_Nullable_To_Standard_String()
        {
            var date = new DateTime(2016, 2, 29);
            var dateString = date.ToCommonString();

            Assert.Equal("2016/02/29", dateString);

            DateTime? nullable = null;
            dateString = nullable.ToCommonString();
            Assert.True(string.IsNullOrEmpty(dateString));
        }

        [Fact]
        public void DateTime_To_Standard_String()
        {
            string dateString = "1070101";

            Assert.Equal(new DateTime(2018, 1, 1), dateString.ConvertChineseToDateTime());
        }
    }
}
