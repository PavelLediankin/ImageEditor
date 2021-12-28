using MyPhotoshop.Data;
using MyPhotoshop.Extensions;
using MyPhotoshop.Parameters;

namespace MyPhotoshop.Filters
{
    public class MatrixFilter : ParametrizedFilter<EmptyParameters>
    {
        private readonly string name;
        private Matrix processingMatrix;

        public MatrixFilter(string name, Matrix processingMatrix)
        {
            this.name = name;
            this.processingMatrix = processingMatrix;
        }

        public override Photo Process(Photo photo, EmptyParameters parameters)
        {
            var sizeAdding = processingMatrix.Size / 2;
            var tempPhoto = GetTempPhoto(photo, sizeAdding);
            var processedTemp = processingMatrix.ProcessByMatrix(tempPhoto, sizeAdding);
            return GetResultPhotoFromTemp(processedTemp, sizeAdding);
        }

        private static Photo GetTempPhoto(Photo photo, int sizeAdding)
        {
            var tempPhoto = new Photo
                (photo.Width + 2 * sizeAdding, photo.Height + 2 * sizeAdding);
            for (var x = 0; x < photo.Width; x++)
                for (var y = 0; y < photo.Height; y++)
                    tempPhoto[x + sizeAdding, y + sizeAdding] = photo[x, y];
            FillTempPhotoBorders(tempPhoto, sizeAdding);
            return tempPhoto;
        }

        /// <summary>
        /// ToRefactor
        /// </summary>
        /// <param name="tempPhoto"></param>
        /// <param name="sizeAdding"></param>
        private static void FillTempPhotoBorders(Photo tempPhoto, int sizeAdding)
        {
            for (var x = 0; x < sizeAdding; x++)
            for (var y = 0; y < tempPhoto.Height; y++)
            {
                var start = tempPhoto[sizeAdding, y];
                var end = tempPhoto[tempPhoto.Width - sizeAdding - 1, y];
                tempPhoto[x, y] = start;
                tempPhoto[tempPhoto.Width - x - 1, y] = end;
            }
            
            for (var y = 0; y < sizeAdding; y++)
            for (var x = 0; x < tempPhoto.Width; x++)
            {
                var start = tempPhoto[sizeAdding, y];
                var end = tempPhoto[tempPhoto.Width - sizeAdding - 1, y];
                tempPhoto[x, y] = start;
                tempPhoto[x, tempPhoto.Height - y - 1] = end;
            }
        }
        
        private static Photo GetResultPhotoFromTemp(Photo tempPhoto, int sizeAdding)
        {
            var result = new Photo
                (tempPhoto.Width - 2 * sizeAdding, tempPhoto.Height - 2 * sizeAdding);
            for (var x = sizeAdding; x < tempPhoto.Width - sizeAdding - 1; x++)
            for (var y = sizeAdding; y < tempPhoto.Height - sizeAdding - 1; y++)
                result[x, y] = tempPhoto[x, y];
            return result;
        }

        public override string ToString()
        {
            return name;
        }
    }
}