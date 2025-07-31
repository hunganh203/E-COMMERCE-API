using AutoMapper;
using System.ComponentModel;

namespace Shared.Mappings
{
    public class UnmappedAttribute : Attribute
    {
    }

    public static class IgnoreNoMapExtensions
    {
        public static IMappingExpression<TSource, TDestination> IgnoreAllUnmapped<TSource, TDestination>(
            this IMappingExpression<TSource, TDestination> expression)
        {
            var sourceType = typeof(TSource);
            foreach (var property in sourceType.GetProperties())
            {
                var descriptor = TypeDescriptor.GetProperties(sourceType)[property.Name];
                var attribute = (UnmappedAttribute)descriptor?.Attributes[typeof(UnmappedAttribute)]!;
                expression.ForMember(property.Name, opt => opt.Ignore());
            }
            return expression;
        }
    }
}