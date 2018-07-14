using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.Util;
using static EyeTracker.Utilities;
using System.Reflection;
using System.ComponentModel;

namespace EyeTracker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Task processingTask;
        bool running = true;
        System.Drawing.Point facePoint = new System.Drawing.Point(0, 0);

        public MainWindow()
        {
            InitializeComponent();

            processingTask = new Task(CaptureWebcam);
            processingTask.Start();
        }

        private void CaptureWebcam()
        {
            VideoCapture cap = new VideoCapture();
            CascadeClassifier cascEye = LoadCascade("haarcascade_eye_tree_eyeglasses.xml");
            CascadeClassifier cascFace = LoadCascade("haarcascade_frontalface_default.xml");

            while (running)
            {
                using (var imageFrame = cap.QueryFrame().ToImage<Bgr, Byte>())
                {
                    if (imageFrame != null)
                    {
                        var grayframe = imageFrame.Convert<Gray, byte>();
                        var faces = cascFace.DetectMultiScale(grayframe, 1.1, 10, System.Drawing.Size.Empty);

                        System.Drawing.Rectangle largestFace = System.Drawing.Rectangle.Empty;
                        float largestArea = 0;
                        bool isFace = false;

                        foreach (var face in faces)
                        {
                            float area = face.Width * face.Height;
                            if (area > largestArea)
                            {
                                largestArea = area;
                                largestFace = face;
                                isFace = true;
                            }
                        }

                        if (isFace)
                        {
                            float midX = largestFace.Left + largestFace.Width / 2.0f;
                            float midY = largestFace.Top + largestFace.Height / 2.0f;
                            float eyesY = midY - (largestFace.Height * 0.1f);

                            facePoint = new System.Drawing.Point(Convert.ToInt32(midX), Convert.ToInt32(eyesY));
                        }
                    }
                }

                Task.Delay(100);
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            running = false;
            base.OnClosing(e);
        }
    }
}
