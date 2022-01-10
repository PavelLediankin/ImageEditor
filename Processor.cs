using MyPhotoshop.Data;
using MyPhotoshop.Filters;

namespace MyPhotoshop.Window
{
    public class Processor : IProcessor
    {
        public Photo ProcessPhoto(IFilter filter, Photo photo, double[] parameter)
        {
            return filter.Process(photo, parameter);
        }
    }
}