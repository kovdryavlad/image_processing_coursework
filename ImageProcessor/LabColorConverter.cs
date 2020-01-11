﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessor
{
    public class LabColorConverter
    {
        public static double[] lab2rgb(double[] lab)
        {
            double y = (lab[0] + 16) / 116,
                   x = lab[1] / 500 + y,
                   z = y - lab[2] / 200,
                   r, g, b;

            x = 0.95047 * ((x * x * x > 0.008856) ? x * x * x : (x - 16 / 116) / 7.787);
            y = 1.00000 * ((y * y * y > 0.008856) ? y * y * y : (y - 16 / 116) / 7.787);
            z = 1.08883 * ((z * z * z > 0.008856) ? z * z * z : (z - 16 / 116) / 7.787);

            r = x * 3.2406 + y * -1.5372 + z * -0.4986;
            g = x * -0.9689 + y * 1.8758 + z * 0.0415;
            b = x * 0.0557 + y * -0.2040 + z * 1.0570;

            r = (r > 0.0031308) ? (1.055 * Math.Pow(r, 1d / 2.4) - 0.055) : 12.92 * r;
            g = (g > 0.0031308) ? (1.055 * Math.Pow(g, 1d / 2.4) - 0.055) : 12.92 * g;
            b = (b > 0.0031308) ? (1.055 * Math.Pow(b, 1d / 2.4) - 0.055) : 12.92 * b;

            return new[]{Math.Max(0, Math.Min(1, r)) * 255,
                         Math.Max(0, Math.Min(1, g)) * 255,
                         Math.Max(0, Math.Min(1, b)) * 255 };
        }

        public static double[] rgb2lab(double[] rgb)
        {
            double r = rgb[0] / 255,
                   g = rgb[1] / 255,
                   b = rgb[2] / 255,
                   x, y, z;

            r = (r > 0.04045) ? Math.Pow((r + 0.055) / 1.055, 2.4) : r / 12.92;
            g = (g > 0.04045) ? Math.Pow((g + 0.055) / 1.055, 2.4) : g / 12.92;
            b = (b > 0.04045) ? Math.Pow((b + 0.055) / 1.055, 2.4) : b / 12.92;

            x = (r * 0.4124 + g * 0.3576 + b * 0.1805) / 0.95047;
            y = (r * 0.2126 + g * 0.7152 + b * 0.0722) / 1.00000;
            z = (r * 0.0193 + g * 0.1192 + b * 0.9505) / 1.08883;

            x = (x > 0.008856) ? Math.Pow(x, 1d / 3) : (7.787 * x) + 16 / 116;
            y = (y > 0.008856) ? Math.Pow(y, 1d / 3) : (7.787 * y) + 16 / 116;
            z = (z > 0.008856) ? Math.Pow(z, 1d / 3) : (7.787 * z) + 16 / 116;

            return new[] { (116 * y) - 16, 500 * (x - y), 200 * (y - z) };
        }

        public static double deltaE(double[] labA, double[] labB)
        {
            var deltaL = labA[0] - labB[0];
            var deltaA = labA[1] - labB[1];
            var deltaB = labA[2] - labB[2];

           // return Math.Sqrt(deltaL * deltaL + deltaA * deltaA + deltaB * deltaB);

            var c1 = Math.Sqrt(labA[1] * labA[1] + labA[2] * labA[2]);
            var c2 = Math.Sqrt(labB[1] * labB[1] + labB[2] * labB[2]);
            var deltaC = c1 - c2;
            var deltaH = deltaA * deltaA + deltaB * deltaB - deltaC * deltaC;
            deltaH = deltaH < 0 ? 0 : Math.Sqrt(deltaH);
            var sc = 1.0 + 0.045 * c1;
            var sh = 1.0 + 0.015 * c1;
            var deltaLKlsl = deltaL / (1.0);
            var deltaCkcsc = deltaC / (sc);
            var deltaHkhsh = deltaH / (sh);
            var i = deltaLKlsl * deltaLKlsl + deltaCkcsc * deltaCkcsc + deltaHkhsh * deltaHkhsh;
            return i < 0 ? 0 : Math.Sqrt(i);
        }
    }
}
