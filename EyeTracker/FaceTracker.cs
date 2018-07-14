using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    class FaceTracker
    {
        Task processingTask;
        bool running = true;
        public Point FacePoint { get; private set; } = new Point(0, 0);
        readonly int webcam;

        public FaceTracker(int webcam = -1)
        {
            this.webcam = webcam;
        }

        public void Start()
        {
            running = true;

            processingTask = new Task(CaptureWebcam);
            processingTask.Start();
        }

        public void Stop()
        {
            running = false;
        }

        private void CaptureWebcam()
        {
            VideoCapture cap;
            if (webcam >= 0) cap = new VideoCapture(webcam);
            else cap = new VideoCapture();

            CascadeClassifier cascEye = LoadCascade("haarcascade_eye_tree_eyeglasses.xml");
            CascadeClassifier cascFace = LoadCascade("haarcascade_frontalface_default.xml");

            while (running)
            {
                using (var imageFrame = cap.QueryFrame().ToImage<Bgr, Byte>())
                {
                    if (imageFrame != null)
                    {
                        var grayframe = imageFrame.Convert<Gray, byte>();
                        var faces = cascFace.DetectMultiScale(grayframe, 1.1, 10, Size.Empty);

                        Rectangle largestFace = Rectangle.Empty;
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

                            FacePoint = new Point(Convert.ToInt32(midX), Convert.ToInt32(eyesY));
                        }
                    }
                }

                Task.Delay(100);
            }
        }
    }
}
