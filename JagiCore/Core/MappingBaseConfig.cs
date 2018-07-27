using AutoMapper;
using AutoMapper.Configuration;
using JagiCore.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace JagiCore
{
    /// <summary>
    /// 可以直接使用，或者使用以下方式客製 Converter: 
    ///     class MapperConfig : MappingBaseConfig
    ///     {
    ///         public MapperConfig(Assembly assembly) : base(assembly)
    ///         {
    ///     
    ///         }
    ///     
    ///         public override void ConverterSettings()
    ///         {
    ///             Mapper.CreateMap<DateTime, string>().ConvertUsing<DateTimeToString>();
    ///     
    ///             base.ConverterSettings();
    ///         }
    ///     }
    /// </summary>
    public class MappingBaseConfig
    {
        private readonly List<Assembly> _assemblies;

        /// <summary>
        /// 傳入執行的 Assembly 呼叫方式：
        ///    Assembly assembly = Assembly.GetExecutingAssembly();
        ///    MappingBaseConfig config = new MappingBaseConfig(assembly);
        /// </summary>
        /// <param name="assembly"></param>
        public MappingBaseConfig(Assembly assembly)
        {
            _assemblies = new List<Assembly> { assembly };
        }

        public MappingBaseConfig(List<Assembly> assemblies)
        {
            _assemblies = assemblies;
        }

        /// <summary>
        /// 提供基本的設定： string to decimal?, string to DateTime? and int? to string
        /// 如果有需要其他的 Converter 可以 Override 此函數，自行設定。
        /// </summary>
        public virtual void ConverterSettings()
        {
            Mapper.Initialize(config =>
            {
                config.CreateMap<string, decimal?>().ConvertUsing<StringToDecimalNull>();
                config.CreateMap<string, DateTime?>().ConvertUsing<StringToDateTimeNull>();
                config.CreateMap<int?, string>().ConvertUsing<NullableIntToString>();
                config.CreateMap<int, string>().ConvertUsing<IntToString>();
                config.CreateMap<string, int>().ConvertUsing<StringToInt>();
                config.CreateMap<DateTime?, string>().ConvertUsing<DateTimeNullToString>();
                config.CreateMap<DateTime, string>().ConvertUsing<DateTimeToString>();
            });
        }

        /// <summary>
        /// 執行 AutoMapper 的設定，讓 IMapFrom & IMapCustomized 可以使用
        /// 請注意，如果有需要設定 Converter，可以 Override ConverterSetting()
        /// 請注意，只能被呼叫一次，否則就會出現 Already Initialized 錯誤
        /// </summary>
        public virtual void Execute()
        {
            ConverterSettings();

            var config = new MapperConfigurationExpression();
            foreach(var assembly in _assemblies)
                SetOneAssembly(assembly, config);

            Mapper.Initialize(config);
        }


        /// <summary>
        /// 提供多次重新建立 Automapper 的方案
        /// </summary>
        /// <returns></returns>
        public virtual IMapper GetNewMapper()
        {
            ConverterSettings();

            var config = new MapperConfigurationExpression();
            foreach (var assembly in _assemblies)
                SetOneAssembly(assembly, config);

            Mapper.Reset();
            return (new MapperConfiguration(config)).CreateMapper();
        }

        private void SetOneAssembly(Assembly assembly, MapperConfigurationExpression config)
        {
            Type[] types;
            if (assembly == null)
                types = Assembly.GetEntryAssembly().GetExportedTypes();
            else
                types = assembly.GetExportedTypes();

            LoadStarndardMappings(types, config);
            LoadCustomMappings(types, config);
        }

        private void LoadCustomMappings(IEnumerable<Type> types, IMapperConfigurationExpression config)
        {
            var maps = (from t in types
                        from i in t.GetInterfaces()
                        where typeof(IMapFromCustomized).IsAssignableFrom(t) &&
                            !t.GetTypeInfo().IsAbstract && !t.GetTypeInfo().IsInterface
                        select (IMapFromCustomized)Activator.CreateInstance(t)).ToArray();

            foreach (var map in maps)
                map.CreateMappings(config);
        }

        private void LoadStarndardMappings(IEnumerable<Type> types, IMapperConfigurationExpression config)
        {
            var maps = (from t in types
                        from i in t.GetInterfaces()
                        where i.GetTypeInfo().IsGenericType &&
                              i.GetGenericTypeDefinition() == typeof(IMapFrom<>) &&
                              !t.GetTypeInfo().IsAbstract && !t.GetTypeInfo().IsInterface
                        select new
                        {
                            Source = i.GetGenericArguments()[0],
                            Destination = t
                        }).ToArray();

            foreach (var map in maps)
                config.CreateMap(map.Source, map.Destination);
        }
    }
}
