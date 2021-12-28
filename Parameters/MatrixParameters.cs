using MyPhotoshop.Data;

namespace MyPhotoshop.Parameters
{
    /// <summary>
    /// Values
    /// </summary>
    public class MatrixParameters : IParameters
    {
        private readonly Matrix matrix;

        [ParameterInfo(Name = "Коэффициент(Уровень)", MaxValue = 10, MinValue = 0, Increment = 0.1, DefaultValue = 1)]
        public double Coeficient { get; set; }

        // Channels
        
    }
}