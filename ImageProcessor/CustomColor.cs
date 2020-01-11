using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessor
{
    public class ColorCustom
    {
        public double r;
        public double g;
        public double b;

        static Random random = new Random();

        public static ColorCustom GetRandomColor()
        {
            ColorCustom color = new ColorCustom();

            color.r = random.Next(256);
            color.g = random.Next(256);
            color.b = random.Next(256);

            return color;
        }

        public ColorCustom() { }

        public ColorCustom(int r, int g, int b)
        {
            this.r = r;
            this.g = g;
            this.b = b;
        }
    }
}
