using ImageProcessor;
using SimpleMatrix;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SimpleProcessing
{
    public partial class Form1 : Form
    {
        double[,,] m_originalImage;
        double[,,] m_workImage;

        string m_imageName;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //testing of quarter matrix filling
            //SimpleMatrix.Matrix m = new SimpleMatrix.Matrix(3, 3, new[]{
            //    1,2,3d,
            //    4,5,6,
            //    7,8, 9
            //});
            //
            //System.Diagnostics.Debug.WriteLine(getKernelByQuarter(m));
        }

        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileWindow = new OpenFileDialog();
            if (openFileWindow.ShowDialog() == DialogResult.OK)
            {
                Bitmap bmp = new Bitmap(openFileWindow.FileName);

                //storing image info
                m_workImage = BitmapConverter.BitmapToDoubleRgb(bmp);
                m_originalImage = (double[,,])m_workImage.Clone();
                m_imageName = Path.GetFileNameWithoutExtension(openFileWindow.FileName);

                //fill fields with image height and width
                textBox1.Text = String.Format("Ширина: {0}{2}Висота: {1}", bmp.Width, bmp.Height, Environment.NewLine);
                SubdivisionWidthTextBox.Text = bmp.Width.ToString();
                SubdivisionHeightTextBox.Text = bmp.Height.ToString();

                OutputBitmapOnPictureBox(bmp);
            }            
        }

        private void вернутьсяКИсходномуToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_workImage = (double[,,])m_originalImage.Clone();
            OutputBitmapOnPictureBox(BitmapConverter.DoubleRgbToBitmap(m_workImage));
        }

        Bitmap segmented;

        Bitmap GetScaledBitmap(Bitmap image, int widthMax = 2000, int heightMax = 2000)
        {
            double Wreal = image.Width;
            double Hreal = image.Height;

            if (Wreal > widthMax || heightMax > 2000)
            {

                double Wmax = widthMax;
                double Hmax = heightMax;

                double l = Hreal / Wreal;

                int scaledWidth = (int)Wmax;
                int scaledHeight = (int)Hmax;

                if (Wreal / Wmax > Hreal / Hmax)
                    scaledHeight = (int)(Wmax * l);

                else
                    scaledWidth = (int)(Hmax / l);

                return Service.ResizeImage(image, scaledWidth, scaledHeight);
            }

            return image;
        }


        //размытие по гауссу
        private void button5_Click(object sender, EventArgs e)
        {
            double sigma = 0.8;

            GaussianBlur gaussianBlur = new GaussianBlur();
            double[][] filter = gaussianBlur.getKernel(sigma);

            m_workImage = DoubleArrayImageOperations.ConvolutionFilter(m_workImage, filter);
            OutputBitmapOnPictureBox(BitmapConverter.DoubleRgbToBitmap(m_workImage));
        }

        private void зберегтиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog()
            {
                InitialDirectory = Environment.CurrentDirectory,
                FileName = m_imageName + "_processed",
                DefaultExt = ".png"
            };

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                BitmapConverter.DoubleRgbToBitmap(m_workImage).Save(sfd.FileName);
            }
        }

        private void pictureBox1_SizeChanged(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"width: {pictureBox1.Width} - height: {pictureBox1.Height}");
        }

        void OutputBitmapOnPictureBox(Bitmap image)
        {
            double Wreal = image.Width;
            double Hreal = image.Height;

            if (Wreal > pictureBox1.Width || Hreal > pictureBox1.Height)
            {

                double Wmax = pictureBox1.Width;
                double Hmax = pictureBox1.Height;

                double l = Hreal / Wreal;

                int scaledWidth = (int)Wmax;
                int scaledHeight = (int)Hmax;

                if (Wreal / Wmax > Hreal / Hmax)
                    scaledHeight = (int)(Wmax * l);

                else
                    scaledWidth = (int)(Hmax / l);

                pictureBox1.Image = Service.ResizeImage(image, scaledWidth, scaledHeight);
            }

            else
            {
                pictureBox1.Image = image;
            }
        }

        bool workImageOnScreen = true;

        private void перемикачАБэToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (workImageOnScreen)
            {
                OutputBitmapOnPictureBox(BitmapConverter.DoubleRgbToBitmap(m_originalImage));
                workImageOnScreen = false;
            }
            else
            {
                OutputBitmapOnPictureBox(BitmapConverter.DoubleRgbToBitmap(m_workImage));
                workImageOnScreen = true;
            }

        }

        void ApplyFilter(SimpleMatrix.Matrix kernel, double normilizer)
        {
            kernel /= normilizer;

            m_workImage = DoubleArrayImageOperations.ConvolutionFilter(m_workImage, kernel.data);
            RefreshWorkImage();
        }

        void RefreshWorkImage() => OutputBitmapOnPictureBox(BitmapConverter.DoubleRgbToBitmap(m_workImage));

        private SimpleMatrix.Matrix getKernelByQuarter(SimpleMatrix.Matrix kernelQuarter)
        {
            int n = kernelQuarter.Columns;
            int N = 2 * n - 1;
            SimpleMatrix.Matrix m = new SimpleMatrix.Matrix(N);

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    //левое
                    m.data[i][j] = kernelQuarter.data[i][j];
                    //вправое
                    m.data[i][N - 1 - j] = m.data[i][j];
                    //вниз 
                    m.data[N - 1 - i][j] = m.data[i][j];
                    //вниз вправо
                    m.data[N - 1 - i][N - 1 - j] = m.data[i][j];

                }
            }

            return m;
        }

        private void S20ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SimpleMatrix.Matrix kernel = new SimpleMatrix.Matrix(3, 3, new double[] {
                1,    6,    1,
                6,   36,    6,
                1,    6,    1
            });

            ApplyFilter(kernel, 64);
        }

        private void S30ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SimpleMatrix.Matrix kernel = new SimpleMatrix.Matrix(3, 3, new double[] {
                1,    4,    1,
                4,   16,    4,
                1,    4,    1
            });

            ApplyFilter(kernel, 36);
        }

        private void S40ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SimpleMatrix.Matrix kernel = new SimpleMatrix.Matrix(5, 5, new double[] {
                1,     76,     230,     76,    1, 
                76,  5776,   17480,   5776,   76,
               230, 17480,   52900,  17480,  230,
                76,  5776,   17480,   5776,   76,
                1,     76,     230,     76,    1
            });

            ApplyFilter(kernel, 147456);
        }

        //ВЧ20
        private void ToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            SimpleMatrix.Matrix kernel = new SimpleMatrix.Matrix(3, 3, new double[] {
                -1,   -6,   -1,
                -6,   28,   -6,
                -1,   -6,   -1
            });

            ApplyFilter(kernel, 64);
        }

        private void S30ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SimpleMatrix.Matrix kernel = new SimpleMatrix.Matrix(3, 3, new double[] {
                -1,    -4,    -1,
                -4,    20,    -4,
                -1,    -4,    -1
            });

            ApplyFilter(kernel, 36);
        }

        private void S40ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SimpleMatrix.Matrix kernel = new SimpleMatrix.Matrix(5, 5, new double[] {
                 1,     76,     230,     76,    1,
                76,   5776,   17480,   5776,   76,
               230,  17480,   52900,  17480,  230,
                76,   5776,   17480,   5776,   76,
                 1,     76,     230,     76,    1
            });

            kernel = -1.0 * kernel;
            kernel.data[2][2] = 94556;

            ApplyFilter(kernel, 147456);
        }

        private void S20ToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            SimpleMatrix.Matrix kernel = new SimpleMatrix.Matrix(5, 5, new double[] {
                 1,    -8,    48,   -8,   1,
                -8,    64,  -384,   64,  -8,
                48,  -384,  2304, -384,  48,
                -8,    64,  -384,   64,  -8,
                 1,    -8,    48,   -8,   1
            });
            

            ApplyFilter(kernel, 1156);
        }

        private void S30ToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            SimpleMatrix.Matrix kernel = new SimpleMatrix.Matrix(5, 5, new double[] {
                 1,    -6,    24,   -6,    1,
                -6,    36,  -144,   36,   -6,
                24,  -144,   576,  -144,  24,
                -6,    36,  -144,   36,   -6,
                 1,    -6,    24,   -6,    1
            });


            ApplyFilter(kernel, 196);
        }

        private void S40ToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            SimpleMatrix.Matrix kernelQuarter = new SimpleMatrix.Matrix(5, 5, new double[] {
                 0.000000451,  0.000032858, -0.000173172,  0.000543531, -0.001478877,
                 0.000032858,  0.002394093, -0.012617568,  0.039602551, -0.107753344,
                -0.000173172, -0.012617568,  0.066498257, -0.208716980,  0.567891511,
                 0.000543531,  0.039602551,  -0.20871698,  0.655096535, -1.782431695,
                -0.001478877, -0.107753344,  0.567891511, -1.782431695,  4.84976271,
            });

            SimpleMatrix.Matrix kernel = getKernelByQuarter(kernelQuarter);

            ApplyFilter(kernel, 1);
        }

      
        //stab 0
        private void ToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            SimpleMatrix.Matrix kernel = new SimpleMatrix.Matrix(5, 5, new double[] {
                  1,     8,    -74,     8,    1,
                  8,    64,   -592,    64,    8,
                -74,  -592,   5476,  -592,  -74,
                  8,    64,   -592,    64,    8,
                  1,     8,    -74,     8,    1
            });


            ApplyFilter(kernel, 3136);
        }
        //stab 1
        private void ToolStripMenuItem4_Click(object sender, EventArgs e)
        {
            SimpleMatrix.Matrix kernelQuarter = new SimpleMatrix.Matrix(4, 4, new double[] {
                3.75457e-09, 8.93587e-07, 5.40282e-06, -7.38748e-05,
                8.93587e-07, 0.000212674, 0.001285871, -0.01758221,
                5.40282e-06, 0.001285871, 0.007774658, -0.106305883,
                -7.38748e-05, -0.01758221, -0.106305883, 1.45356119
            });

            SimpleMatrix.Matrix kernel = getKernelByQuarter(kernelQuarter);

            ApplyFilter(kernel, 1);
        }

        //stab 2
        private void ToolStripMenuItem5_Click(object sender, EventArgs e)
        {
            SimpleMatrix.Matrix kernelQuarter = new SimpleMatrix.Matrix(4, 4, new double[] {
                1.24562e-08, 2.96456e-06, 1.59314e-05, -0.000149424,
                2.96456e-06, 0.000705566, 0.003791678, -0.035562919,
                1.59314e-05, 0.003791678, 0.020376288, -0.191113331,
                -0.000149424, -0.035562919, -0.191113331, 1.792490633
            });

            SimpleMatrix.Matrix kernel = getKernelByQuarter(kernelQuarter);

            ApplyFilter(kernel, 1);
        }

        //stab 3
        private void ToolStripMenuItem6_Click(object sender, EventArgs e)
        {
            SimpleMatrix.Matrix kernelQuarter = new SimpleMatrix.Matrix(5, 5, new double[] {
                1.6236e-10, 4.1165e-08, 9.20847e-07, 2.14132e-06, -1.8949e-05,
                4.1165e-08, 1.0437e-05, 0.000233473, 0.000542915, -0.004804375,
                9.20847e-07, 0.000233473, 0.00522272, 0.012144825, -0.107472272,
                2.14132e-06, 0.000542915, 0.012144825, 0.028241375, -0.249914233,
                -1.8949e-05, -0.004804375, -0.107472272, -0.249914233, 2.211546853,
            });

            SimpleMatrix.Matrix kernel = getKernelByQuarter(kernelQuarter);

            ApplyFilter(kernel, 1);
        }

        //stab 4
        private void ToolStripMenuItem7_Click(object sender, EventArgs e)
        {
            SimpleMatrix.Matrix kernelQuarter = new SimpleMatrix.Matrix(5, 5, new double[] {
                5.26177e-10, 1.32248e-07, 2.7676e-06, 4.92362e-06, -3.85865e-05,
                1.32248e-07, 3.32391e-05, 0.000695605, 0.001237494, -0.009698272,
                2.7676e-06, 0.000695605, 0.014557157, 0.025897446, -0.202958992,
                4.92362e-06, 0.001237494, 0.025897446, 0.046072025, -0.361067725,
                -3.85865e-05, -0.009698272, -0.202958992, -0.361067725, 2.829697678
            });

            SimpleMatrix.Matrix kernel = getKernelByQuarter(kernelQuarter);

            ApplyFilter(kernel, 1);
        }

        //scale 20
        private void МаштабуватиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            double k = Convert.ToDouble(toolStripTextBox1.Text.Replace(".", ","));

            m_workImage = ImageScaleChanger.ResizeImage(m_workImage, k, ScaleOptions.S20);
            RefreshWorkImage();
        }

        //scale 21
        private void S30ToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            double k = Convert.ToDouble(toolStripTextBox1.Text.Replace(".", ","));

            m_workImage = ImageScaleChanger.ResizeImage(m_workImage, k, ScaleOptions.S21);
            RefreshWorkImage();
        }

        void DoSubdiviosion(ScaleOptions scaleOptions) {
            int w = 0;
            int h = 0;
            try
            {
                w = Convert.ToInt32(SubdivisionWidthTextBox.Text);
                h = Convert.ToInt32(SubdivisionHeightTextBox.Text);
            }
            catch {
                MessageBox.Show("Помилка з конвертуванням параметрів для процедури Subdivision");
                return;
            }
            m_workImage = ImageScaleChanger.SubdivisionImageResize(m_workImage, w, h, scaleOptions);
            RefreshWorkImage();
        }

        //subdivision20
        private void ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            DoSubdiviosion(ScaleOptions.S20);
        }

        //subdivision21
        private void ToolStripMenuItem8_Click(object sender, EventArgs e)
        {
            DoSubdiviosion(ScaleOptions.S21);
        }

        private void СтатистикиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBox1.Text += Environment.NewLine;

            StatisticInfo[] statisticInfo = ImageStatistics.GetStatistics(m_originalImage, m_workImage);
            PrintStatisticInfo(statisticInfo[0], "R");
            PrintStatisticInfo(statisticInfo[1], "G");
            PrintStatisticInfo(statisticInfo[2], "B");

            textBox1.Text += String.Format("{1}Загальний PSNR: {0:0.0000}{1}", statisticInfo.Select(el=>el.PSNR).Average(), Environment.NewLine);
        }

        void PrintStatisticInfo(StatisticInfo statisticInfo, string colorComponentName)
        {
            textBox1.Text += Environment.NewLine + "Складова " + colorComponentName + Environment.NewLine;

            textBox1.Text += String.Format("Average error: {0:0.0000}{1}", statisticInfo.averageError, Environment.NewLine);    
            textBox1.Text += String.Format("Sigma: {0:0.0000}{1}", statisticInfo.sigma, Environment.NewLine);    
            textBox1.Text += String.Format("PSNR: {0:0.0000}{1}", statisticInfo.PSNR, Environment.NewLine);    
        }

        private void S20вручнуToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_workImage = BlurEffectWithoutMatrix.Apply(m_workImage);
            RefreshWorkImage();
        }

        private void ПорівняльнийТестToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SimpleMatrix.Matrix kernel = new SimpleMatrix.Matrix(3, 3, new double[] {
                1,    6,    1,
                6,   36,    6,
                1,    6,    1
            });

            double normilizer = 64;
            kernel /= normilizer;

            DateTime t1, t2;
            double matrixTime, formulasTime;

            t1 = DateTime.Now;
            DoubleArrayImageOperations.ConvolutionFilter(m_workImage, kernel.data);
            t2 = DateTime.Now;
            matrixTime = (t2 - t1).TotalMilliseconds;

            t1 = DateTime.Now;
            BlurEffectWithoutMatrix.Apply(m_workImage);
            t2 = DateTime.Now;
            formulasTime = (t2 - t1).TotalMilliseconds;

            textBox1.Text += Environment.NewLine + "Час обробки з використання матриць: " + matrixTime.ToString("0.000") + "мс"  + Environment.NewLine;
            textBox1.Text += Environment.NewLine + "Час обробки (формула напряму): " + formulasTime.ToString("0.000") + "мс" + Environment.NewLine;
        }
    }
}
