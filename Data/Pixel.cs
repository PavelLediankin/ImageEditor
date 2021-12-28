using System;

namespace MyPhotoshop.Data
{
    public struct Pixel
    {
        private double r;
        private double g;
        private double b;

        public double R 
        { 
            get => r;
            set => r = CheckValue(value);
        }

        public double G 
        {
            get => g;
            set => g = CheckValue(value);
        }

        public double B 
        {
            get => b;
            set => b = CheckValue(value);
        }

        public Pixel(double R , double G, double B)
        {
            r = g = b = 0;
            this.R = R;
            this.G = G;
            this.B = B;
        }

        public static double Trim(double value)
        {
            if (value < 0)
                value = 0;
            if (value > 1)
                value = 1;
            return value;
        }

        private static double CheckValue(double value)
        {
            if (value < 0 || value > 1 || double.IsNaN(value))
                throw new Exception(string.Format
                    ("Wrong channel value {0} (the value must be between 0 and 1", value));
            return value;
        }

        public static Pixel operator *(Pixel px, double param)
            => new Pixel(
                Trim(px.R * param),
                Trim(px.G * param),
                Trim(px.B * param));

        public static Pixel operator *(double param, Pixel px) 
            => px * param;

        public static Pixel operator +(Pixel px1, Pixel px2)
            => new Pixel(
                Trim(px1.R * px2.R),
                Trim(px1.G * px2.G),
                Trim(px1.B * px2.B));
    }
}