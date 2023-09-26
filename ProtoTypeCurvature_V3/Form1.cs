using Accord.Statistics.Distributions.Univariate;
using Emgu.CV;
using Emgu.CV.ML;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProtoTypeCurvature_V3
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        VideoCapture captureFrame = null;
        Mat frame = new Mat();
        Mat frameEd = new Mat();
        double TotalFrames;
        string s;
        private void loadVideoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            cracked = 0;
            count = 0;
            lbl_cracked.Text = "Cracked Frames: " + cracked;

            if (captureFrame != null)
                captureFrame.Stop();

            if (DialogResult.OK == openFileDialog1.ShowDialog())
            {
                try
                {
                    captureFrame = new VideoCapture(openFileDialog1.FileName);
                    captureFrame.ImageGrabbed += ImageGrabbed;
                    TotalFrames = captureFrame.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.FrameCount);
                    listBox1.Items.Clear();
                    listBox1.Items.Add("Video: " + Path.GetFileName(openFileDialog1.FileName));
                    listBox1.Items.Add("Resolution: (" + captureFrame.Width + ", " + captureFrame.Height+")");
                    listBox1.Items.Add("Frame Rate:" + Math.Round(captureFrame.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.Fps), 2));

                    TimeSpan tsp = new TimeSpan(0, 0, (int)(TotalFrames /captureFrame.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.Fps)));
                    listBox1.Items.Add("Video Length:" + tsp);
                    listBox1.Items.Add("Total Frames: (" + TotalFrames + ")");
                    captureFrame.Start();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        int count =0;
        EDCLRLibraryV2.EDGenerator ed = new EDCLRLibraryV2.EDGenerator();
        MemoryStream sourceImageStream;

        float step =5;
        int cracked=0;
        private void ImageGrabbed(object sender, EventArgs e)
        {
            int samplestep = 5;//n
            float stepsize = 200; //z=1/200 = 0.005
            int curvatureRange = 100;

            count++;
            this.Text = "Actual Frame No. =" + count.ToString();

            captureFrame.Retrieve(frame);
            imageBox1.Image = frame;

            if ((count % step) == 0)
            {//do crack check
                sourceImageStream = new MemoryStream();
                frame.Bitmap.Save(sourceImageStream, System.Drawing.Imaging.ImageFormat.Png);
                byte[] sourceImageData = sourceImageStream.ToArray();

                Point[][] segments;
                
                unsafe
                {
                    int*** f;
                    fixed (byte* ptr = sourceImageData)
                    {
                        f = ed.convertToGray(ptr, sourceImageData.Length);
                    }

                    //int*** f = ed.Generate(file);

                    int imax = (int)f[0][0][0];
                    segments = new Point[imax][];

                    for (int i = 1; i < (imax + 1); i++)
                    {
                        int jmax = (int)f[i][0][0];
                        segments[i - 1] = new Point[jmax];
                        for (int j = 1; j < (jmax + 1); j++)
                        {
                            Point pf = new Point(f[i][j][0], f[i][j][1]);
                            segments[i - 1][j - 1] = pf;
                        }
                    }
                }

                /**************  TO SHOW THE ED  ***********************/
                Random rd = new Random((int)DateTime.Now.Ticks);
                //just to determine the size of the image
                Bitmap bm = new Bitmap(frame.Width, frame.Height);

                for (int i = 0; i < segments.GetLength(0); i++)
                {
                    //choose different colors for each segment
                    Color cl = Color.FromArgb(rd.Next(0, 255), rd.Next(0, 255), rd.Next(0, 255));
                    //Color cl = Color.FromArgb(255, 0, 0);

                    for (int j = 0; j < segments[i].GetLength(0); j++)
                    {
                       
                        bm.SetPixel(segments[i][j].X, segments[i][j].Y, cl);
                        //if we want to see the drawing in slow motion!!!!
                        //pictureBox1.Image = bm;
                        //Application.DoEvents();
                    }
                }
                pictureBox1.Image = bm;
                /********************        END ED *********************/
                Curvature2 curv = new Curvature2();
                double[,] histogram = curv.calculate(segments, 4, 1000);
                //double[,] histogram = curv.calculate(segments, samplestep, stepsize, curvatureRange);

                double[] hist = new double[10];// histogram.GetLength(0)];

                for (int i = 0; i < hist.Length; i++)
                {
                    hist[i] = histogram[i, 1];
                    if (hist[i] == 0)
                        hist[i] = 0.00000001;
                }

                //GammaDistribution gamma = GammaDistribution.Estimate(hist);
                GammaDistribution gamma = new GammaDistribution();
                gamma.Fit(hist);

                float alpha = (float)gamma.Shape;
                float beta = (float)gamma.Scale;

                // normalize the beta(theta) value and use the same min and max used for trainning.
                alpha = (alpha - minK) / (maxK - minK);
                beta = (beta - minTheta) / (maxTheta - minTheta);
                Matrix<float> row = new Matrix<float>(new float[,] { { alpha, beta } });

                float predict = svm.Predict(row);

                if (predict == 1)
                {
                    //lsbx_message.Items.Add("Frame: " + count + " ==> Cracked!");
                    //lsbx_message.SelectedIndex = lsbx_message.Items.Count - 1;
                    lbl_status.ForeColor = Color.Red;
                    lbl_status.Text = " Frame ID: " + count + " ==> Cracked!";
                    cracked++;
                    lbl_cracked.Text = "Cracked Frames: " + cracked.ToString();
                }
                else
                {
                    lbl_status.ForeColor = Color.Green;
                    lbl_status.Text = " Frame ID: " + count + " ==> None Cracked!";
                }

                
            }

            //if (count >= TotalFrames)
            //{
            //    string filename = Path.GetFileNameWithoutExtension(openFileDialog1.FileName);
            //    System.IO.StreamWriter SaveFile = new System.IO.StreamWriter(filename + "_Log.txt");
            //    foreach (string act in lsbx_message.Items)
            //    {
            //        SaveFile.WriteLine(act);
            //    }
            //    SaveFile.Close();
            //}
        }

        SVM svm = null;
        KNearest knn = null;
        float minTheta = float.MaxValue, maxTheta = float.MinValue;
        float minK = float.MaxValue, maxK = float.MinValue;
        private void Form1_Load(object sender, EventArgs e)
        {
            
            try
            {
                if (File.Exists("svm.txt"))
                {
                    svm = new SVM();
                    FileStorage file = new FileStorage("svm.txt", FileStorage.Mode.Read);
                    svm.Read(file.GetNode("opencv_ml_svm"));
                    // lsbx_main.Items.Add("Trained SVM is loaded !");
                    //knn = new KNearest();
                    //FileStorage file = new FileStorage("knn.txt", FileStorage.Mode.Read);
                    //knn.Read(file.GetNode("opencv_ml_knn"));
                }
                else
                {
                    MessageBox.Show("No SVM/KNN file to load.....");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            if (File.Exists("normalizationSVM.txt"))
            {
                StreamReader sr = new StreamReader("normalizationSVM.txt");
                string line = sr.ReadLine();
                string[] xy = line.Split(',');
                maxK = float.Parse(xy[0]);
                minK = float.Parse(xy[1]);

                line = sr.ReadLine();
                xy = line.Split(',');
                maxTheta = float.Parse(xy[0]);
                minTheta = float.Parse(xy[1]);
            }
            else
            {
                MessageBox.Show("No normalization file to load.....");
            }
        }

        private void stepToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StepSizeFrm frm = new StepSizeFrm();
            frm.ShowDialog();
            step = frm.stepsize;
        }

        private void btn_stop_Click(object sender, EventArgs e)
        {
            captureFrame.Stop();
        }

        private void btn_pause_Click(object sender, EventArgs e)
        {
            captureFrame.Pause();
        }

        private void lbl_title_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void lbl_cracked_Click(object sender, EventArgs e)
        {

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btn_loadVideo_Click(object sender, EventArgs e)
        {
            cracked = 0;
            count = 0;
            lbl_cracked.Text = "Cracked Frames: " + cracked;

            if (captureFrame != null)
                captureFrame.Stop();

            if (DialogResult.OK == openFileDialog1.ShowDialog())
            {
                try
                {
                    captureFrame = new VideoCapture(openFileDialog1.FileName);
                    captureFrame.ImageGrabbed += ImageGrabbed;
                    TotalFrames = captureFrame.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.FrameCount);
                    listBox1.Items.Clear();
                    listBox1.Items.Add("Video: " + Path.GetFileName(openFileDialog1.FileName));
                    listBox1.Items.Add("Resolution: (" + captureFrame.Width + ", " + captureFrame.Height + ")");
                    listBox1.Items.Add("Frame Rate:" + Math.Round(captureFrame.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.Fps), 2));

                    TimeSpan tsp = new TimeSpan(0, 0, (int)(TotalFrames / captureFrame.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.Fps)));
                    listBox1.Items.Add("Video Length:" + tsp);
                    listBox1.Items.Add("Total Frames: (" + TotalFrames + ")");
                    captureFrame.Start();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }

       private void btn_continue_Click(object sender, EventArgs e)
        {
            captureFrame.Start();
        }
    }
}
