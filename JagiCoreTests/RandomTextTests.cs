using JagiCore.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace JagiCoreTests
{
    public class RandomTextTests
    {
        [Fact]
        public void Test_Random_Default_Text_Length_12()
        {
            var random = new RandomText();
            var result = random.Generate();

            Assert.Equal(12, result.Length);
        }

        [Fact]
        public void Test_Random_Text_Length_10()
        {
            var random = new RandomText();
            var result = random.Generate(10);

            Assert.Equal(10, result.Length);
        }
    }
}
