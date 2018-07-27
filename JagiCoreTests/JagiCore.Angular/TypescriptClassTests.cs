using JagiCore.Angular;
using Xunit;

namespace JagiCoreTests
{
    public class TypescriptClassTests
    {
        [Fact]
        public void Test_Create_Typescript_Class_By_VectorClass()
        {
        //Given
            var type = typeof(VectorClass);
        
        //When
            var strClass = ModelParser.CreateTypescriptClass(type);
            string[] classes = strClass.Split('\n');

        //Then
            Assert.Equal("export class VectorClass {", classes[0]);
            Assert.Equal("  constructor(", classes[1]);
            Assert.Equal("     public Id: number,", classes[2]);
            Assert.Equal("     public FirstName: string,", classes[3]);
            Assert.Equal("     public LastName: string,", classes[4]);
            Assert.Equal("     public Code: string,", classes[5]);
            Assert.Equal("     public ChildCode: string,", classes[6]);
            Assert.Equal("     public Date: string", classes[7]);
            Assert.Equal(") { }", classes[8]);
            Assert.Equal("}", classes[9]);
        }

        [Fact]
        public void Test_Create_Typescript_Class_By_VectorClass2()
        {
        //Given
            var type = typeof(VectorClass2);
        
        //When
            var strClass = ModelParser.CreateTypescriptClass(type);
            string[] classes = strClass.Split('\n');

        //Then
            Assert.Equal("export class VectorClass2 {", classes[0]);
            Assert.Equal("  constructor(", classes[1]);
            Assert.Equal("     public Check: boolean,", classes[2]);
            Assert.Equal("     public Radio: string,", classes[3]);
            Assert.Equal("     public PreferRadioStyle: string", classes[4]);
        }
    }
}