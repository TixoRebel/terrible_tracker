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
        readonly FaceTracker tracker;
        readonly EyeTrackerHelper eyeHelper;
        string[] backgrounds;
        int curBackground = 0;

        Person[] people =
        {
            new Person { xLeft = 507, yLeft = 273, xRight = 607, yRight = 275, face = @"res\characters\anna_kendrick\anna_kendrick_face.png", white = @"res\characters\anna_kendrick\anna_kendrick_sclera.png", left = @"res\characters\anna_kendrick\anna_kendrick_left_iris.png", right = @"res\characters\anna_kendrick\anna_kendrick_right_iris.png" },
            //new Person { xLeft = 593, yLeft = 364, xRight = 1021, yRight = 384, face = @"res\characters\chris_hemsworth\chris_hemsworth_face.png", white = @"res\characters\chris_hemsworth\chris_hemsworth_sclera.png", left = @"res\characters\chris_hemsworth\chris_hemsworth_left_iris.png", right = @"res\characters\chris_hemsworth\chris_hemsworth_right_iris.png" },
            new Person { xLeft = 540, yLeft = 304, xRight = 690, yRight = 301, face = @"res\characters\jotaro\jotaro_kujo_face.png", white = @"res\characters\jotaro\jotaro_kujo_sclera.png", left = @"res\characters\jotaro\jotaro_kujo_left_iris.png", right = @"res\characters\jotaro\jotaro_kujo_right_iris.png" }

        };
        int curPerson = 0;

        public MainWindow()
        {
            backgrounds = Directory.GetFiles(System.IO.Path.Combine(Environment.CurrentDirectory, @"res\background\"));
            InitializeComponent();
            tracker = new FaceTracker(HandleImage);
            eyeHelper = new EyeTrackerHelper(260, tracker.WebcamWidth, tracker.WebcamHeight);
            eyeHelper.setVirtualEyeDepth(0.0033);
            RunOnUIThread(() =>
            {
                BitmapImage background = new BitmapImage(new Uri(backgrounds[0]));
                backgroundImage.ImageSource = background;
                BitmapImage face = new BitmapImage(new Uri(System.IO.Path.Combine(Environment.CurrentDirectory, people[curPerson].face)));
                faceImage.Source = new TransformedBitmap(face, new ScaleTransform(0.5, 0.5));
                Canvas.SetBottom(faceImage, 0);

                BitmapImage white = new BitmapImage(new Uri(System.IO.Path.Combine(Environment.CurrentDirectory, people[curPerson].white)));
                whiteImage.Source = new TransformedBitmap(white, new ScaleTransform(0.5, 0.5));
                Canvas.SetBottom(whiteImage, 0);

                BitmapImage left = new BitmapImage(new Uri(System.IO.Path.Combine(Environment.CurrentDirectory, people[curPerson].left)));
                leftImgae.Source = new TransformedBitmap(left, new ScaleTransform(0.5, 0.5));
                BitmapImage right = new BitmapImage(new Uri(System.IO.Path.Combine(Environment.CurrentDirectory, people[curPerson].right)));
                rightImage.Source = new TransformedBitmap(right, new ScaleTransform(0.5, 0.5));
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

                Canvas.SetTop(leftImgae, people[curPerson].yLeft + point.Y);
                Canvas.SetLeft(leftImgae, people[curPerson].xLeft + point.X);

                Canvas.SetTop(rightImage, people[curPerson].yRight + point.Y);
                Canvas.SetLeft(rightImage, people[curPerson].xRight + point.X);
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

                case Key.Up:
                    {
                        if (curPerson == people.Length - 1)
                            curPerson = 0;
                        else
                            curPerson++;

                        BitmapImage face = new BitmapImage(new Uri(System.IO.Path.Combine(Environment.CurrentDirectory, people[curPerson].face)));
                        faceImage.Source = new TransformedBitmap(face, new ScaleTransform(0.5, 0.5));
                        Canvas.SetBottom(faceImage, 0);

                        BitmapImage white = new BitmapImage(new Uri(System.IO.Path.Combine(Environment.CurrentDirectory, people[curPerson].white)));
                        whiteImage.Source = new TransformedBitmap(white, new ScaleTransform(0.5, 0.5));
                        Canvas.SetBottom(whiteImage, 0);

                        BitmapImage left = new BitmapImage(new Uri(System.IO.Path.Combine(Environment.CurrentDirectory, people[curPerson].left)));
                        leftImgae.Source = new TransformedBitmap(left, new ScaleTransform(0.5, 0.5));
                        BitmapImage right = new BitmapImage(new Uri(System.IO.Path.Combine(Environment.CurrentDirectory, people[curPerson].right)));
                        rightImage.Source = new TransformedBitmap(right, new ScaleTransform(0.5, 0.5));
                    }
                    break;

                case Key.Down:
                    {
                        if (curPerson == 0)
                            curPerson = people.Length - 1;
                        else
                            curPerson--;

                        BitmapImage face = new BitmapImage(new Uri(System.IO.Path.Combine(Environment.CurrentDirectory, people[curPerson].face)));
                        faceImage.Source = new TransformedBitmap(face, new ScaleTransform(0.5, 0.5));
                        Canvas.SetBottom(faceImage, 0);

                        BitmapImage white = new BitmapImage(new Uri(System.IO.Path.Combine(Environment.CurrentDirectory, people[curPerson].white)));
                        whiteImage.Source = new TransformedBitmap(white, new ScaleTransform(0.5, 0.5));
                        Canvas.SetBottom(whiteImage, 0);

                        BitmapImage left = new BitmapImage(new Uri(System.IO.Path.Combine(Environment.CurrentDirectory, people[curPerson].left)));
                        leftImgae.Source = new TransformedBitmap(left, new ScaleTransform(0.5, 0.5));
                        BitmapImage right = new BitmapImage(new Uri(System.IO.Path.Combine(Environment.CurrentDirectory, people[curPerson].right)));
                        rightImage.Source = new TransformedBitmap(right, new ScaleTransform(0.5, 0.5));
                    }
                    break;
            }
        }
    }
}