using System;
using System.ComponentModel.DataAnnotations;

namespace thehomebrewapi.ValidationAttributes
{
    public class TastingNoteDateIsValidAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            var tastingNoteDate = (DateTime)value;

            return !(tastingNoteDate == new DateTime(1, 1, 1, 0, 0, 0));
        }
    }
}
