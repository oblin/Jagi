using AutoMapper;

namespace JagiCore.Interfaces
{
    public interface IMapFromCustomized
    {
        void CreateMappings(IMapperConfigurationExpression configuration);
    }
}
