using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleProcessing
{
    class ImageStatistics
    {
        private static StatisticInfo AverageError(double[,,] imageA, double[,,] imageB, int color)
        {
            if (color < 0 || color > 2)
                throw new Exception("Invalid value of 'color' parameter");

            int heightA = imageA.GetLength(1);
            int widthA = imageA.GetLength(2);

            int heightB = imageB.GetLength(1);
            int widthB= imageA.GetLength(2);

            int minHeight = Math.Min(heightA, heightB);
            int minWidth = Math.Min(widthA, widthB);

            //if((heightA != heightB) || (widthA != widthB))
            //    throw new Exception("Images must have the same size");

            StatisticInfo res = new StatisticInfo();
            res.eps = new double[minHeight * minWidth];
            for (int i = 0; i < minHeight; i++)
                for (int j = 0; j < minWidth; j++)
                {
                    double eps = imageA[color, i, j] - imageB[color, i, j];
                    res.averageError += eps;
                    res.eps[i * heightA + j] = eps;
                }

            res.averageError /= (minHeight * minWidth);

            return res;
        }

        private static StatisticInfo AverageErrorR(double[,,] imageA, double[,,] imageB) => AverageError(imageA, imageB, 0);

        private static StatisticInfo AverageErrorG(double[,,] imageA, double[,,] imageB) => AverageError(imageA, imageB, 1);

        private static StatisticInfo AverageErrorB(double[,,] imageA, double[,,] imageB) => AverageError(imageA, imageB, 2);

        private static void FillSigma(StatisticInfo statisticInfo)
        {
            double disp = 0;
            int N = statisticInfo.eps.Length;
            for (int i = 0; i < N; i++)
                disp += (statisticInfo.eps[i] - statisticInfo.averageError).Pow(2);


            statisticInfo.sigma = Math.Sqrt(disp / (N - 1));
        }

        private static void FillPSNR(StatisticInfo statisticInfo) {
            statisticInfo.PSNR = 10 * Math.Log10(255 * 255d / (statisticInfo.sigma * statisticInfo.sigma));
        }

        public static StatisticInfo[] GetStatistics(double[,,] imageA, double[,,] imageB) {
            StatisticInfo r = AverageErrorR(imageA, imageB);
            StatisticInfo g = AverageErrorG(imageA, imageB);
            StatisticInfo b = AverageErrorB(imageA, imageB);

            FillSigma(r);
            FillSigma(g);
            FillSigma(b);

            FillPSNR(r);
            FillPSNR(g);
            FillPSNR(b);

            return new[] { r, g, b };
        }
    }
}
