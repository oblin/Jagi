using JagiCore.Helpers;
using Xunit;

namespace JagiCoreTests
{
    public class UtilityTests
    {
        [Fact]
        public void Test_Oracle_Is_Chinese_End()
        {
            string chinese = "這是中文字串";

            var result = Utility.TrimOracleEndNotChineseChar(chinese);

            Assert.Equal(chinese, result);
        }

        [Fact]
        public void Test_Oracle_Not_Chinese_End()
        {
            string chinese = "上海聯合藥房股份有限公司밨";
            string chineseCorrect = "上海聯合藥房股份有限公司";

            var result = Utility.TrimOracleEndNotChineseChar(chinese);

            Assert.NotEqual(chinese, result);
            Assert.Equal(chineseCorrect, result);
        }
    }
}
