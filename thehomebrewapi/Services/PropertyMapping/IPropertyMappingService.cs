using System.Collections.Generic;

namespace thehomebrewapi.Services
{
    public interface IPropertyMappingService
    {
        Dictionary<string, PropertyMappingValue> GetPropertyMapping<TSource, TDesination>();
        bool ValidMappingExistsFor<TSource, TDestination>(string fields);
    }
}