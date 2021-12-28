using System;
using System.Drawing;
using MyPhotoshop.Data;
using MyPhotoshop.Parameters;

namespace MyPhotoshop.Filters
{
    public class FreeTransformer : ITransformer<EmptyParameters>
    {
        public Size ResultSize { get; private set; }

        private Func<Size, Size> sizeTransform;
        private Func<Point, Size, Point> pointTransform;
        private Size oldSize;

        public FreeTransformer(Func<Size, Size> sizeTransform, Func<Point, Size, Point> pointTransform)
        {
            this.sizeTransform = sizeTransform;
            this.pointTransform = pointTransform;
        }

        public void Prepare(Size size, EmptyParameters parameters)
        {
            oldSize = size;
            ResultSize = sizeTransform(size);
        }

        public Point? MapPoint(Point newPoint)
        {
            return pointTransform(newPoint, oldSize);
        }
    }
}