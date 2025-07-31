using Application.Enums;
using System.ComponentModel;

namespace Application.Extensions
{
    public static class EnumExtensions
    {
        public static string GetEnumDescription(this Enum enumValue)
        {
            var fieldInfo = enumValue.GetType().GetField(enumValue.ToString());

            if (fieldInfo == null)
            {
                return "Enum data does not exist";
            }
            var descriptionAttributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

            return descriptionAttributes.Length > 0 ? descriptionAttributes[0].Description : enumValue.ToString();
        }

        public static string GetTextEnumDescription(Enum value)
        {
            return GetTextEnumDescription(value.GetType(), value.ToString());
        }

        public static string GetTextEnumDescription(Type type, string value)
        {
            var fi = type.GetField(value);

            if (fi == null)
            {
                return "";
            }
            if (fi.GetCustomAttributes(typeof(DescriptionAttribute), false)
                is DescriptionAttribute[] attributes && attributes.Any())
            {
                return attributes.First().Description;
            }

            return value.ToString();
        }

        /// <summary>
        /// Parser enum to return to client, param increaseKeyBy is used for the enum start with 1, not 0
        /// </summary>
        public static IEnumerable<EnumOutputModel> ParserEnumObjects<T>()
        {
            var results = ParserEnumObjects(typeof(T));
            return results;
        }

        public static IEnumerable<EnumOutputModel> ParserEnumObjects(Type objType)
        {
            var results = Enum.GetNames(objType).Select((value, key)
                => new EnumOutputModel
                {
                    Value = value,
                    Key = (int)Enum.Parse(objType, value, true),
                    Text = GetTextEnumDescription(objType, value)
                });
            return results;
        }
    }
}