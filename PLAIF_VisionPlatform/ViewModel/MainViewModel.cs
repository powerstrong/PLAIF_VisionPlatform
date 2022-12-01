using Microsoft.Toolkit.Mvvm.Input;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using OpenCvSharp;
using PLAIF_VisionPlatform.Model;
using PLAIF_VisionPlatform.ViewModel.Command;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Numerics;
using System.Windows.Media;
using System.Windows.Threading;
using PLAIF_VisionPlatform.Utilities;
using PLAIF_VisionPlatform.Work;

namespace PLAIF_VisionPlatform.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public DelegateCommand StartClick { get; set; }
        public IAsyncRelayCommand StopClick { get; set; }
        public ICommand PauseClick { get; set; }
        public ICommand CaptureClick { get; set; }

        private MainModel _mainModel;
        private RosbridgeMgr _rosmgr;

        private BitmapImage img2D;

        public BitmapImage Img2D
        {
            get { return img2D; }
            set
            {
                img2D = value;
                NotifyPropertyChanged(nameof(img2D));
            }
        }

        private BitmapImage imgDepth;

        public BitmapImage ImgDepth
        {
            get { return imgDepth; }
            set { 
                imgDepth = value;
                NotifyPropertyChanged(nameof(imgDepth));
            }
        }

        private Vector3[] pointCloud;

        public Vector3[] PointCloud
        {
            get { return pointCloud; }
            set { 
                pointCloud = value;
                NotifyPropertyChanged(nameof(pointCloud));
            }
        }


        public MainViewModel() 
        {
            _mainModel = new MainModel();
            _rosmgr = RosbridgeMgr.Instance;
            _rosmgr.SetMainModel(this);

            StartClick = new DelegateCommand(DelegateTestCommand); // AsyncRelayCommand로 하면 async 함수를 넣을 수 있다.
            StopClick = new AsyncRelayCommand(AsyncTestCommand);
            CaptureClick = new AsyncRelayCommand(CaptureCommand);
        }

        //      private int progressValue;
        //      public int ProgressValue
        //      {
        //          get { return progressValue; }
        //          set
        //          {
        //              progressValue = value;
        //              NotifyPropertyChanged(nameof(ProgressValue));
        //          }
        //      }

        public void DelegateTestCommand(object msg)
        {

        }

        public async Task AsyncTestCommand()
        {
            int w = 0;

            // Task.Run을 통해 Task를 생성하고 비동기 작업을 바로 실행
            Task task = Task.Run(() =>
            {
                // 여기 안에 오래 걸리는 작업을 넣어준다
                for (int i = 0; i < 10; i++)
                {
                    Thread.Sleep(500);
                }
            });
            Task<int> task2 = Task.Run(() =>
            {
                for (int i = 0; i < 10; i++)
                {
                    Thread.Sleep(1000);
                    w = i;
                }
                return 5;
            });
            
            w = await task2;
            MessageBox.Show(w.ToString());
        }

        public async Task CaptureCommand()
        {
            Task<bool> task = Task.Run(() =>
            {
                _rosmgr.Capture();

                return true;
            });
            task.Wait();
        }

        public void Create2DBitMapImage(string message)
        {

            byte[] bByte = Convert.FromBase64String(message);

            //2D Image
            Mat rgba = new Mat(1200, 1944, MatType.CV_8UC4, bByte);
            Mat rgb = new Mat();
            Cv2.CvtColor(rgba, rgb, ColorConversionCodes.RGBA2BGR);

            // window Form과 연결할 경우가 아니면, 문제없다.
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.DataBind, new Action(() =>
            {
                try
                {
                    BitmapImage img = new BitmapImage();
                    img.BeginInit();
                    img.StreamSource = rgb.ToMemoryStream();
                    img.EndInit();
                    Img2D = img;
                }
                catch
                {

                }
            }));


            #region json파일로 읽어서 보여주기 - 나중에 디버깅할때 필요할까?

            //using (StreamReader file = File.OpenText("2dimage_json.json")) //"2dimage_json.json" //pointcloudimage_json //depthimage_json
            //{
            //    using (JsonTextReader readder = new JsonTextReader(file))
            //    {
            //        JObject json = (JObject)JToken.ReadFrom(readder);

            //        if (json != null)
            //        {
            //            string strdata = json["data"].ToString();
            //            //byte[] bbyte = encoding.utf8.getbytes(strdata);

            //            byte[] bbyte = Convert.FromBase64String(strdata);

            //            //2d image
            //            Mat rgba = new Mat(1200, 1944, MatType.CV_8UC4, bbyte);
            //            Mat rgb = new Mat();
            //            Cv2.CvtColor(rgba, rgb, ColorConversionCodes.RGB2BGR);
            //            //boolean save = cv2.imwrite("test2d_rgb.jpg", rgb);

            //            BitmapImage img = new BitmapImage();
            //            img.BeginInit();
            //            img.StreamSource = rgb.ToMemoryStream();
            //            img.EndInit();
            //            Img2D = img;
            //        }
            //    }
            //}

            #endregion
        }

        public void Create3DBitMapImage(string message)
        {
            byte[] bByte = Convert.FromBase64String(message);

            //Depth Image
            Mat depth = new Mat(1200, 1944, MatType.CV_32FC1, bByte);

            #region Method 1
            double min;
            double max;
            Mat depthAdj = new Mat();
            Cv2.MinMaxIdx(depth, out min, out max);
            Cv2.ConvertScaleAbs(depth, depthAdj, 255 / max);
            #endregion

            #region Method 2
            //double min;
            //double max;
            //Mat depthAdj = new Mat();
            //Mat falseColorsMap = new Mat();
            //Cv2.MinMaxIdx(depth, out min, out max);

            //// expand your range to 0..255. Similar to histEq();
            //depth.ConvertTo(depthAdj, MatType.CV_8UC1, 255 / (max - min), -min);
            ////map.convertTo(adjMap, CV_8UC1, 255 / (max - min), -min);

            //// this is great. It converts your grayscale image into a tone-mapped one, 
            //// much more pleasing for the eye
            //// function is found in contrib module, so include contrib.hpp 
            //// and link accordingly
            //Cv2.ApplyColorMap(depthAdj, falseColorsMap, ColormapTypes.Autumn);
            //Cv2.CvtColor(falseColorsMap, falseColorsMap, ColorConversionCodes.BGRA2RGB);

            #endregion

            // window Form과 연결할 경우가 아니면, 문제없다.
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.DataBind, new Action(() =>
            {
                try
                {
                    BitmapImage img = new BitmapImage();
                    img.BeginInit();
                    img.StreamSource = depthAdj.ToMemoryStream();
                    img.EndInit();
                    ImgDepth = img;
                }
                catch
                {

                }
            }));

            #region json파일로 읽어서 보여주기 - 나중에 디버깅할때 필요할까?
            //using (StreamReader file = File.OpenText("DepthImage_json.json")) //"2DImage_json.json" //PointCloudImage_json //DepthImage_json
            //{
            //    using (JsonTextReader readder = new JsonTextReader(file))
            //    {
            //        JObject json = (JObject)JToken.ReadFrom(readder);

            //        if (json != null)
            //        {
            //            string strData = json["data"].ToString();
            //            //byte[] bByte = Encoding.UTF8.GetBytes(strData);

            //            byte[] bByte = Convert.FromBase64String(strData);

            //            //Depth Image
            //            Mat depth = new Mat(1200, 1944, MatType.CV_32FC1, bByte);
            //            //Mat rgb = new Mat();
            //            //Cv2.CvtColor(rgba, rgb, ColorConversionCodes.RGBA2BGR);
            //            //Boolean Save = Cv2.ImWrite("TESTDepth.jpg", depth);

            //            #region Method 1
            //            double min;
            //            double max;
            //            Mat depthAdj = new Mat();
            //            Cv2.MinMaxIdx(depth, out min, out max);
            //            Cv2.ConvertScaleAbs(depth, depthAdj, 255 / max);
            //            // Boolean Save = Cv2.ImWrite("TESTDepthBefore.jpg", depth);
            //            //Save = Cv2.ImWrite("TESTDepthAfter.jpg", depthAdj);
            //            #endregion

            //            #region Method 2
            //            //double min;
            //            //double max;
            //            //Mat depthAdj = new Mat();
            //            //Mat falseColorsMap = new Mat();
            //            //Cv2.MinMaxIdx(depth, out min, out max);

            //            //// expand your range to 0..255. Similar to histEq();
            //            //depth.ConvertTo(depthAdj, MatType.CV_8UC1, 255 / (max - min), -min);
            //            ////map.convertTo(adjMap, CV_8UC1, 255 / (max - min), -min);

            //            //// this is great. It converts your grayscale image into a tone-mapped one, 
            //            //// much more pleasing for the eye
            //            //// function is found in contrib module, so include contrib.hpp 
            //            //// and link accordingly
            //            //Cv2.ApplyColorMap(depthAdj, falseColorsMap, ColormapTypes.Autumn);
            //            //Cv2.CvtColor(falseColorsMap, falseColorsMap, ColorConversionCodes.BGRA2RGB);

            //            #endregion

            //            BitmapImage img = new BitmapImage();
            //            img.BeginInit();
            //            img.StreamSource = depthAdj.ToMemoryStream();
            //            img.EndInit();
            //            ImgDepth = img;
            //        }
            //    }
            //}
            #endregion
        }

        public void CreatePointCloud(string message)
        {

            byte[] bByte = Convert.FromBase64String(message);

            //PointCloud2
            int width = 1944;
            int height = 1200;
            int row_step = 31104;
            int point_step = 16;
            int size = bByte.Length;
            size = size / point_step;

            //Calculator
            Vector3[] pcl = new Vector3[size];

            for (int n = 0; n < size; n++)
            {
                int x_posi = n * point_step + 0;
                int y_posi = n * point_step + 4;
                int z_posi = n * point_step + 8;

                float x = BitConverter.ToSingle(bByte, x_posi);
                float y = BitConverter.ToSingle(bByte, y_posi);
                float z = BitConverter.ToSingle(bByte, z_posi);

                pcl[n] = new Vector3(x, y, z); //new Vector3(y, z, x);

                //Log
               // Console.WriteLine("pclCoordinates:x=" + pcl[n].X + ",y=" + pcl[n].Y + ",z=" + pcl[n].Z);

            }

            // window Form과 연결할 경우가 아니면, 문제없다.
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.DataBind, new Action(() =>
            {
                try
                {
                    PointCloud = pcl;
                }
                catch
                {

                }
            }));

            #region json파일로 읽어서 보여주기 - 나중에 디버깅할때 필요할까?
            //using (StreamReader file = File.OpenText("PointCloudImage_json.json")) //"2DImage_json.json" //PointCloudImage_json //DepthImage_json
            //{
            //    using (JsonTextReader readder = new JsonTextReader(file))
            //    {
            //        JObject json = (JObject)JToken.ReadFrom(readder);

            //        if (json != null)
            //        {
            //            string strData = json["data"].ToString();
            //            //byte[] bByte = Encoding.UTF8.GetBytes(strData);

            //            byte[] bByte = Convert.FromBase64String(strData);

            //            //PointCloud2
            //            int width = 1944;
            //            int height = 1200;
            //            int row_step = 31104;
            //            int point_step = 16;
            //            int size = bByte.Length;
            //            size = size / point_step;

            //            //Calculator
            //            Vector3[] pcl = new Vector3[size];

            //            for (int n = 0; n < size; n++)
            //            {
            //                int x_posi = n * point_step + 0;
            //                int y_posi = n * point_step + 4;
            //                int z_posi = n * point_step + 8;

            //                float x = BitConverter.ToSingle(bByte, x_posi);
            //                float y = BitConverter.ToSingle(bByte, y_posi);
            //                float z = BitConverter.ToSingle(bByte, z_posi);

            //                pcl[n] = new Vector3(x, y, z); //new Vector3(y, z, x);

            //                //var str = pcl[n].ToString();
            //                //if (!str.Contains("NaN"))
            //                //{
            //                //    var str2 = string.Format("{0:s} {1:s} {2:s}", pcl[n].X.ToString(), pcl[n].Y.ToString(), pcl[n].Z.ToString());
            //                //}

            //                //Log
            //                Console.WriteLine("pclCoordinates:x=" + pcl[n].X + ",y=" + pcl[n].Y + ",z=" + pcl[n].Z);
            //            }

            //            PointCloud = pcl;
            //        }
            //    }
            //}
            #endregion
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
