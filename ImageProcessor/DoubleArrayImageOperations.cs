using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessor
{
    public class DoubleArrayImageOperations
    {
        private static double[,,] processImage(double[,,] arrayImage, Action<ColorCustom> pixelAction)
        {
            double[,,] res = (double[,,])arrayImage.Clone();

            int width = arrayImage.GetLength(2),
                height = arrayImage.GetLength(1);

            ColorCustom c = new ColorCustom();

            for (int h = 0; h < height; h++)
            {
                for (int w = 0; w < width; w++)
                {                    
                    c.r = res[0, h, w];
                    c.g = res[1, h, w];
                    c.b = res[2, h, w];
                    

                    pixelAction(c);

                    res[0, h, w] = c.r;
                    res[1, h, w] = c.g;
                    res[2, h, w] = c.b;
                }
            }

            return res;
        }

        public static double[,,] ChangeBrightness(double[,,] arrayImage, double n)
        {
            return processImage(arrayImage, (c) =>
            {
                c.r += n;
                c.g += n;
                c.b += n;
            });
        }

        public static double[,,] Negative(double[,,] arrayImage)
        {
            return processImage(arrayImage, (c) =>
            {
                c.r = 255 - c.r;
                c.g = 255 - c.g;
                c.b = 255 - c.b;
            });
        }

        public static double[,,] GetWhiteBlack(double[,,] arrayImage, double brightness)
        {
            return processImage(arrayImage, (c) =>
            {
                double sum = c.r + c.g + c.b;
                if (sum/3d > brightness)
                {
                    c.r = 255;
                    c.g = 255;
                    c.b = 255;
                }
                else {
                    c.r = 0;
                    c.g = 0;
                    c.b = 0;
                }
            });
        }

        public static double[,,] GetGrayScale(double[,,] arrayImage)
        {
            return processImage(arrayImage, (c) =>
            {
                double gray = c.r * 0.2126 + c.g * 0.7152 + c.b * 0.0722;
                c.r = gray;
                c.g = gray;
                c.b = gray;
            });
        }

        public static double[,,] Sepia(double[,,] arrayImage)
        {
            return processImage(arrayImage, (c) =>
            {
                double r = c.r, g = c.g, b = c.b;
                c.r = r * 0.393 + g * 0.769 + b * 0.189;
                c.g = r * 0.349 + g * 0.686 + b * 0.168;
                c.b = r * 0.272 + g * 0.534 + b * 0.131;
            });
        }

        public static double[,,] contrast(double[,,] arrayImage, double coef)
        {
            double average = 0;
            processImage(arrayImage, (c) =>
            {
                average += c.r * 0.299 + c.g * 0.587 + c.b * 0.114;
            });
            average /= arrayImage.GetLength(1) * arrayImage.GetLength(2);

            return processImage(arrayImage, (c) =>
            {
                c.r = average + coef* (c.r - average);
                c.g = average + coef* (c.g - average);
                c.b = average + coef* (c.b - average);
            });
        }

        public static double[,,] ConvolutionFilter(double[,,] arrayImage, double[][] filter)
        {
            int height = arrayImage.GetLength(1);
            int width = arrayImage.GetLength(2);

            int filterHeight = filter[0].Length;
            int filterWidth = filter.Length;

            double[,,] resultImage = new double[3, height, width];
            int filterRadius = filter.GetLength(0)/2;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
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

                            pixelR += arrayImage[0, yy, xx] * filter[FX][FY];
                            pixelG += arrayImage[1, yy, xx] * filter[FX][FY];
                            pixelB += arrayImage[2, yy, xx] * filter[FX][FY];
                        }
                    }

                    resultImage[0, y, x] = pixelR;
                    resultImage[1, y, x] = pixelG;
                    resultImage[2, y, x] = pixelB;
                }
            }

            return resultImage;
        }

        public static double[,,] GetImageInLab(double[,,] arrayImage)
        {
            //перерабоать позже

            return processImage(arrayImage, (c) =>
            {
                double[] lab = LabColorConverter.rgb2lab(new[] { c.r, c.g, c.b });

                c.r = lab[0];
                c.g = lab[1];
                c.b = lab[2];
            });
        }
    }


}
