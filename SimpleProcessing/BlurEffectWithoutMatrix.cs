using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleProcessing
{
    class BlurEffectWithoutMatrix
    {
        public static double[,,] Apply(double[,,] img) {

            //1,    6,    1,
            //6,   36,    6,
            //1,    6,    1
            //64 - normalizer

            int height = img.GetLength(1);
            int width = img.GetLength(2);

            double[,,] resultImage = new double[3, height, width];

            for (int i = 1; i < height-1; i++)
            {
                for (int j = 1; j < width-1; j++)
                {
                    resultImage[0, i, j] = (img[0, i - 1, j - 1] + 6 * img[0, i - 1, j] + img[0, i - 1, j + 1] + 6 * img[0, i, j - 1] + 36 * img[0, i, j] + 6 * img[0, i, j + 1] + img[0, i + 1, j - 1] + 6 * img[0, i + 1, j] + img[0, i + 1, j + 1]) / 64d;
                    resultImage[1, i, j] = (img[1, i - 1, j - 1] + 6 * img[1, i - 1, j] + img[1, i - 1, j + 1] + 6 * img[1, i, j - 1] + 36 * img[1, i, j] + 6 * img[1, i, j + 1] + img[1, i + 1, j - 1] + 6 * img[1, i + 1, j] + img[1, i + 1, j + 1]) / 64d;
                    resultImage[2, i, j] = (img[2, i - 1, j - 1] + 6 * img[2, i - 1, j] + img[2, i - 1, j + 1] + 6 * img[2, i, j - 1] + 36 * img[2, i, j] + 6 * img[2, i, j + 1] + img[2, i + 1, j - 1] + 6 * img[2, i + 1, j] + img[2, i + 1, j + 1]) / 64d;
                }
            }

            return resultImage;

        }
    }
}
