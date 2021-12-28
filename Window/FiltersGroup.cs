using System.Collections.Generic;
using System.Linq;
using MyPhotoshop.Filters;

namespace MyPhotoshop.Window
{
    public class FiltersGroup
    {
        public List<IFilter> Filters { get; }
        public readonly string Name;

        public FiltersGroup(IEnumerable<IFilter> filters, string name)
        {
            Name = name;
            Filters = filters.ToList();
        }
    }
}