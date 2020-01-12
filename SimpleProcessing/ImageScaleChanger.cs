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
        public static double[,,] ResizeImage(double[,,] image, double k, ScaleOptions scaleOptions) {
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

                    //chosing gama
                    double[][] gama = null;
                    if(scaleOptions ==ScaleOptions.S20)
                        gama = getGama20(x, y);
                    else if (scaleOptions == ScaleOptions.S21)
                        gama = getGama21(x, y);

                    double[] resultRGB = ApplyConvolutionFilterByCoords(image, i, j, gama);

                    scaledImage[0, i_new, j_new] = resultRGB[0];
                    scaledImage[1, i_new, j_new] = resultRGB[1];
                    scaledImage[2, i_new, j_new] = resultRGB[2];
                }
            }


            return scaledImage;
        }

        public static double[,,] SubdivisionImageResize(double[,,] image, int M, int N, ScaleOptions scaleOptions)
        {
            int V = image.GetLength(1);
            int U = image.GetLength(2);

            double[,,] scaledImage = new double[3, N, M];

            for (int n = 0; n < N; n++)
            {
                for (int m = 0; m < M; m++)
                {
                    double u_star = (U * (2 * m + 1)) / (2d * M);
                    double v_star = (V * (2 * n + 1)) / (2d * N);

                    int u = (int)u_star;
                    int v = (int)v_star;

                    double x = 2 * (u_star - u) - 1;
                    double y = 2 * (v_star - v) - 1;

                    //chosing gama
                    double[][] gama = null;
                    if (scaleOptions == ScaleOptions.S20)
                        gama = getGama20(x, y);
                    else if (scaleOptions == ScaleOptions.S21)
                        gama = getGama21(x, y);

                    double[] resultRGB = ApplyConvolutionFilterByCoords(image, u, v, gama);

                    scaledImage[0, n, m] = resultRGB[0];
                    scaledImage[1, n, m] = resultRGB[1];
                    scaledImage[2, n, m] = resultRGB[2];
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

        static double[][] getGama21(double x, double y)
        {
            double normilizer = 1d / 2304;
            Matrix m = new Matrix(5);

            m.data[0][0] = (1 - x) * (1 - x) * (1 - y) * (1 - y);
            m.data[0][1] = -(1 - x) * (1 - x) * (2 - 16 * y + 10 * y * y);
            m.data[0][2] = -(1 - x) * (1 - x) * (46 - 18 * y * y);
            m.data[0][3] = -(1 - x) * (1 - x) * (2 + 16 * y + 10 * y * y);
            m.data[0][4] = (1 - x) * (1 - x) * (1 + y) * (1 + y);

            m.data[1][0] = -(1 - y) * (1 - y) * (2 - 16 * x + 10 * x * x);
            m.data[1][1] = (2 - 16 * x + 10 * x * x) * (2 - 16 * y + 10 * y * y);
            m.data[1][2] = (2 - 16 * x + 10 * x * x) * (46 - 18 * y * y);
            m.data[1][3] = (2 - 16 * x + 10 * x * x) * (2 + 16 * y + 10 * y * y);
            m.data[1][4] = -(2 - 16 * x + 10 * x * x) * (1 + y) * (1 + y);

            m.data[2][0] = -(1 - y) * (1 - y) * (46 - 18 * x * x);
            m.data[2][1] = (2 - 16 * y + 10 * y * y) * (46 - 18 * x * x);
            m.data[2][2] = (46 - 18 * y * y) * (46 - 18 * x * x);
            m.data[2][3] = (46 - 18 * x * x) * (2 + 16 * y + 10 * y * y);
            m.data[2][4] = -(46 - 18 * x * x) * (1 + y) * (1 + y);

            m.data[3][0] = -(1 - y) * (1 - y) * (2 + 16 * x + 10 * x * x);
            m.data[3][1] = (2 - 16 * y + 10 * y * y) * (2 + 16 * x + 10 * x * x);
            m.data[3][2] = (2 + 16 * x + 10 * x * x) * (46 - 18 * y * y);
            m.data[3][3] = (2 + 16 * x + 10 * x * x) * (2 + 16 * y + 10 * y * y);
            m.data[3][4] = -(2 + 16 * x + 10 * x * x) * (1 + y) * (1 + y);

            m.data[4][0] = (1 + x) * (1 + x) * (1 - y) * (1 - y);
            m.data[4][1] = -(1 + x) * (1 + x) * (2 - 16 * y + 10 * y * y);
            m.data[4][2] = -(1 + x) * (1 + x) * (46 - 18 * y * y);
            m.data[4][3] = -(1 + x) * (1 + x) * (2 + 16 * y + 10 * y * y);
            m.data[4][4] = (1 + x) * (1 + x) * (1 + y) * (1 + y);

            return normilizer * m;
        }
    }

    public enum ScaleOptions {
        S20,
        S21
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
