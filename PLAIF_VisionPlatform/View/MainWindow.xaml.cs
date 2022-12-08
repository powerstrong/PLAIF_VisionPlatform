using PLAIF_VisionPlatform.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;
using Image = System.Windows.Controls.Image;

namespace PLAIF_VisionPlatform
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        /// <summary>
        /// 원본 포인트입니다.
        /// </summary>
        private Point origin_2D;

        /// <summary>
        /// 시작 포인트입니다.
        /// </summary>
        private Point start_2D;

        /// <summary>
        /// 원본 포인트입니다.
        /// </summary>
        private Point origin_3D;

        /// <summary>
        /// 시작 포인트입니다.
        /// </summary>
        private Point start_3D;

        public MainWindow()
        {
            InitializeComponent();

            #region 2D Image Zoom In/Out에 대한 정의
            TransformGroup transformGroup_2D = new TransformGroup();
            ScaleTransform scaleTransform_2D = new ScaleTransform();
            transformGroup_2D.Children.Add(scaleTransform_2D);
            TranslateTransform translateTransform_2D = new TranslateTransform();
            transformGroup_2D.Children.Add(translateTransform_2D);
            Image_2D_Control.RenderTransform = transformGroup_2D;

            Image_2D_Control.MouseWheel += Image_MouseWheel;
            Image_2D_Control.MouseLeftButtonDown += Image_MouseDown;
            Image_2D_Control.MouseLeftButtonUp += Image_MouseUp;
            Image_2D_Control.MouseMove += Image_MouseMove;
            #endregion

            #region Depth Image Zoom In/Out에 대한 정의
            TransformGroup transformGroup_3D = new TransformGroup();
            ScaleTransform scaleTransform_3D = new ScaleTransform();
            transformGroup_3D.Children.Add(scaleTransform_3D);
            TranslateTransform translateTransform_3D = new TranslateTransform();
            transformGroup_3D.Children.Add(translateTransform_3D);
            Image_Depth_Control.RenderTransform = transformGroup_3D;

            Image_Depth_Control.MouseWheel += Image_MouseWheel;
            Image_Depth_Control.MouseLeftButtonDown += Image_MouseDown;
            Image_Depth_Control.MouseLeftButtonUp += Image_MouseUp;
            Image_Depth_Control.MouseMove += Image_MouseMove;
            #endregion

            DataContext = new MainViewModel();
            navframe.Navigate(new Uri(@"/View/Settings View/ConnectionPage.xaml", UriKind.Relative));
        }

        private void sidebar_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected = sidebar.SelectedItem as NavButton;
            if (selected is not null)
                navframe.Navigate(selected.Navlink);
        }

        private void Image_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            Image? img = sender as Image;

            if (img != null)
            {
                var transform = (ScaleTransform)((TransformGroup)img.RenderTransform).Children.First(c => c is ScaleTransform);
                double zoom = e.Delta > 0 ? .1 : -.1;

                transform.ScaleX += zoom;
                transform.ScaleY += zoom;

                if (transform.ScaleX > 3) transform.ScaleX = 3;
                if (transform.ScaleY > 3) transform.ScaleY = 3;
                if (transform.ScaleX < 1) transform.ScaleX = 1;
                if (transform.ScaleY < 1) transform.ScaleY = 1;
            }
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Image? img = sender as Image;

            Point start;
            Point origin;
            Border border;

            if (img != null)
            {
                switch (img.Name)
                {
                    default:
                    case "Image_2D_Control":
                        border = border2D;
                        break;
                    case "Image_Depth_Control":
                        border = border3D;
                        break;
                }

                img.CaptureMouse();
                var translateTransform = (TranslateTransform)((TransformGroup)img.RenderTransform).Children.First(c => c is TranslateTransform);
                start = e.GetPosition(border);
                origin = new Point(translateTransform.X, translateTransform.Y);

                switch (img.Name)
                {
                    default:
                    case "Image_2D_Control":
                        start_2D = start;
                        origin_2D = origin;
                        break;
                    case "Image_Depth_Control":
                        start_3D = start;
                        origin_3D = origin;
                        break;
                }
            }
        }

        private void Image_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Image? img = sender as Image;
            if (img != null)
            {
                img.ReleaseMouseCapture();
            }            
        }

        private void Image_MouseMove(object sender, MouseEventArgs e)
        {
            Image? img = sender as Image;
            Point start;
            Point origin;
            Border border;

            if (img != null)
            {
                if (!img.IsMouseCaptured) return;

                switch (img.Name)
                {
                    default:
                    case "Image_2D_Control":
                        start = start_2D;
                        origin = origin_2D;
                        border = border2D;
                        break;
                    case "Image_Depth_Control":
                        start = start_3D;
                        origin = origin_3D;
                        border = border3D;
                        break;
                }

                var translateTransform = (TranslateTransform)((TransformGroup)img.RenderTransform).Children.First(c => c is TranslateTransform);
                var scaleTransform = (ScaleTransform)((TransformGroup)img.RenderTransform).Children.First(c => c is ScaleTransform);
                Vector v = start - e.GetPosition(border);
                translateTransform.X = origin.X - v.X;
                translateTransform.Y = origin.Y - v.Y;

                double LimitPoseX = ((img.Width * scaleTransform.ScaleX) - img.Width) / 2;
                double LimitPoseY = ((img.Height * scaleTransform.ScaleY) - img.Height) / 2;

                if (translateTransform.X > LimitPoseX) translateTransform.X = LimitPoseX;
                if (translateTransform.Y > LimitPoseY) translateTransform.Y = LimitPoseY;
                if (translateTransform.X < -LimitPoseX) translateTransform.X = -LimitPoseX;
                if (translateTransform.Y < -LimitPoseY) translateTransform.Y = -LimitPoseY;
            }  
        }
    }
}
