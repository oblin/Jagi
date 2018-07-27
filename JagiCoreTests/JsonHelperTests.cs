using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JagiCore.Helpers;
using Xunit;
using Newtonsoft.Json;
using System.Dynamic;

namespace JagiCoreTests
{
    public class JsonHelperTests
    {
        [Fact]
        public void Convert_Object_To_Json_String()
        {
            var obj = new { Id = 1, Text = "Test" };
            string result = obj.ToJsonString();

            Assert.Equal("{\"Id\":1,\"Text\":\"Test\"}", result);

            dynamic jsonObject = JsonConvert.DeserializeObject<ExpandoObject>(result);
            Assert.Equal("Test", jsonObject.Text);
        }

        [Fact]
        public void Convert_Object_Without_Null_Value_To_Json_String()
        {
            var obj = new TestSample1 { Id = 1, Text = "Test" };
            string result = obj.ToJsonString();

            Assert.Equal("{\"Id\":1,\"Text\":\"Test\"}", result);

            dynamic jsonObject = JsonConvert.DeserializeObject<TestSample1>(result);
            Assert.Equal("Test", jsonObject.Text);
        }

        [Fact]
        public void Convert_Object_With_Null_Value_To_Json_String()
        {
            var obj = new TestSample1 { Id = 1, Text = "Test" };
            string result = obj.ToJsonString(includeNull: true);

            Assert.Equal("{\"Id\":1,\"Text\":\"Test\",\"Desc\":null}", result);
        }

        [Fact]
        public void Convert_Object_To_Json_Camel_String()
        {
            var obj = new { Id = 1, Text = "Test" };
            string result = obj.ToCamelJsonString();

            Assert.Equal("{\"id\":1,\"text\":\"Test\"}", result);
        }

        [Fact]
        public void Convert_Object_Without_Null_Value_To_Camel_Json_String()
        {
            var obj = new TestSample1 { Id = 1, Text = "Test" };
            string result = obj.ToCamelJsonString();

            Assert.Equal("{\"id\":1,\"text\":\"Test\"}", result);
        }

        [Fact]
        public void Convert_Object_With_Null_Value_To_Camel_Json_String()
        {
            var obj = new TestSample1 { Id = 1, Text = "Test" };
            string result = obj.ToCamelJsonString(includeNull: true);

            Assert.Equal("{\"id\":1,\"text\":\"Test\",\"desc\":null}", result);
        }

        [Fact]
        public void Convert_Object_With_DateTime_Value_To_Json_String()
        {
            var obj = new TestSample2 { Id = 1, Text = "Test", StartDate = new DateTime(1998, 2, 28) };
            string result = obj.ToJsonString(includeNull: true);

            Assert.Equal("{\"Id\":1,\"Text\":\"Test\",\"Desc\":null,\"StartDate\":\"1998-02-28T00:00:00\"}", result);
        }

        [Fact]
        public void Convert_Object_With_Null_DateTime_Value_To_Json_String()
        {
            var obj = new TestSample3 { Id = 1, Text = "Test", StartDate = new DateTime(1998, 2, 28) };
            string result = obj.ToJsonString(includeNull: true);

            Assert.Equal("{\"Id\":1,\"Text\":\"Test\",\"Desc\":null,\"StartDate\":\"1998-02-28T00:00:00\"}", result);

            obj = new TestSample3 { Id = 1, Text = "Test" };
            result = obj.ToJsonString(includeNull: true);

            Assert.Equal("{\"Id\":1,\"Text\":\"Test\",\"Desc\":null,\"StartDate\":null}", result);

            obj = new TestSample3 { Id = 1, Text = "Test" };
            result = obj.ToJsonString();

            Assert.Equal("{\"Id\":1,\"Text\":\"Test\"}", result);
        }

        [Fact]
        public void Convert_Object_Property_To_Name_String()
        {
            var obj = new TestSample1 { Text = "TextName" };
            string prefixPropString = obj.ToPrefixPropertyString(o => o.Text, "model");

            Assert.Equal("model.Text", prefixPropString);
        }

        [Fact]
        public void Convert_Json_String_To_Specified_Class()
        {
            var jsonString = "{\"Id\":1,\"Text\":\"Test\"}";

            TestSample1 sample = jsonString.JsonStringToObject<TestSample1>();
            Assert.Equal(1, sample.Id);
            Assert.Equal("Test", sample.Text);
        }

        [Fact]
        public void Convert_Json_String_To_Dynamic()
        {
            var jsonString = "{\"Id\":1,\"Text\":\"Test\"}";

            dynamic sample = jsonString.JsonStringToObject();
            Assert.Equal(1, sample.Id);
            Assert.Equal("Test", sample.Text);
        }
    }

    public class TestSample1
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string Desc { get; set; }
    }

    public class TestSample2
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string Desc { get; set; }
        public DateTime StartDate { get; set; }
    }

    public class TestSample3
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string Desc { get; set; }
        public DateTime? StartDate { get; set; }
    }
}
