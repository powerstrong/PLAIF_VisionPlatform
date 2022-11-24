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

namespace PLAIF_VisionPlatform.ViewModel
{
    class MainViewModel : INotifyPropertyChanged
    {
        public DelegateCommand StartClick { get; set; }
        public IAsyncRelayCommand StopClick { get; set; }
        public ICommand PauseClick { get; set; }
        public ICommand CaptureClick { get; set; }
        public IAsyncRelayCommand ConnectClick { get; set; }

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
            _rosmgr = new RosbridgeMgr(this);

            StartClick = new DelegateCommand(DelegateTestCommand); // AsyncRelayCommand로 하면 async 함수를 넣을 수 있다.
            StopClick = new AsyncRelayCommand(AsyncTestCommand);
            ConnectClick = new AsyncRelayCommand(ConnectCommand);
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

        private string uriText = "ws://192.168.1.75:9090";

        public string UriText
        {
            get { return uriText; }
            set { uriText = value; 
				NotifyPropertyChanged(nameof(UriText));
            }
        }

        private string connectButtonText = "Connect to ROS";

        public string ConnectButtonText
        {
            get { return connectButtonText; }
            set { connectButtonText = value; 
				NotifyPropertyChanged(nameof(ConnectButtonText));
            }
        }


        public async Task ConnectCommand()
        {
            ConnectButtonText = _rosmgr.IsConnected() ? "Disconnecting..." : "Connecting..";
            Task<bool> task = Task.Run(() =>
            {
                _rosmgr.Connect(uriText); 
                
                return true;
            });
            task.Wait();
            ConnectButtonText = _rosmgr.IsConnected() ? "Connect to ROS" : "Disconnect from ROS";
        }

        public async Task CaptureCommand()
        {
            Task<bool> task = Task.Run(() =>
            {
                _rosmgr.Capture();

                return true;
            });
            task.Wait();

            Create2DBitMapImage();
            Create3DBitMapImage();
            CreatePointCloud();
        }

        private void Create2DBitMapImage()
        {
            using (StreamReader file = File.OpenText("2DImage_json.json")) //"2DImage_json.json" //PointCloudImage_json //DepthImage_json
            {
                using (JsonTextReader readder = new JsonTextReader(file))
                {
                    JObject json = (JObject)JToken.ReadFrom(readder);

                    if (json != null)
                    {
                        string strData = json["data"].ToString();
                        //byte[] bByte = Encoding.UTF8.GetBytes(strData);

                        byte[] bByte = Convert.FromBase64String(strData);

                        //2D Image
                        Mat rgba = new Mat(1200, 1944, MatType.CV_8UC4, bByte);
                        Mat rgb = new Mat();
                        Cv2.CvtColor(rgba, rgb, ColorConversionCodes.RGBA2BGR);
                        //Boolean Save = Cv2.ImWrite("TEST2D_RGB.jpg", rgb);

                        BitmapImage img = new BitmapImage();
                        img.BeginInit();
                        img.StreamSource = rgb.ToMemoryStream();
                        img.EndInit();
                        Img2D = img;
                    }
                }
            }
        }

        private void Create3DBitMapImage()
        {
            using (StreamReader file = File.OpenText("DepthImage_json.json")) //"2DImage_json.json" //PointCloudImage_json //DepthImage_json
            {
                using (JsonTextReader readder = new JsonTextReader(file))
                {
                    JObject json = (JObject)JToken.ReadFrom(readder);

                    if (json != null)
                    {
                        string strData = json["data"].ToString();
                        //byte[] bByte = Encoding.UTF8.GetBytes(strData);

                        byte[] bByte = Convert.FromBase64String(strData);

                        //Depth Image
                        Mat depth = new Mat(1200, 1944, MatType.CV_32FC1, bByte);
                        //Mat rgb = new Mat();
                        //Cv2.CvtColor(rgba, rgb, ColorConversionCodes.RGBA2BGR);
                        //Boolean Save = Cv2.ImWrite("TESTDepth.jpg", depth);

                        #region Method 1
                        double min;
                        double max;
                        Mat depthAdj = new Mat();
                        Cv2.MinMaxIdx(depth, out min, out max);
                        Cv2.ConvertScaleAbs(depth, depthAdj, 255 / max);
                        // Boolean Save = Cv2.ImWrite("TESTDepthBefore.jpg", depth);
                        //Save = Cv2.ImWrite("TESTDepthAfter.jpg", depthAdj);
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

                        BitmapImage img = new BitmapImage();
                        img.BeginInit();
                        img.StreamSource = depthAdj.ToMemoryStream();
                        img.EndInit();
                        ImgDepth = img;
                    }
                }
            }
        }

        private void CreatePointCloud()
        {
            using (StreamReader file = File.OpenText("PointCloudImage_json.json")) //"2DImage_json.json" //PointCloudImage_json //DepthImage_json
            {
                using (JsonTextReader readder = new JsonTextReader(file))
                {
                    JObject json = (JObject)JToken.ReadFrom(readder);

                    if (json != null)
                    {
                        string strData = json["data"].ToString();
                        //byte[] bByte = Encoding.UTF8.GetBytes(strData);

                        byte[] bByte = Convert.FromBase64String(strData);

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

                            //var str = pcl[n].ToString();
                            //if (!str.Contains("NaN"))
                            //{
                            //    var str2 = string.Format("{0:s} {1:s} {2:s}", pcl[n].X.ToString(), pcl[n].Y.ToString(), pcl[n].Z.ToString());
                            //}

                            //Log
                            Console.WriteLine("pclCoordinates:x=" + pcl[n].X + ",y=" + pcl[n].Y + ",z=" + pcl[n].Z);
                        }

                        PointCloud = pcl;
                    }
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
