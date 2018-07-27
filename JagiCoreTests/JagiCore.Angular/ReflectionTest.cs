using JagiCore.Angular;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Xunit;

namespace JagiCoreTests
{
    public class ReflectionTest
    {
        [Fact]
        public void Get_Class_Public_Properties()
        {
        //Given
            Type ti = typeof(VectorClass);
        //When
            var properties = ti.GetProperties(BindingFlags.Public | BindingFlags.Instance); 
        //Then
            Assert.Equal(6, properties.Length);
            Assert.True(properties.Any(p => p.Name == "Id"));
            Assert.True(properties.Any(p => p.Name == "FirstName"));
            Assert.True(properties.Any(p => p.Name == "Date"));

            Assert.True(properties.FirstOrDefault(p => p.Name == "Id").PropertyType == typeof(int));
            Assert.True(properties.FirstOrDefault(p => p.Name == "FirstName").PropertyType == typeof(string));
            Assert.True(properties.FirstOrDefault(p => p.Name == "Date").PropertyType == typeof(DateTime));
        }

        [Fact]
        public void Get_Property_String_Length()
        {
            var pi = typeof(VectorClass).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .FirstOrDefault(p => p.Name == "FirstName");
            var stringLength = pi.GetCustomAttributes().OfType<StringLengthAttribute>().FirstOrDefault();

            Assert.Equal(30, stringLength.MaximumLength);
            Assert.Equal(0, stringLength.MinimumLength);

            pi = typeof(VectorClass).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .FirstOrDefault(p => p.Name == "LastName");
            stringLength = pi.GetCustomAttributes().OfType<StringLengthAttribute>().FirstOrDefault();

            Assert.Equal(10, stringLength.MaximumLength);
            Assert.Equal(5, stringLength.MinimumLength);
        }

        [Fact]
        public void Get_Property_Display_Information()
        {
            var pi = typeof(VectorClass).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .FirstOrDefault(p => p.Name == "FirstName");
            var displayInfo = pi.GetCustomAttributes().OfType<DisplayAttribute>().FirstOrDefault();

            Assert.Equal("名字", displayInfo.Name);
            Assert.Equal("此為必須輸入的欄位，且不可以超過30個字", displayInfo.Description);
            Assert.Equal("輸入客戶名稱...", displayInfo.Prompt);

            pi = typeof(VectorClass).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .FirstOrDefault(p => p.Name == "LastName");
            displayInfo = pi.GetCustomAttributes().OfType<DisplayAttribute>().FirstOrDefault();

            Assert.Equal("姓氏", displayInfo.Name);
            Assert.Equal("輸入字數限制在 5 -10 個字", displayInfo.Description);
            Assert.Null(displayInfo.Prompt);
        }

        [Fact]
        public void Get_Property_Required_Information()
        {
            var pi = typeof(VectorClass).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .FirstOrDefault(p => p.Name == "FirstName");
            var requiredInfo = pi.GetCustomAttributes().OfType<RequiredAttribute>().FirstOrDefault();

            Assert.NotNull(requiredInfo);

            pi = typeof(VectorClass).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .FirstOrDefault(p => p.Name == "LastName");
            requiredInfo = pi.GetCustomAttributes().OfType<RequiredAttribute>().FirstOrDefault();

            Assert.Null(requiredInfo);
        }

        [Fact]
        public void Get_Property_Dropdown_Attribute()
        {
            var pi = typeof(VectorClass).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .FirstOrDefault(p => p.Name == "Code");
            var selctForAttr = pi.GetCustomAttributes().OfType<DropdownForAttribute>().FirstOrDefault();

            Assert.Equal("County", selctForAttr.CodeMap);
            Assert.Equal("Hospital", selctForAttr.CodeMapFor);

            pi = typeof(VectorClass).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .FirstOrDefault(p => p.Name == "ChildCode");
            var selectAttr = pi.GetCustomAttributes().OfType<DropdownAttribute>().FirstOrDefault();

            Assert.Equal("Hospital", selectAttr.CodeMap);
        }

        [Fact]
        public void Get_Property_Range_Attribute()
        {
            var pi = typeof(VectorClass).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .FirstOrDefault(p => p.Name == "Id");
            var dropdownAttr = pi.GetCustomAttributes().OfType<RangeAttribute>().FirstOrDefault();

            Assert.Equal(10, dropdownAttr.Minimum);
            Assert.Equal(20, dropdownAttr.Maximum);
        }
    }
}