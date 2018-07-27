using AutoMapper;
using JagiCore;
using JagiCore.Interfaces;
using Microsoft.Extensions.DependencyModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace JagiCoreTests.MapTest
{
    public class MapperTests
    {
        [Fact]
        public void Can_Use_IMapFrom_Interface_For_Defined_Config()
        {
            // Get Current Executed Assembly            
            Assembly assembly = typeof(MapDest1).GetTypeInfo().Assembly;
            var config = new MappingBaseConfig(assembly);
            var mapper = config.GetNewMapper();

            var source = new MapSource { Id = 1, Date = new DateTime(1995, 5, 12), Name = "OK" };

            var dest = mapper.Map<MapDest1>(source);

            Assert.Equal(1, dest.Id);
            Assert.Equal("1995-5-12", dest.Date.ToString("yyyy-M-dd"));
        }

        [Fact]
        public void Can_Use_IMapFrom_Interface_For_Multiple_Items()
        {
            Assembly assembly = this.GetType().GetTypeInfo().Assembly;
            var config = new MappingBaseConfig(assembly);
            var mapper = config.GetNewMapper();

            var source = new List<MapSource> {
                new MapSource { Id = 1, Date = new DateTime(1995, 5, 12), Name = "OK" },
                new MapSource { Id = 2, Date = new DateTime(2005, 5, 12), Name = "NotOK" },
                new MapSource { Id = 3, Date = new DateTime(2015, 5, 12), Name = "OK" }
            };
            var dest = mapper.Map<IEnumerable<MapDest1>>(source);

            Assert.Equal(1, dest.First().Id);
            Assert.Equal("1995-5-12", dest.FirstOrDefault(p => p.Id == 1).Date.ToString("yyyy-M-dd"));
        }

        [Fact]
        public void Can_Use_ICustomFrom_Interface_For_Defined_Config()
        {
            // Get Current Executed Assembly
            Assembly assembly = typeof(MapperTests).GetTypeInfo().Assembly;
            var config = new MappingBaseConfig(assembly);
            var mapper = config.GetNewMapper();

            var source = new MapSource { Id = 1, Date = new DateTime(1995, 5, 12), Name = "name" };

            var dest = mapper.Map<MapDest2>(source);

            Assert.Equal("NAME", dest.FirstName);
            Assert.Equal("1995-5-14", dest.AddedDate.ToString("yyyy-M-dd"));
        }

        [Fact]
        public void Can_Use_ICustomFrom_Interface_For_Multiiples()
        {
            // Get Current Executed Assembly
            Assembly assembly = typeof(MapperTests).GetTypeInfo().Assembly;
            var config = new MappingBaseConfig(assembly);
            var mapper = config.GetNewMapper();

            var source = new List<MapSource> {
                new MapSource { Id = 1, Date = new DateTime(1995, 5, 12), Name = "name" },
                new MapSource { Id = 2, Date = new DateTime(2005, 5, 5), Name = "user" }
                };

            var dest = mapper.Map<List<MapDest2>>(source);

            Assert.Equal("USER", dest.Last().FirstName);
            Assert.Equal("2005-5-07", dest.FirstOrDefault(p => p.Id == 2).AddedDate.ToString("yyyy-M-dd"));
        }

        [Fact]
        public void Map_CodeFile_Codes()
        {
            Assembly assembly = typeof(CodeFile).GetTypeInfo().Assembly;
            var config = new MappingBaseConfig(assembly);
            var mapper = config.GetNewMapper();

            var codes = CodeSample.FakeCodes().Value.ToList();
            codes[0].Description = "Description";
            codes[0].Remark = "Remark";
            codes[0].TypeName = "Name";
            codes[0].ParentType = "Test";

            var result = mapper.Map<IEnumerable<Code>>(codes);

            Assert.Equal(2, result.Count());
            Assert.Equal("County", result.First().ItemType);
            Assert.Equal("Hospital", result.Last().ItemType);
        }

        [Fact]
        public void Map_With_Multiple_Assemblies()
        {
            List<Assembly> loadableAssemblies = new List<Assembly>();
            var deps = DependencyContext.Default;
            foreach (var compilationLibrary in deps.CompileLibraries)
            {
                if (compilationLibrary.Name.Equals("JagiCore", StringComparison.OrdinalIgnoreCase))
                {
                    var assembly = Assembly.Load(new AssemblyName(compilationLibrary.Name));
                    loadableAssemblies.Add(assembly);
                }
            }

            var config = new MappingBaseConfig(loadableAssemblies);
            var mapper = config.GetNewMapper();

            var codes = CodeSample.FakeCodes().Value.ToList();
            codes[0].Description = "Description";
            codes[0].Remark = "Remark";
            codes[0].TypeName = "Name";
            codes[0].ParentType = "Test";

            var result = mapper.Map<IEnumerable<Code>>(codes);

            Assert.Equal(2, result.Count());
            Assert.Equal("County", result.First().ItemType);
            Assert.Equal("Hospital", result.Last().ItemType);
        }

    }
}
