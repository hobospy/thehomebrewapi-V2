﻿using System;
using System.Collections.Generic;
using System.Linq;
using thehomebrewapi.Entities;
using thehomebrewapi.Models;

namespace thehomebrewapi.Services
{
    public class PropertyMappingService : IPropertyMappingService
    {
        private Dictionary<string, PropertyMappingValue> _recipePropertyMapping =
            new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
            {
                { "Id", new PropertyMappingValue(new List<string>() { "Id" } ) },
                { "Name", new PropertyMappingValue(new List<string>() { "Name" } ) },
                { "Type", new PropertyMappingValue(new List<string>() { "Type" } ) },
                { "ExpectedABV", new PropertyMappingValue(new List<string>() { "ExpectedABV" } ) },
                { "Favourite", new PropertyMappingValue(new List<string>() { "Favourite" } ) }
            };

        private IList<IPropertyMapping> _propertyMappings = new List<IPropertyMapping>();

        public PropertyMappingService()
        {
            _propertyMappings.Add(new PropertyMapping<RecipeDto, Recipe>(_recipePropertyMapping));
        }

        public bool ValidMappingExistsFor<TSource, TDestination>(string fields)
        {
            if (string.IsNullOrWhiteSpace(fields))
            {
                return true;
            }

            var propertyMapping = GetPropertyMapping<TSource, TDestination>();
            var fieldsAfterSplit = fields.Split(',');

            foreach(var field in fieldsAfterSplit)
            {
                var trimmedField = field.Trim();

                var indexOfFirstSpace = trimmedField.IndexOf(" ");
                var propertyName = indexOfFirstSpace == -1 ?
                    trimmedField : trimmedField.Remove(indexOfFirstSpace);

                if(!propertyMapping.ContainsKey(propertyName))
                {
                    return false;
                }
            }

            return true;
        }

        public Dictionary<string, PropertyMappingValue> GetPropertyMapping
            <TSource, TDesination>()
        {
            // Get matching mapping
            var matchingMapping = _propertyMappings.OfType<PropertyMapping<TSource, TDesination>>();

            if (matchingMapping.Count() == 1)
            {
                return matchingMapping.First()._mappingDictionary;
            }

            throw new Exception($"Cannot find exact property mapping instance " +
                $"for <{typeof(TSource)}, {(typeof(TDesination))}>");
        }
    }
}
