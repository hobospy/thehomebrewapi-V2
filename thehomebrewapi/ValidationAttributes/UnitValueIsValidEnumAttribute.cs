using System;
using System.ComponentModel.DataAnnotations;
using static thehomebrewapi.Entities.Enumerations;

namespace thehomebrewapi.ValidationAttributes
{
    public class UnitValueIsValidEnumAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            var unit = (int)value;

            return Enum.IsDefined(typeof(EUnitOfMeasure), unit);
        }
    }
}
