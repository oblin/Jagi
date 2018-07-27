using JagiCore.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;

namespace JagiCoreTests.MapTest
{
    public class MapSource
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
    }

    public class MapDest1 : IMapFrom<MapSource>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
    }

    public class MapDest2 : IMapFromCustomized
    {
        public int Id { get; set; }
        /// <summary>
        /// 對應 Name 並且改為大寫
        /// </summary>
        public string FirstName { get; set; }
        /// <summary>
        /// 對應 Date 且原始日期加兩天
        /// </summary>
        public DateTime AddedDate { get; set; }

        public void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<MapSource, MapDest2>()
                .ForMember(d => d.FirstName, map => map.MapFrom(s => s.Name.ToUpper()))
                .ForMember(d => d.AddedDate, map => map.MapFrom(s => s.Date.AddDays(2)));
        }
    }
}
