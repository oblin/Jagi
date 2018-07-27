using JagiCore.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace JagiCoreTests
{
    public class DynamicClassTests
    {
        [Fact]
        public void Create_DynamicObject_By_Dictionary()
        {
            List<Field> codes = new List<Field>
            {
                new Field { FieldName = "NotNull", FieldType = typeof(string) },
                new Field { FieldName = "LargeThan5", FieldType = typeof(string) },
            };

            dynamic errorCodes = new DynamicClass(codes);
            errorCodes.NotNull = "不可以是空白";
            Assert.Equal("不可以是空白", errorCodes.NotNull);
            Assert.Throws<KeyNotFoundException>(() => errorCodes.LessThan5);
        }
    }
}
