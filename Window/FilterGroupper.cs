using System.Collections.Generic;
using System.Linq;
using MyPhotoshop.Filters;

namespace MyPhotoshop.Window
{
    public static class FilterGroupper
    {
        public static IEnumerable<FiltersGroup> GetGrouppedFilters(IEnumerable<IFilter> filters)
        {
            return filters.GroupBy(f =>
                {
                    var type = f.GetType();
                    if (type.IsGenericType)
                        return type.GetGenericTypeDefinition();
                    return !type.Name.Contains(nameof(MatrixFilter)) ?
                        type.BaseType.GetGenericTypeDefinition() :
                        type;
                })
                .Select(group => new FiltersGroup(group, group.Key.Name));
        }
    }
}