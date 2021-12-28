using System;
using MyPhotoshop.Data;

namespace MyPhotoshop.Extensions
{
    public static class MatrixExtensions
    {
        public static Pixel UseMatrix(this Matrix matrix, Pixel[,] pixels)
        {
            var size = pixels.GetLength(0);
            if (size != pixels.GetLength(1))
                throw new ArgumentException("Pixels should be square matrix");
            if (size != matrix.Size)
                throw new ArgumentException("Matrix and Pixels should have same sizes");

            var pixel = new UnsafePixel();
            for (var i = 0; i < size; i++)
            for (var j = 0; j < size; j++)
                pixel += UnsafePixel.Multiply(pixels[i, j], matrix[i, j]);
            var normingCoefficient = matrix.GetNormingCoefficient();
            return pixel.ToSafe(normingCoefficient);
        }

        public static Photo ProcessByMatrix
            (this Matrix matrix, Photo tempPhoto, int sizeAdding)
        {
            var result = new Photo(tempPhoto.Width, tempPhoto.Height);
            for (var x = sizeAdding; x < tempPhoto.Width - sizeAdding; x++)
            for (var y = sizeAdding; y < tempPhoto.Height - sizeAdding; y++)
            {
                var pixels = matrix.GetPixelsMatrixFromPhoto(tempPhoto, sizeAdding, x, y);
                result[x, y] = matrix.UseMatrix(pixels);
            }
            return result;
        }

        public static Pixel[,] GetPixelsMatrixFromPhoto
            (this Matrix matrix, Photo tempPhoto, int sizeAdding, int x, int y)
        {
            var size = matrix.Size;
            var pixels = new Pixel[size, size];
            for (var i = 0; i < size; i++)
            for (var k = 0; k < size; k++)
                pixels[i, k] = tempPhoto[i + x - sizeAdding, k + y - sizeAdding];
            return pixels;
        }

        private class UnsafePixel
        {
            private readonly double R;
            private readonly double G;
            private readonly double B;

            public UnsafePixel()
            {
            }

            private UnsafePixel(double R, double G, double B)
            {
                this.R = R;
                this.G = G;
                this.B = B;
            }

            public Pixel ToSafe(double normCoef)
                => new Pixel(
                    Normalize(R, normCoef),
                    Normalize(G, normCoef),
                    Normalize(B, normCoef));

            private static double Normalize(double channel, double normCoef)
                => Pixel.Trim(channel / normCoef);

            public static UnsafePixel Multiply(Pixel px, double k)
                => new UnsafePixel(px.R * k, px.G * k, px.B * k);

            public static UnsafePixel operator +(UnsafePixel px1, UnsafePixel px2)
                => new UnsafePixel(px1.R + px2.R, px1.G + px2.G, px1.B + px2.B);

            public static UnsafePixel operator +(UnsafePixel px1, Pixel px2)
                => px1 + new UnsafePixel(px2.R, px2.G, px2.B);

            public static UnsafePixel operator +(Pixel px1, UnsafePixel px2)
                => px2 + px1;
        }
    }
}