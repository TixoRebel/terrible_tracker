﻿using System;
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
        public String BGImage { get; set; } = @"‪wedding.jpg";
        FaceTracker tracker = new FaceTracker();

        public MainWindow()
        {
            InitializeComponent();
            tracker.Start();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            tracker.Stop();
            base.OnClosing(e);
        }

        public void MoveEyes(System.Drawing.Point xyz)
        {
            Canvas.SetLeft(imgRightEye, xyz.X);
            Canvas.SetBottom(imgRightEye, xyz.Y);
            Canvas.SetLeft(imgLeftEyse, xyz.X);
            Canvas.SetBottom(imgLeftEyse, xyz.Y);
        }
    }
}