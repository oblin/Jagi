using JagiCore.Helpers;
using JagiCoreTests.MultiSelectionSamples;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace JagiCoreTests
{
    public class MultiSelectionHelperTests
    {
        [Fact]
        public void String_To_Multiple_Boolean_Fields()
        {
            var source = new Model { Selection = "1-A" };
            var dest = new CorrectViewModel();

            dest.SetMultiBooleanProperty(() => source.Selection, source.Selection);

            Assert.True(dest.Selection1);
            Assert.False(dest.Selection2);
            Assert.True(dest.SelectionA);
            Assert.False(dest.SelectionB);
        }

        [Fact]
        public void ViewModel_Fields_Must_Be_Boolean_Type()
        {
            var source = new Model { Selection = "1-A" };
            var dest = new WrongViewModel();
            Assert.Throws<InvalidOperationException>(() =>
                dest.SetMultiBooleanProperty(() => source.Selection, source.Selection));
        }

        [Fact]
        public void Seperation_Must_Be_Dash()
        {
            var source = new Model { Selection = "1,A" };
            var dest = new CorrectViewModel();

            Assert.Throws<InvalidOperationException>(() =>
                dest.SetMultiBooleanProperty(() => source.Selection, source.Selection));
        }

        [Fact]
        public void Can_Accept_Trailing_Dash()
        {
            var source = new Model { Selection = "1-A-" };
            var dest = new CorrectViewModel();

            dest.SetMultiBooleanProperty(() => source.Selection, source.Selection);

            Assert.True(dest.Selection1);
            Assert.False(dest.Selection2);
            Assert.True(dest.SelectionA);
            Assert.False(dest.SelectionB);
        }

        [Fact]
        public void Compose_ViewModel_To_MultiSelection_Value()
        {
            var source = new CorrectViewModel
            {
                Selection1 = true,
                SelectionB = true
            };
            var dest = new Model();

            string result = source.ComposeSelection(() => dest.Selection);

            Assert.Equal("1-B", result);
        }

        [Fact]
        public void Model_And_ViewModel_Field_Name_Must_Begin_The_Same()
        {
            var source = new CorrectViewModel
            {
                Selection1 = true,
                SelectionB = true
            };
            var dest = new { Field = string.Empty};

            string result = source.ComposeSelection(() => dest.Field);

            Assert.True(string.IsNullOrEmpty(result));
        }

        [Fact]
        public void Model_And_ViewModel_Set_Value_By_The_Same_FieldName()
        {
            var source = new CorrectViewModel
            {
                Selection1 = true,
                SelectionB = true
            };
            var dest = new { Select = string.Empty };

            string result = source.ComposeSelection(() => dest.Select);

            Assert.Equal("ion1-ionB", result);
        }
    }
}
