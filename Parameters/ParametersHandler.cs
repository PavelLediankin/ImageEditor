using System;
using System.Linq;
using System.Reflection;

namespace MyPhotoshop.Parameters
{
    public class ParametersHandler<TParameters> : IParametersHandler<TParameters>
        where TParameters: IParameters, new()
    {
        private static readonly PropertyInfo[] properties;
        private static readonly ParameterInfo[] parameters;

        static ParametersHandler()
        {
            properties = typeof(TParameters)
                .GetProperties()
                .Where(p => p.GetCustomAttributes(typeof(ParameterInfo), false).Length > 0)
                .ToArray();
            parameters = typeof(TParameters)
                .GetProperties()
                .Select(p => p.GetCustomAttributes(typeof(ParameterInfo), false))
                .Where(p => p.Length > 0)
                .Select(p => p[0])
                .Cast<ParameterInfo>()
                .ToArray();
        }

        public ParameterInfo[] GetParameters()
        {
            return parameters;
        }

        public TParameters CreateParameters(double[] values)
        {
            var parameters = new TParameters();
            if (properties.Length != values.Length)
                throw new ArgumentException();
            for (var i = 0; i < properties.Length; i++)
                properties[i].SetValue(parameters, values[i], new object[0]);
            return parameters;
        }
    }
}