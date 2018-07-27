using System;
using System.Linq;
using Xunit;
using JagiCore.Angular;

namespace JagiCoreTests
{
    public class ModelParserTests
    {
        [Fact]
        public void Model_Parser_Get_All_Properties()
        {
            var properties = ModelParser.CreateProperties(typeof(VectorClass));

            Assert.Equal(6, properties.Count);
            Assert.NotNull(properties.FirstOrDefault(p => p.Name == "FirstName"));

            var propertyRule = properties.FirstOrDefault(p => p.Name == "Date");
            Assert.NotNull(propertyRule);
            Assert.Null(properties.FirstOrDefault(p => p.Name == "NotFound"));
        }

        [Fact]
        public void Parse_Model_Input_Type()
        {
            var properties = ModelParser.CreateProperties(typeof(VectorClass));
            var propertyRule = properties.FirstOrDefault(p => p.Name == "Date");
            Assert.Equal(InputTag.Date, propertyRule.InputType);

            propertyRule = properties.FirstOrDefault(p => p.Name == "Id");
            Assert.Equal(InputTag.InputNumber, propertyRule.InputType);

            propertyRule = properties.FirstOrDefault(p => p.Name == "Code");
            Assert.Equal(InputTag.SelectFor, propertyRule.InputType);

            propertyRule = properties.FirstOrDefault(p => p.Name == "FirstName");
            Assert.Equal(InputTag.InputString, propertyRule.InputType);
        }

        [Fact]
        public void Parse_Model_With_Required_Validation()
        {
            var properties = ModelParser.CreateProperties(typeof(VectorClass));
            var propertyRule = properties.FirstOrDefault(p => p.Name == "FirstName");
            Assert.Equal(InputTag.InputString, propertyRule.InputType);

            var validations = propertyRule.Validations;
            Assert.NotNull(validations.FirstOrDefault(v => v.Type == ValidationType.Required));
            Assert.True(validations.FirstOrDefault(v => v.Type == ValidationType.Required).Message.Contains("【FirstName】必須要輸入"));
        }

        [Fact]
        public void Parse_Model_With_MaxLength_Validation()
        {
            var properties = ModelParser.CreateProperties(typeof(VectorClass));
            var propertyRule = properties.FirstOrDefault(p => p.Name == "FirstName");
            Assert.Equal(InputTag.InputString, propertyRule.InputType);

            var validations = propertyRule.Validations;
            Assert.NotNull(validations.FirstOrDefault(v => v.Type == ValidationType.Required));
            
            Assert.NotNull(validations.FirstOrDefault(v => v.Type == ValidationType.MaxLength));            
            Assert.True(validations.FirstOrDefault(v => v.Type == ValidationType.MaxLength).Message.Contains("30"));            
        }

        [Fact]
        public void Parse_Model_With_MinLength_With_MaxLength_Validation()
        {
            var properties = ModelParser.CreateProperties(typeof(VectorClass));
            var propertyRule = properties.FirstOrDefault(p => p.Name == "LastName");
            Assert.Equal(InputTag.InputString, propertyRule.InputType);

            var validations = propertyRule.Validations;
            Assert.Null(validations.FirstOrDefault(v => v.Type == ValidationType.Required));
            
            Assert.NotNull(validations.FirstOrDefault(v => v.Type == ValidationType.MinLength));            
            Assert.True(validations.FirstOrDefault(v => v.Type == ValidationType.MinLength).Message.Contains("5"));

            Assert.NotNull(validations.FirstOrDefault(v => v.Type == ValidationType.MaxLength));            
            Assert.True(validations.FirstOrDefault(v => v.Type == ValidationType.MaxLength).Message.Contains("10"));                        
        }

        [Fact]
        public void Parse_Model_With_Min_Max_Value()
        {
            var properties = ModelParser.CreateProperties(typeof(VectorClass));
            var propertyRule = properties.FirstOrDefault(p => p.Name == "Id");

            var validations = propertyRule.Validations;

            Assert.NotNull(validations.FirstOrDefault(v => v.Type == ValidationType.MinValue));
            Assert.True(validations.FirstOrDefault(v => v.Type == ValidationType.MinValue).Message.Contains("10"));

            Assert.NotNull(validations.FirstOrDefault(v => v.Type == ValidationType.MaxValue));
            Assert.True(validations.FirstOrDefault(v => v.Type == ValidationType.MaxValue).Message.Contains("20"));
        }

        [Fact]
        public void Check_Required_For_Primitive_And_DateTime()
        {
            var properties = ModelParser.CreateProperties(typeof(VectorClass));
            var propertyRule = properties.FirstOrDefault(p => p.Name == "Id");

            var validations = propertyRule.Validations;
            Assert.NotNull(validations.FirstOrDefault(v => v.Type == ValidationType.Required));

            propertyRule = properties.FirstOrDefault(p => p.Name == "Date");
            validations = propertyRule.Validations;
            Assert.NotNull(validations.FirstOrDefault(v => v.Type == ValidationType.Required));
        }

        [Fact]
        public void Check_Property_Display_Name()
        {
            var properties = ModelParser.CreateProperties(typeof(VectorClass));
            var propertyRule = properties.FirstOrDefault(p => p.Name == "FirstName");

            Assert.Equal("名字", propertyRule.DisplayName);

            propertyRule = properties.FirstOrDefault(p => p.Name == "LastName");
            Assert.Equal("姓氏", propertyRule.DisplayName);
        }

        [Fact]
        public void Check_Property_Prompt()
        {
            var properties = ModelParser.CreateProperties(typeof(VectorClass));
            var propertyRule = properties.FirstOrDefault(p => p.Name == "FirstName");

            Assert.Equal("輸入客戶名稱...", propertyRule.Prompt);
        }

        [Fact]
        public void Check_Property_Tooltip()
        {
            var properties = ModelParser.CreateProperties(typeof(VectorClass));
            var propertyRule = properties.FirstOrDefault(p => p.Name == "FirstName");

            Assert.Equal("此為必須輸入的欄位，且不可以超過30個字", propertyRule.Tooltip);
            Assert.NotEqual("輸入客戶名稱...", propertyRule.Tooltip);
        }

        [Fact]
        public void Not_Generate_Hidden_Field()
        {
            var properties = ModelParser.CreateProperties(typeof(VectorClass2));

            Assert.Null(properties.FirstOrDefault(p => p.Name == "Id"));
        }
    }
}