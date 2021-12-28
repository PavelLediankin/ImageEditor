namespace MyPhotoshop.Parameters
{
    public interface IParametersHandler<TParameters>
    {
        ParameterInfo[] GetParameters();
        TParameters CreateParameters(double[] values);
    }
}