using MyPhotoshop.Data;
using MyPhotoshop.Extensions;
using MyPhotoshop.Parameters;

namespace MyPhotoshop.Filters
{
    public abstract class ParametrizedFilter<TParameters> : IFilter
         where TParameters : IParameters, new()
    {
        private readonly IParametersHandler<TParameters> handler = new ParametersHandler<TParameters>();

        public ParameterInfo[] GetParameters()
        {
            return handler.GetParameters();
        }

        public abstract Photo Process(Photo photo, TParameters parameters);

        public Photo Process(Photo photo, double[] parameters)
        {
            var tParams = handler.CreateParameters(parameters);
            return Process(photo, tParams);
        }
    }
}