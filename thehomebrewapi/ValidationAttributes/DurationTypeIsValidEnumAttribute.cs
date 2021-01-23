using System;
using System.ComponentModel.DataAnnotations;
using static thehomebrewapi.Entities.Enumerations;

namespace thehomebrewapi.ValidationAttributes
{
    public class DurationTypeIsValidEnumAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            var duration = (int)value;

            return (Enum.IsDefined(typeof(ETypeOfDuration), duration));
        }
    }
}
