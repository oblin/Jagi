using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JagiCore.Helpers;
using Xunit;
using System.Dynamic;
using Newtonsoft.Json;

namespace JagiCoreTests
{
    public class TypeHelperTests
    {
        [Fact]
        public void Convert_Json_String_To_Dictionary()
        {
            const string customerJson = "{ 'Id': 1, 'FirstName': 'First', 'LastName': 'Second', 'Description': 'Test' }";

            IDictionary<string, object> result = 
                JsonConvert.DeserializeObject<IDictionary<string, object>>(customerJson);

            Assert.Equal("1", result["Id"].ToString());
            Assert.Equal("First", result["FirstName"]);
            Assert.NotEqual("Second", result["Description"]);
        }

        [Fact]
        public void Convert_String_To_Pascal_String()
        {
            string target = "yiming";

            Assert.Equal("Yiming", target.ToPascalCase());

            target = "yiming lin";
            Assert.Equal("YimingLin", target.ToPascalCase());

            target = "yimingLin";
            Assert.Equal("YimingLin", target.ToPascalCase());
        }

        [Fact]
        public void Convert_String_To_Camel_String()
        {
            string target = "Yiming";

            Assert.Equal("yiming", target.ToCamelCase());

            target = "yiming lin";
            Assert.Equal("yimingLin", target.ToCamelCase());

            target = "YimingLin";
            Assert.Equal("yimingLin", target.ToCamelCase());
        }

        [Fact]
        public void String_Append_With_Seperator()
        {
            string text1 = "This", text2 = "is", text3 = "book";

            Assert.Equal("This is book", text1.AppendSeperator(text2.AppendSeperator(text3)));
        }

        [Fact]
        public void Test_Enum_To_Dictionary()
        {
            var enums = TypeHelper.ToEnumDictionary<RadioOption>();

            Assert.Equal(3, enums.Count);
            Assert.Equal("Option1", enums[0]);
            Assert.Equal("Option2", enums[1]);
        }

        [Fact]
        public void Test_Enum_Object_To_Dictionary()
        {
            var item = RadioOption.Option1;
            var enums = TypeHelper.ToEnumDictionary(item.GetType());

            Assert.Equal(3, enums.Count);
            Assert.Equal("Option1", enums[0]);
            Assert.Equal("Option2", enums[1]);
        }
    }
}
