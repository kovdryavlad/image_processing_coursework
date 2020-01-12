using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleMatrix;

namespace SimpleProcessing
{
    public class ImageScaleChanger
    {
        public static double[,,] ResizeImage(double[,,] image, double k) {
            int N = (int)(image.GetLength(1) * k);
            int M = (int)(image.GetLength(2) * k);

            double[,,] scaledImage = new double[3, N, M];

            for (int i_new = 0; i_new < N; i_new++)
            {
                for (int j_new = 0; j_new < M; j_new++)
                {
                    double yy = i_new / k;
                    double xx = j_new / k;

                    int i = (int)xx;
                    int j = (int)yy;

                    double x = 2 * (xx - i);
                    double y = 2 * (yy - j);

                    double[][] gama = getGama20(x, y);

                    double[] resultRGB = ApplyConvolutionFilterByCoords(image, i, j, gama);

                    scaledImage[0, i_new, j_new] = resultRGB[0];
                    scaledImage[1, i_new, j_new] = resultRGB[1];
                    scaledImage[2, i_new, j_new] = resultRGB[2];
                }
            }


            return scaledImage;
        }


        static double[] ApplyConvolutionFilterByCoords(double[,,] image, int x, int y, double[][] gama)
        {
            int height = image.GetLength(1);
            int width = image.GetLength(2);

            int filterWidth = gama.Length;
            int filterHeight = gama.Length;

            int filterRadius = gama.Length / 2;

            double pixelR = 0;
            double pixelG = 0;
            double pixelB = 0;

            int StartY = y - filterRadius;
            int StartX = x - filterRadius;

            for (int FX = 0; FX < filterWidth; FX++)
            {
                for (int FY = 0; FY < filterHeight; FY++)
                {
                    int xx = StartX + FX;
                    int yy = StartY + FY;

                    if (xx < 0)
                        xx = Math.Abs(xx) - 1;


                    if (yy < 0)
                        yy = Math.Abs(yy) - 1;

                    if (xx >= width)
                        xx = xx - (FX - filterRadius);


                    if (yy >= height)
                        yy = yy - (FY - filterRadius);

                    pixelR += image[0, yy, xx] * gama[FX][FY];
                    pixelG += image[1, yy, xx] * gama[FX][FY];
                    pixelB += image[2, yy, xx] * gama[FX][FY];
                }
            }

            return new double[] { pixelR, pixelG, pixelB };
        }

       static double[][] getGama20(double x, double y) {

            return 1d/64 * new Matrix(3, 3, new double[] {
                  (1-x).Pow(2)*(1-y).Pow(2),  (1-x).Pow(2)*(6-2*y*y),    (1-x).Pow(2)*(1+y).Pow(2), 
                     (6-2*x*x)*(1-y).Pow(2),     (6-2*x*x)*(6-2*y*y),       (6-2*x*x)*(1+y).Pow(2),
                  (1+x).Pow(2)*(1-y).Pow(2),  (1+x).Pow(2)*(6-2*y*y),    (1+x).Pow(2)*(1+y).Pow(2)
            });
        }
    }

    public static class DoubleHelper{
        public static double Pow(this double x, int y)
        {
            switch (y)
            {
                case 1: return x;
                case 2: return x * x;
                case 3: return x * x * x;
                case 4: return x * x * x * x;

                default:
                    return Math.Pow(x, y);
            }
        }
    }
}
