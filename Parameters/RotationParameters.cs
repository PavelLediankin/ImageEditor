namespace MyPhotoshop.Parameters
{
    public class RotationParameters : IParameters
    {
        [ParameterInfo(Name = "Angle", MaxValue = 360, MinValue = 0, Increment = 5, DefaultValue = 0)]
        public double Angle { get; set; }
    }
}