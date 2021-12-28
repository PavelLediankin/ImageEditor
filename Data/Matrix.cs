using System;
using System.Linq;

namespace MyPhotoshop.Data
{
    public class Matrix
    {
        public readonly int Size;
        private readonly double[,] values;
        private double determinantCash = double.NaN;

        public Matrix(int size)
        {
            if (size <= 0)
                throw new ArgumentException("Matrix Should Have Positive Size");
            Size = size;
            values = new double[Size, Size];
        }

        public double this[int i, int j]
        {
            get => values[i, j];
            set
            {
                determinantCash = double.NaN;
                values[i, j] = value;
            }
        }

        public double GetNormingCoefficient()
        {
            var sum = values.Cast<double>().Sum();
            return sum != 0 ? sum : 1;
        }

        //Сделал его по ошибке но вдруг пригодится
        #region Determinant

        public Matrix GetMinor(int row, int column)
        {
            var minor = new Matrix(Size - 1);
            for (var i = 0; i < Size; i++)
            for (var j = 0; j < Size; j++)
            {
                if (i == row || j == column)
                    continue;
                var minorI = GetMinorPosition(row, i);
                var minorJ = GetMinorPosition(column, j);
                minor[minorI, minorJ] = values[i, j];
            }
            return minor;
        }

        private static int GetMinorPosition(int row, int i)
            => i > row ? i - 1 : i;

        public double GetDeterminant()
        {
            if (!double.IsNaN(determinantCash))
                return determinantCash;

            var determinant = 0.0;
            if (Size == 1)
                return values[0, 0];
            if (Size == 2)
                return values[0, 0] * values[1, 1] - values[0, 1] * values[1, 0];
            
            for (var i = 0; i < Size; i++)
                determinant += (i % 2 == 1 ? 1 : -1) * values[0, i] *
                               GetMinor(0, i).GetDeterminant();
            determinantCash = determinant;
            return determinant;
        }

        #endregion 

        #region Operators

        private static Matrix Multiply(Matrix m, double k)
        {
            var res = new Matrix(m.Size);
            for (var i = 0; i < m.Size; i++)
            for (var j = 0; j < m.Size; j++)
                res[i, j] = m[i, j] * k;
            return res;
        }
        
        public static Matrix Multiply(Matrix m1, Matrix m2)
        {
            var res = new Matrix(m1.Size);
            for (var i = 0; i < m1.Size; i++)
            for (var j = 0; j < m2.Size; j++)
            for (var k = 0; k < m2.Size; k++)
                res[i, j] += m1[i, k] * m2[k, j];
            return res;
        }
        
        public static Matrix Subtract(Matrix m1, Matrix m2)
        {
            var res = new Matrix(m1.Size);
            for (var i = 0; i < m1.Size; i++)
            for (var j = 0; j < m2.Size; j++)
                res[i, j] = m1[i, j] - m2[i, j];
            return res;
        }

        public static Matrix Sum(Matrix m1, Matrix m2)
        {
            var res = new Matrix(m1.Size);
            for (var i = 0; i < m1.Size; i++)
            for (var j = 0; j < m2.Size; j++)
                res[i, j] = m1[i, j] + m2[i, j];
            return res;
        }

        public static Matrix operator *(Matrix m1, Matrix m2)
            => Multiply(m1, m2);

        public static Matrix operator *(Matrix m1, double k)
            => Multiply(m1, k);

        public static Matrix operator *(double k, Matrix m1)
            => Multiply(m1, k);

        public static Matrix operator -(Matrix m1, Matrix m2)
            => Subtract(m1, m2);

        public static Matrix operator +(Matrix a, Matrix b) 
            => Sum(a, b);

        #endregion
    }
}