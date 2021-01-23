using System;
using System.ComponentModel.DataAnnotations;
using static thehomebrewapi.Entities.Enumerations;

namespace thehomebrewapi.ValidationAttributes
{
    public class BeerTypeIsValidEnumAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            var beerType = (int)value;

            return Enum.IsDefined(typeof(ETypeOfBeer), beerType);
        }
    }
}
