using System;
using System.Drawing;
using MyPhotoshop.Parameters;

namespace MyPhotoshop.Filters
{
    public class TransformFilter : TransformFilter<EmptyParameters>
    {

        public TransformFilter(string name,
            Func<Size, Size> sizeTransform, Func<Point, Size, Point> pointTransform)
            : base(name, new FreeTransformer(sizeTransform, pointTransform))
        {
        }
    }
}
