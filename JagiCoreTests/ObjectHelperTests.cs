using JagiCore.Helpers;
using JagiCoreTests.ObjectHelperSample;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace JagiCoreTests
{
    public class ObjectHelperTests
    {
        private List<SampleCopy2> samples;

        public ObjectHelperTests()
        {
            samples = new List<SampleCopy2>
            {
                new SampleCopy2 { Number = 11, Text = "Abc" },
                new SampleCopy2 { Number = 12, Text = "ABc" },
                new SampleCopy2 { Number = 16, Text = "web", StartDate = new DateTime(2015,3,2) }
            };
        }

        [Fact]
        public void Test_CopyTo_Object_Basic_Type()
        {
            var result = samples[0].CopyTo();
            Assert.Equal(11, result.Number);
            Assert.Equal("Abc", result.Text);

            result = new SampleCopy2();
            samples[1].CopyTo(result);
            Assert.Equal(12, result.Number);
            Assert.Equal("ABc", result.Text);
        }

        [Fact]
        public void Test_CopyTo_Date_Type()
        {
            var result = samples[2].CopyTo();
            Assert.Equal("web", result.Text);
            Assert.Equal("2015-3-2", ((DateTime)result.StartDate).ToString("yyyy-M-d"));
        }

        [Fact]
        public void Test_CopyToExcludeNull_不會變更既有的資料()
        {
            var result = samples[1].CopyToExcludeNull();
            Assert.Null(result.StartDate);

            // 測試 Exclude null 不會將空白資料 copy to 欄位
            result = new SampleCopy2();
            DateTime targetDate = new DateTime(2015, 3, 4);
            result.StartDate = targetDate;

            // 因為 Sample.StartDate 是 null，因此既有的 StartDate 不會被變更
            samples[1].CopyToExcludeNull(result);
            Assert.Equal(targetDate, result.StartDate);

            // 測試 Copy 會將 null copy to 欄位
            samples[1].CopyTo(result);
            Assert.Null(result.StartDate);
        }

        [Fact]
        public void Test_CopyTo_Complex_Property()
        {
            SampleCopy2 source = new SampleCopy2
            {
                Number = 1,
                Text = "ComplexField"
            };

            var result = source.CopyTo();
            Assert.Equal(1, result.Number);
            Assert.Null(result.StartDate);

            DateTime testDate = new DateTime(2012, 2, 2);
            source.ComplexProp = new Sample { Number = 10, Text = "Detail", StartDate = testDate };

            result = source.CopyTo();

            Assert.Equal(1, result.Number);
            Assert.Equal(10, result.ComplexProp.Number);
            Assert.Equal("Detail", result.ComplexProp.Text);
            Assert.Equal(testDate, result.ComplexProp.StartDate);
        }

        [Fact]
        public void Test_CopyToExclude_不會變更既有的物件型態資料()
        {
            SampleCopy2 source = new SampleCopy2
            {
                Number = 1,
                Text = "ComplexField"
            };

            Assert.Null(source.ComplexProp);

            Sample testSample = new Sample { Number = 10, Text = "Detail" };
            DateTime testDate = new DateTime(2012, 2, 2);
            SampleCopy2 target = new SampleCopy2
            {
                StartDate = testDate,
                ComplexProp = testSample
            };

            source.CopyToExcludeNull(target);
            Assert.Equal(1, target.Number);
            Assert.NotNull(target.ComplexProp);
            Assert.Equal(10, target.ComplexProp.Number);
            Assert.Equal("Detail", target.ComplexProp.Text);
        }

        [Fact]
        public void Test_CopyTo_Differenct_Class()
        {
            Sample testSample = new Sample { Number = 10, Text = "Detail" };
            DateTime testDate = new DateTime(2012, 2, 2);
            SampleCopy2 source = new SampleCopy2
            {
                Number = 1,
                Text = "ComplexField",
                StartDate = testDate,
                ComplexProp = testSample
            };

            SampleCopy3 target = new SampleCopy3
            {
                Id = 1,
                Number = 2
            };

            source.CopyTo(target);

            Assert.Equal(1, target.Number);
            Assert.Equal(1, target.Id);
        }

        [Fact]
        public void Can_Set_Any_Type_Value()
        {
            DateTime date = new DateTime(2016, 2, 28);

            var sampe = new Sample();
            sampe.SetPropertyValue(s => s.Text, "Test");
            sampe.SetPropertyValue(s => s.Number, 10);
            sampe.SetPropertyValue(s => s.StartDate, date);

            Assert.Equal("Test", sampe.Text);
            Assert.Equal(10, sampe.Number);
            Assert.Equal(date, sampe.StartDate);

            var any = new { Text = string.Empty, Number = 11, Date = date };
            any.SetPropertyValue(a => a.Text, "Check");
            Assert.Equal("Check", any.Text);
            Assert.Equal(11, any.Number);
            Assert.Equal(date, any.Date);
        }
    }
}
