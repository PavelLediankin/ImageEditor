using MyPhotoshop.Data;
using MyPhotoshop.Filters;

namespace MyPhotoshop.Window
{
    public interface IProcessor
    {
        Photo ProcessPhoto(IFilter filter, Photo photo, double[] parameter);
    }
}