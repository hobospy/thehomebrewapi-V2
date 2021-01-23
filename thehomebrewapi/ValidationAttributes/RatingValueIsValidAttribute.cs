using System.ComponentModel.DataAnnotations;

namespace thehomebrewapi.ValidationAttributes
{
    public class RatingValueIsValidAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            var rating = (double)value;

            return !(rating < 0 || rating > 5);
        }
    }
}
