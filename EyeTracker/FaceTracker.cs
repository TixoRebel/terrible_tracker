﻿using System;
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
        readonly int webcam;
        readonly Action<FaceTracker> onUpdate;
        VideoCapture cap;

        public Point FacePoint { get; private set; } = new Point(0, 0);
        public int WebcamWidth { get; private set; } = 0;
        public int WebcamHeight { get; private set; } = 0;
        public int FaceWidth { get; private set; } = 0;
        public int FaceHeight { get; private set; } = 0;
        public bool Face { get; private set; } = false;

        public FaceTracker(int webcam = -1)
        {
            this.webcam = webcam;
        }

        public FaceTracker(Action<FaceTracker> onUpdate, int webcam = -1)
        {
            this.onUpdate = onUpdate;
            this.webcam = webcam;
            
            if (webcam >= 0) cap = new VideoCapture(webcam);
            else cap = new VideoCapture();

            WebcamWidth = cap.Width;
            WebcamHeight = cap.Height;
        }

        public bool Start()
        {
            running = true;

            if (!cap.IsOpened) return false;

            processingTask = new Task(() => { CaptureWebcam(cap); });
            processingTask.Start();
            return true;
        }

        public void Stop()
        {
            running = false;
        }

        private void CaptureWebcam(VideoCapture cap)
        {            
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
                            Face = true;
                            float midX = largestFace.Left + largestFace.Width / 2.0f;
                            float midY = largestFace.Top + largestFace.Height / 2.0f;
                            float eyesY = midY - (largestFace.Height * 0.1f);

                            FacePoint = new Point(Convert.ToInt32(midX), Convert.ToInt32(eyesY));
                            FaceWidth = largestFace.Width;
                            FaceHeight = largestFace.Height;

                            onUpdate?.Invoke(this);
                        }
                    }
                }

                Task.Delay(100);
            }
        }
    }
}
