using System;
using System.Drawing;
using System.Windows.Forms;
using MyPhotoshop.Data;
using MyPhotoshop.Filters;
using MyPhotoshop.Parameters;
using Ninject;

namespace MyPhotoshop
{
    public static class FilterRegistrator
    {
        public static void Register(StandardKernel container)
        {
            RegisterParametrizedFilters(container);
            RegisterTransformFilters(container);
            RegisterMatrixFilters(container);
        }

        private static void RegisterParametrizedFilters(StandardKernel container)
        {
            container.Bind<IFilter>().ToConstant(new PixelFilter<LighteningParameters>(
                "Осветление/затемнение",
                (original, parameters) => original * parameters.Coeficient
                ));
            container.Bind<IFilter>().ToConstant(new PixelFilter<EmptyParameters>(
                "Оттенки серого",
                (original, ps) =>
                {
                    var lightness = 0.2126 * original.R + 0.7152 * original.G + 0.0722 * original.B;
                    return new Pixel(lightness, lightness, lightness);
                }
            ));
        }

        private static void RegisterTransformFilters(StandardKernel container)
        {
            container.Bind<IFilter>().ToConstant(new TransformFilter(
                "Отражение по горизонтали",
                (size) => size,
                (pt, size) => new Point(size.Width - pt.X - 1, pt.Y)
            ));
            container.Bind<IFilter>().ToConstant(new TransformFilter(
                "Поворот против ч.с.",
                (size) => new Size(size.Height, size.Width),
                (pt, size) => new Point(pt.Y, pt.X)
            ));

            container.Bind<IFilter>().ToConstant(new TransformFilter<RotationParameters>(
                "Свободное вращение", new RotateTransformer()));
        }

        private static void RegisterMatrixFilters(StandardKernel container)
        {
            var sharpness = GetSimpleMatrix(10, -1, 3);
            container.Bind<IFilter>().ToConstant(new MatrixFilter(
                "Улучшение четкости",
                sharpness));

            var blur = GetSimpleMatrix(1, 1, 3);
            container.Bind<IFilter>().ToConstant(new MatrixFilter(
                "Размытие",
                blur));

            var emboss = GetGradientMatrix(-2, 1, 3);
            emboss[1, 1] = 1;
            container.Bind<IFilter>().ToConstant(new MatrixFilter(
                "Глубина",
                emboss));

            var lightSharpness = new Matrix(3);
            lightSharpness[0, 0] = -1;
            lightSharpness[1, 1] = 2;
            container.Bind<IFilter>().ToConstant(new MatrixFilter(
                "Легкое улучшение четкости",
                lightSharpness));

            var lightBlur = GetSimpleMatrix(1, 1, 3,
                (i, j) => i == 2 || j == 2);
            container.Bind<IFilter>().ToConstant(new MatrixFilter(
                "Легкое размытие",
                lightBlur));

            var lightEmboss = new Matrix(3);
            lightEmboss[0, 0] = 1;
            lightEmboss[1, 1] = 1;
            lightEmboss[2, 2] = -1;
            container.Bind<IFilter>().ToConstant(new MatrixFilter(
                "Легкая глубина",
                lightEmboss));

            var horrible = GetGradientMatrix(-2, 1, 3);
            container.Bind<IFilter>().ToConstant(new MatrixFilter(
                "Ужас",
                horrible));
        }

        private static Matrix GetSimpleMatrix(double center, double other, int length,
            Func<int, int, bool> emptyPositionsSelector = null)
        {
            var matrix = new Matrix(length);
            for (var i = 0; i < length; i++)
            for (var j = 0; j < length; j++)
            {
                if(emptyPositionsSelector != null && emptyPositionsSelector(i, j))
                    continue;
                matrix[i, j] = other;
            }
            matrix[1, 1] = center;
            return matrix;
        }

        private static Matrix GetGradientMatrix(double start, double delta, int length,
            Func<int, int, bool> emptyPositionsSelector = null)
        {
            var matrix = new Matrix(length);
            for (var i = 0; i < length; i++)
            for (var j = 0; j < length; j++)
            {
                if (emptyPositionsSelector != null && emptyPositionsSelector(i, j))
                    continue;
                matrix[i, j] = start + delta *(i + j);
            }
            return matrix;
        }
    }
}