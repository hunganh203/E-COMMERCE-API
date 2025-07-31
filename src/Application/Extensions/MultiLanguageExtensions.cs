using Application.Enums;

namespace Application.Extensions
{
    public static class MultiLanguageExtensions
    {
        /// <summary>
        /// T: Base default enum class (enum name without language prefix)
        /// Example: GetEnumByLanguage&lt;TaskType&gt;("Vi")
        /// </summary>
        public static IEnumerable<EnumOutputModel> GetEnumByLanguage<T>(string language)
        {
            return GetEnumByLanguage(typeof(T), language);
        }

        public static IEnumerable<EnumOutputModel> GetEnumByLanguage(Type objectType, string language)
        {
            var assemblyName = objectType.AssemblyQualifiedName;
            if (assemblyName != null)
            {
                var sourceClassName = objectType.Name;
                var desiredClassName = $"{sourceClassName}{language}";
                assemblyName = assemblyName.Replace(sourceClassName, desiredClassName);
                objectType = Type.GetType(assemblyName) ?? objectType;   //  If cannot get enum -> take the default
            }

            var result = EnumExtensions.ParserEnumObjects(objectType);

            return result;
        }
    }
}