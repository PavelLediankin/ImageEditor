using System.Drawing;
using MyPhotoshop.Data;
using MyPhotoshop.Parameters;

namespace MyPhotoshop.Filters
{
    public class TransformFilter<TParameters> : ParametrizedFilter<TParameters>
        where TParameters : IParameters, new()
    {
        private readonly string name;
        private readonly ITransformer<TParameters> transformer;

        public TransformFilter(string name,
            ITransformer<TParameters> transformer)
        {
            this.name = name;
            this.transformer = transformer;
        }

        public override Photo Process(Photo photo, TParameters parameters)
        {
            var size = new Size(photo.Width, photo.Height);
            transformer.Prepare(size, parameters);
            var result = new Photo(transformer.ResultSize.Width, transformer.ResultSize.Height);
            for (var x = 0; x < result.Width; x++)
                for (var y = 0; y < result.Height; y++)
                {
                    var pt = new Point(x, y);
                    var oldPt = transformer.MapPoint(pt);
                    if(oldPt.HasValue)
                    result[x, y] = photo[oldPt.Value.X, oldPt.Value.Y];
                }
            return result;
        }

        public override string ToString()
        {
            return name;
        }
    }
}
