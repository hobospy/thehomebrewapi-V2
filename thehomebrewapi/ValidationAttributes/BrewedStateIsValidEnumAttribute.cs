using System;
using System.ComponentModel.DataAnnotations;
using static thehomebrewapi.Entities.Enumerations;

namespace thehomebrewapi.ValidationAttributes
{
    public class BrewedStateIsValidEnumAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            var brewedState = (int)value;

            return Enum.IsDefined(typeof(EBrewedState), brewedState);
        }
    }
}
