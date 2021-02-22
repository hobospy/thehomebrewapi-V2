using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using thehomebrewapi.Models;

namespace thehomebrewapi.Helpers
{
    public static class EnumExtensions
    {
        public static string ToDescriptionString<TEnum>(this TEnum @enum)
        {
            FieldInfo info = @enum.GetType().GetField(@enum.ToString());
            var attributes = (DescriptionAttribute[])info.GetCustomAttributes(typeof(DescriptionAttribute), false);

            return attributes?[0].Description ?? @enum.ToString();
        }

        public static TEnum ToEnumValueFromDescription<TEnum>(string descriptionValue) where TEnum : Enum
        {
            foreach(var field in typeof(TEnum).GetFields())
            {
                if (Attribute.GetCustomAttribute(field,
                    typeof(DescriptionAttribute)) is DescriptionAttribute attribute)
                {
                    if (attribute.Description == descriptionValue)
                    {
                        //return (TEnum)field.GetValue(null);
                        var value = (TEnum)field.GetValue(null);
                        return value;
                    }
                }
                else
                {
                    if (field.Name == descriptionValue)
                    {
                        return (TEnum)field.GetValue(null);
                    }
                }
            }

            throw new ArgumentException("Enum description not found.", nameof(descriptionValue));
        }

        public static IEnumerable<EnumDto> GetEnumValues<TEnum>()
        {
            var returnValue = new List<EnumDto>();
            foreach(var enumValue in Enum.GetValues(typeof(TEnum)))
            {
                returnValue.Add(new EnumDto()
                {
                    Name = Enum.GetName(typeof(TEnum), enumValue),
                    Description = enumValue.ToDescriptionString(),
                    Value = Convert.ToInt32(enumValue)
                });
            }

            return returnValue;
        }
    }
}
