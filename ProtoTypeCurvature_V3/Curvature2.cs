using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtoTypeCurvature_V3
{
    class Curvature2
    {
        public Curvature2()
        {
        }

        //https://stackoverflow.com/questions/32629806/how-can-i-calculate-the-curvature-of-an-extracted-contour-by-opencv
        //histogram curvature values are multiplied by 100
        //200 means 2
        int[] histogram;
        public void calculatev1(string filepath)
        {
            histogram = new int[1000];
            StreamReader strd = new StreamReader(filepath);

            //read original image size, not needed here!
            string line = strd.ReadLine();
            string[] xy = line.Split('/');
            //int width = int.Parse(xy[0]);
            //int height = int.Parse(xy[1]);

            while ((line = strd.ReadLine()) != null)
            {
                xy = line.Split(',');
                PointF p = new PointF();
                p.X = float.Parse(xy[0]);
                p.Y = float.Parse(xy[1]);

                line = strd.ReadLine();
                if (line == "#")
                    break;
                xy = line.Split(',');
                PointF p1 = new PointF();
                p1.X = float.Parse(xy[0]);
                p1.Y = float.Parse(xy[1]);
                line = strd.ReadLine();
                if (line == "#")
                    break;
                do
                {
                    xy = line.Split(',');
                    PointF p2 = new PointF();
                    p2.X = float.Parse(xy[0]);
                    p2.Y = float.Parse(xy[1]);

                    //now we have three consecutive points we can calculate first and second derivatives and the curvature value.
                    PointF firstDerivative = new PointF();
                    PointF secondDerivative = new PointF();
                    firstDerivative.X = p1.X - p.X;
                    firstDerivative.Y = p1.Y - p.Y;
                    secondDerivative.X = p2.X - (2.0F * p1.X) + p.X;
                    secondDerivative.Y = p2.Y - (2.0F * p1.Y) + p.Y;

                    double e1 = (secondDerivative.Y * firstDerivative.X) - (secondDerivative.X * firstDerivative.Y);
                    double e2 = Math.Sqrt(
                                            Math.Pow((Math.Pow(firstDerivative.X, 2.0F) + Math.Pow(firstDerivative.Y, 2.0F)), 3.0F)
                                            );

                    double curvature = Math.Abs(e1 / e2);
                    double curv = Math.Round((curvature * 200.0));
                    int hisvalue = (int)curv;
                    if (hisvalue < 1000)
                        histogram[hisvalue]++;

                    p = p1;
                    p1 = p2;

                    for (int i = 0; i < 10; i++)
                    {
                        line = strd.ReadLine();
                        if (line == "#")
                            break;
                    }
                }
                while (line != "#");
            }

            string name = Path.GetDirectoryName(filepath);
            Directory.CreateDirectory(name + "\\Histogram");
            name = name + "\\Histogram\\Histo" + Path.GetFileNameWithoutExtension(filepath) + ".txt";

            StreamWriter sw = new StreamWriter(name);

            //save histogram into a text file.
            for (int i = 0; i < histogram.Length; i++)
            {
                double x = i / 200.0;//to get the original values back
                int y = histogram[i];
                sw.WriteLine(x + ", " + y);
            }

            sw.Close();
        }

        public double[,] calculate(Point[][] segments, int stepsize, int curvatureRange)
        {
            double[,] histogram = new double[curvatureRange, 2];

            PointF p = new PointF(), p1 = new PointF(), p2 = new PointF(); //3 point are needed for the first and second derivatives (curvature calculation)
            for (int i = 0; i < segments.GetLength(0); i++)
            {
                p.X = segments[i][0].X;
                p.Y = segments[i][0].Y;

                if (stepsize >= segments[i].GetLength(0))
                    continue;
                p1.X = segments[i][stepsize].X;
                p1.Y = segments[i][stepsize].Y;
                for (int j = (2 * stepsize); j < segments[i].GetLength(0);)
                {
                    p2.X = segments[i][j].X;
                    p2.Y = segments[i][j].Y;

                    //now we have three consecutive points we can calculate first and second derivatives and the curvature value.
                    PointF firstDerivative = new PointF();
                    PointF secondDerivative = new PointF();
                    firstDerivative.X = p1.X - p.X;
                    firstDerivative.Y = p1.Y - p.Y;
                    secondDerivative.X = p2.X - (2.0F * p1.X) + p.X;
                    secondDerivative.Y = p2.Y - (2.0F * p1.Y) + p.Y;

                    double e1 = (secondDerivative.Y * firstDerivative.X) - (secondDerivative.X * firstDerivative.Y);
                    double e2 = Math.Sqrt(
                                            Math.Pow((Math.Pow(firstDerivative.X, 2.0F) + Math.Pow(firstDerivative.Y, 2.0F)), 3.0F)
                                            );

                    double curvature = Math.Abs(e1 / e2);
                    //150 to match the stepsize used in the paper!! (WHAT THIS EXACTLY MEAN!!!!!!!!!)
                    double curv = Math.Round((curvature * 15.0));
                    int hisvalue = (int)curv;
                    if (hisvalue < curvatureRange)
                        histogram[hisvalue, 1]++;

                    p = p1;
                    p1 = p2;

                    j = j + stepsize;
                }
            }

            //writing the real curvature values to X axeses!
            for (int i = 0; i < histogram.GetLength(0); i++)
            {
                histogram[i, 0] = Math.Round(i / 15.0, 3);//to get the original values back
            }

            return histogram;
        }
        public void calculatev2(string filepath, int stepsize, int curvatureRange)
        {
            histogram = new int[curvatureRange];
            StreamReader strd = new StreamReader(filepath);

            //read original image size, not needed here!
            string line = strd.ReadLine();
            string[] xy = line.Split('/');
            //int width = int.Parse(xy[0]);
            //int height = int.Parse(xy[1]);

            while ((line = strd.ReadLine()) != null)
            {
                xy = line.Split(',');
                PointF p = new PointF();
                p.X = float.Parse(xy[0]);
                p.Y = float.Parse(xy[1]);

                //jump sevral points for stepsize
                for (int i = 0; i < stepsize; i++)
                {
                    line = strd.ReadLine();
                    if (line == "#")
                        break;
                }

                if (line == "#")
                    break;
                xy = line.Split(',');
                PointF p1 = new PointF();
                p1.X = float.Parse(xy[0]);
                p1.Y = float.Parse(xy[1]);
                //jump sevral points for stepsize
                for (int i = 0; i < stepsize; i++)
                {
                    line = strd.ReadLine();
                    if (line == "#")
                        break;
                }

                if (line == "#")
                    break;
                do
                {
                    xy = line.Split(',');
                    PointF p2 = new PointF();
                    p2.X = float.Parse(xy[0]);
                    p2.Y = float.Parse(xy[1]);

                    //now we have three consecutive points we can calculate first and second derivatives and the curvature value.
                    PointF firstDerivative = new PointF();
                    PointF secondDerivative = new PointF();
                    firstDerivative.X = p1.X - p.X;
                    firstDerivative.Y = p1.Y - p.Y;
                    secondDerivative.X = p2.X - (2.0F * p1.X) + p.X;
                    secondDerivative.Y = p2.Y - (2.0F * p1.Y) + p.Y;

                    double e1 = (secondDerivative.Y * firstDerivative.X) - (secondDerivative.X * firstDerivative.Y);
                    double e2 = Math.Sqrt(
                                            Math.Pow((Math.Pow(firstDerivative.X, 2.0F) + Math.Pow(firstDerivative.Y, 2.0F)), 3.0F)
                                            );

                    double curvature = Math.Abs(e1 / e2);
                    //150 to match the stepsize used in the paper!!
                    double curv = Math.Round((curvature * 150.0));
                    int hisvalue = (int)curv;
                    if (hisvalue < curvatureRange)
                        histogram[hisvalue]++;

                    p = p1;
                    p1 = p2;

                    //jump sevral points for stepsize
                    for (int i = 0; i < stepsize; i++)
                    {
                        line = strd.ReadLine();
                        if (line == "#")
                            break;
                    }
                }
                while (line != "#");
            }

            string name = Path.GetDirectoryName(filepath);
            Directory.CreateDirectory(name + "\\Histogram");
            name = name + "\\Histogram\\Histo" + Path.GetFileNameWithoutExtension(filepath) + ".txt";

            StreamWriter sw = new StreamWriter(name);

            //save histogram into a text file.
            for (int i = 0; i < histogram.Length; i++)
            {
                double x = Math.Round(i / 150.0, 3);//to get the original values back
                int y = histogram[i];
                sw.WriteLine(x + ", " + y);
            }

            sw.Close();
        }

        //same as the calculatev2, but input image will be smoothed by Gausian kernel of sigma=10 for both X and Y directions. NOT FINISHED!!
        public void calculatev3(string filepath, int stepsize, int curvatureRange)
        {
            Random rd = new Random((int)DateTime.Now.Ticks);

            histogram = new int[curvatureRange];
            StreamReader strd = new StreamReader(filepath);

            //read original image size, not needed here!
            string line = strd.ReadLine();
            string[] xy = line.Split('/');
            //int width = int.Parse(xy[0]);
            //int height = int.Parse(xy[1]);
            ArrayList points = new ArrayList();
            //http://dev.theomader.com/gaussian-kernel-calculator/
            //0.198005	0.200995	0.202001	0.200995	0.198005
            double[] kernel = new double[] { 0.198005, 0.200995, 0.202001, 0.200995, 0.198005 };

            while ((line = strd.ReadLine()) != null)
            {
                //choose different colors for each segment
                Color cl = Color.FromArgb(0, 0, 0);

                do
                {
                    Point p = new Point();
                    xy = line.Split(',');
                    p.X = int.Parse(xy[0]);
                    p.Y = int.Parse(xy[1]);
                    points.Add(p);
                    //if we want to see the drawing in slow motion!!!!
                    //pictureBox1.Image = bm;
                    //Application.DoEvents();
                    line = strd.ReadLine();
                }
                while (line != "#");

                //now one segment is loaded into the list, we apply Gaussian kernel of size=5, sigma=10!
                //http://dev.theomader.com/gaussian-kernel-calculator/
                //0.198005	0.200995	0.202001	0.200995	0.198005

            }

            //pictureBox1.Image = bm;
        }
        public void calculateTEST()
        {
            //169, 142
            //168, 143
            //167, 143
            //166, 144

            //PointF[] ls = new PointF[4] {new PointF(1,3), new PointF(3,4), new PointF(4,2), new PointF(6,1)};
            PointF[] ls = new PointF[4] { new PointF(10, 10), new PointF(40, 140), new PointF(140, 180), new PointF(100, 100) };

            PointF[] firstDerivative = new PointF[3] { new PointF(), new PointF(), new PointF() };

            PointF[] secondDerivative = new PointF[2] { new PointF(), new PointF() };

            double curv1, curv2;

            firstDerivative[0].X = ls[1].X - ls[0].X;
            firstDerivative[0].Y = ls[1].Y - ls[0].Y;

            firstDerivative[1].X = ls[2].X - ls[1].X;
            firstDerivative[1].Y = ls[2].Y - ls[1].Y;

            firstDerivative[2].X = ls[3].X - ls[2].X;
            firstDerivative[2].Y = ls[3].Y - ls[2].Y;
            /*************************************************/

            secondDerivative[0].X = ls[2].X - 2 * ls[1].X + ls[0].X;
            secondDerivative[0].Y = ls[2].Y - 2 * ls[1].Y + ls[0].Y;

            secondDerivative[1].X = ls[3].X - 2 * ls[2].X + ls[1].X;
            secondDerivative[1].Y = ls[3].Y - 2 * ls[2].Y + ls[1].Y;
            /**************************************************/

            curv1 = (firstDerivative[0].X * secondDerivative[0].Y - (secondDerivative[0].X * firstDerivative[0].Y)) /
                    Math.Sqrt(Math.Pow((Math.Pow(firstDerivative[0].X, 2.0F) + Math.Pow(firstDerivative[0].Y, 2.0F)), 3.0F));

            curv2 = (firstDerivative[1].X * secondDerivative[1].Y - (secondDerivative[1].X * firstDerivative[1].Y)) /
                    Math.Sqrt(Math.Pow((Math.Pow(firstDerivative[1].X, 2.0F) + Math.Pow(firstDerivative[1].Y, 2.0F)), 3.0F));
        }
    }
}
