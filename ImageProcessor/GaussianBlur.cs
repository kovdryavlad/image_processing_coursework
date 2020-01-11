using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleMatrix;

namespace ImageProcessor
{
    public class GaussianBlur
    {
        double m_sigma;

        //Гауссіан
        double G(double x, double y) => Math.Exp(-(x * x + y * y) / (2 * m_sigma * m_sigma)) / Math.Sqrt(2 * Math.PI * m_sigma * m_sigma);

        //формування ядра згортки
        public double[][] getKernel(double sigma)
        {
            m_sigma = sigma;

            //обчисленя розмірів ядра згортки
            int radius = kernelRadius(m_sigma);
            int size = 2 * radius + 1;

            double sum = 0;     //сума елементів у ядрі
            Matrix kernel = new Matrix(size);

            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    //обчислення значеннь ядра згортки 
                    //(з переносом початку координат у центр матриці)
                    sum += kernel[i, j] = G(i - radius, j - radius);
           
            kernel = 1d/sum * kernel;   //нормування
            
            return kernel.data;
        }

        //радіус згортки
        int kernelRadius(double sigma) => (int)Math.Ceiling(sigma * 3);
    }
}
