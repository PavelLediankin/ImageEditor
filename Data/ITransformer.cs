using System.Drawing;
using MyPhotoshop.Parameters;

namespace MyPhotoshop.Data
{
    public interface ITransformer<TParameters>
        where TParameters: IParameters, new()
    {
        void Prepare(Size size, TParameters parameters);
        Size ResultSize { get; }
        Point? MapPoint(Point newPoint);
    }
}