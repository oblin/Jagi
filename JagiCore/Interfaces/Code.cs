using System;
using System.Collections.Generic;
using AutoMapper;
using JagiCore.Helpers;

namespace JagiCore.Interfaces
{
    /// <summary>
    /// 提供 CodeFile & CodeDetail 的扁平化物件
    /// ItemType + ParentCode + ItemCode 都可能會是 key
    /// </summary>
    public class Code : IMapFromCustomized
    {
        /// <summary>
        /// 此欄位可以忽略，僅作用在 CodesController 中
        /// </summary>
        public int Id { get; set; }
        public string ItemType { get; set; }
        public string Description { get; set; }
        public string ParentType { get; set; }
        public string ParentCode { get; set; }
        public string ItemCode { get; set; }

        public void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<CodeFile, Code>()
                .ForMember(d => d.Description, opt => opt.MapFrom(s => s.TypeName))
                .ForMember(d => d.ItemCode, opt => opt.Ignore())
                //.ForSourceMember(s => s.Remark, opt => opt.Ignore())
                .ForSourceMember(s => s.CodeDetails, opt => opt.Ignore());
        }
    }
}
