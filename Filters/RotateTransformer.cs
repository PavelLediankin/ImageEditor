using System;
using System.Drawing;
using MyPhotoshop.Data;
using MyPhotoshop.Parameters;

namespace MyPhotoshop.Filters
{
    public class RotateTransformer : ITransformer<RotationParameters>
    {
        public Size ResultSize { get; private set; }
        public Size OriginalSize { get; private set; }
        public double Angle { get; private set; }

        public void Prepare(Size size, RotationParameters parameters)
        {
            OriginalSize = size;
            Angle = Math.PI * parameters.Angle / 180;
            ResultSize = new Size(
                (int)(size.Width * Math.Abs(Math.Cos(Angle)) + size.Height * Math.Abs(Math.Sin(Angle))),
                (int)(size.Height * Math.Abs(Math.Cos(Angle)) + size.Width * Math.Abs(Math.Sin(Angle))));

        }

        public Point? MapPoint(Point point)
        {
            point = new Point(point.X - ResultSize.Width / 2, point.Y - ResultSize.Height / 2);
            var x = OriginalSize.Width / 2 + (int)(point.X * Math.Cos(Angle) + point.Y * Math.Sin(Angle));
            var y = OriginalSize.Height / 2 + (int)(-point.X * Math.Sin(Angle) + point.Y * Math.Cos(Angle));
            if (x < 0 || x >= OriginalSize.Width ||
                y < 0 || y >= OriginalSize.Height)
                return null;
            return new Point(x, y);
        }
    }
}