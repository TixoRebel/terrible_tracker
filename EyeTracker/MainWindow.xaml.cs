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
using System.IO;

namespace EyeTracker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public String BGImage { get; set; } = @"‪wedding.jpg";
        readonly FaceTracker tracker;
        readonly EyeTrackerHelper eyeHelper;
        string[] backgrounds;
        int curBackground = 0;

        public MainWindow()
        {
            backgrounds = Directory.GetFiles(System.IO.Path.Combine(Environment.CurrentDirectory, @"res\background\"));
            InitializeComponent();
            tracker = new FaceTracker(HandleImage);
            eyeHelper = new EyeTrackerHelper(260, tracker.WebcamWidth, tracker.WebcamHeight);
            eyeHelper.setVirtualEyeDepth(0.004);
            RunOnUIThread(() =>
            {
                BitmapImage background = new BitmapImage(new Uri(backgrounds[0]));
                backgroundImage.ImageSource = background;
                BitmapImage face = new BitmapImage(new Uri(System.IO.Path.Combine(Environment.CurrentDirectory, @"res\characters\anna_kendrick\anna_kendrick_face.png")));
                faceImage.Source = new TransformedBitmap(face, new ScaleTransform(0.5, 0.5));
                BitmapImage white = new BitmapImage(new Uri(System.IO.Path.Combine(Environment.CurrentDirectory, @"res\characters\anna_kendrick\anna_kendrick_sclera.png")));
                whiteImage.Source = new TransformedBitmap(white, new ScaleTransform(0.5, 0.5));
                BitmapImage left = new BitmapImage(new Uri(System.IO.Path.Combine(Environment.CurrentDirectory, @"res\characters\anna_kendrick\anna_kendrick_left_iris.png")));
                leftImgae.Source = new TransformedBitmap(left, new ScaleTransform(0.5, 0.5));
                BitmapImage right = new BitmapImage(new Uri(System.IO.Path.Combine(Environment.CurrentDirectory, @"res\characters\anna_kendrick\anna_kendrick_right_iris.png")));
                rightImage.Source = new TransformedBitmap(right, new ScaleTransform(0.5, 0.5));

                Canvas.SetTop(leftImgae, 208);
                Canvas.SetLeft(leftImgae, 537);

                Canvas.SetTop(rightImage, 210);
                Canvas.SetLeft(rightImage, 637);
            });
            tracker.Start();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            tracker.Stop();
            base.OnClosing(e);
        }

        void HandleImage(FaceTracker tra)
        {
            RunOnUIThread(() =>
            {
                var point = eyeHelper.getPupilRelativePos(60, 30, tra.FaceWidth, tra.FacePoint.X, tra.FacePoint.Y);
                //MessageBox.Show(tra.FaceWidth.ToString());

                Canvas.SetTop(leftImgae, 208 + point.Y - 15);
                Canvas.SetLeft(leftImgae, 537 + point.X - 30);

                Canvas.SetTop(rightImage, 210 + point.Y - 15);
                Canvas.SetLeft(rightImage, 637 + point.X - 30);
            });
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch(e.Key)
            {
                case Key.Left:
                    {
                        if (curBackground == backgrounds.Length - 1)
                            curBackground = 0;
                        else
                            curBackground++;

                        BitmapImage background = new BitmapImage(new Uri(backgrounds[curBackground]));
                        backgroundImage.ImageSource = background;
                    }
                    break;

                case Key.Right:
                    {
                        if (curBackground == 0)
                            curBackground = backgrounds.Length - 1;
                        else
                            curBackground--;

                        BitmapImage background = new BitmapImage(new Uri(backgrounds[curBackground]));
                        backgroundImage.ImageSource = background;
                    }
                    break;
            }
        }
    }
}