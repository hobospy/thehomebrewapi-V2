using System;
using System.Collections.Generic;
using System.Linq;
using thehomebrewapi.Entities;
using thehomebrewapi.Models;

namespace thehomebrewapi.Services
{
    public class PropertyMappingService : IPropertyMappingService
    {
        // Example used to explain purpose of mapping
        //private Dictionary<string, PropertyMappingValue> _authorPropertyMapping =
        //  new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
        //  {
        //       { "Id", new PropertyMappingValue(new List<string>() { "Id" } ) },
        //       { "MainCategory", new PropertyMappingValue(new List<string>() { "MainCategory" } )},
        //       { "Age", new PropertyMappingValue(new List<string>() { "DateOfBirth" } , true) },
        //       { "Name", new PropertyMappingValue(new List<string>() { "FirstName", "LastName" }) }
        //  };

        private Dictionary<string, PropertyMappingValue> _recipePropertyMapping =
            new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
            {
                { "Id", new PropertyMappingValue(new List<string>() { "Id" } ) },
                { "Name", new PropertyMappingValue(new List<string>() { "Name" } ) },
                { "Description", new PropertyMappingValue(new List<string>() { "Description" } ) },
                { "Type", new PropertyMappingValue(new List<string>() { "Type" } ) },
                { "ExpectedABV", new PropertyMappingValue(new List<string>() { "ExpectedABV" } ) },
                { "Favourite", new PropertyMappingValue(new List<string>() { "Favourite" } ) }
            };
        private Dictionary<string, PropertyMappingValue> _brewPropertyMapping =
            new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
            {
                { "Id", new PropertyMappingValue(new List<string>() { "Id" } ) },
                { "Name", new PropertyMappingValue(new List<string>() { "Name" } ) },
                { "BrewDate", new PropertyMappingValue(new List<string>() { "BrewDate" } ) },
                { "BrewedState", new PropertyMappingValue(new List<string>() { "BrewedState" } ) },
                { "BrewingNotes", new PropertyMappingValue(new List<string>() { "BrewingNotes" } ) },
                { "ABV", new PropertyMappingValue(new List<string>() { "ABV" } ) },
                { "Rating", new PropertyMappingValue(new List<string>() { "Rating" } ) }
            };
        private Dictionary<string, PropertyMappingValue> _waterProfilePropertyMapping =
            new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
            {
                { "Id", new PropertyMappingValue(new List<string>() { "Id" } ) },
                { "Name", new PropertyMappingValue(new List<string>() { "Name" } ) },
                { "Description", new PropertyMappingValue(new List<string>() { "Description" } ) }
            };
        private Dictionary<string, PropertyMappingValue> _tastingNotePropertyMapping =
            new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
            {
                { "Id", new PropertyMappingValue(new List<string>() { "Id" } ) },
                { "Note", new PropertyMappingValue(new List<string>() { "Note" } ) },
                { "Date", new PropertyMappingValue(new List<string>() { "Date" } ) }
            };

        private IList<IPropertyMapping> _propertyMappings = new List<IPropertyMapping>();

        public PropertyMappingService()
        {
            _propertyMappings.Add(new PropertyMapping<RecipeDto, Recipe>(_recipePropertyMapping));
            _propertyMappings.Add(new PropertyMapping<BrewDto, Brew>(_brewPropertyMapping));
            _propertyMappings.Add(new PropertyMapping<WaterProfileDto, WaterProfile>(_waterProfilePropertyMapping));
            _propertyMappings.Add(new PropertyMapping<TastingNoteDto, TastingNote>(_tastingNotePropertyMapping));
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
