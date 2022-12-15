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
using PLAIF_VisionPlatform.Interface;
using PLAIF_VisionPlatform.ViewModel.HelixView;
using System.Collections.ObjectModel;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;

namespace PLAIF_VisionPlatform.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged, Observer
    {
        public ICommand ImportClick { get; set; }
        public ICommand ExportClick { get; set; }

        public ICommand StartClick { get; set; }
        public ICommand CaptureClick { get; set; }

        private MainModel _mainModel;
        private RosbridgeMgr _rosmgr;

        private BitmapImage? img2D;

        public BitmapImage Img2D
        {
            get { return img2D!; }
            set
            {
                img2D = value;
                NotifyPropertyChanged(nameof(img2D));
            }
        }

        private BitmapImage? imgDepth;

        public BitmapImage ImgDepth
        {
            get { return imgDepth!; }
            set { 
                imgDepth = value;
                NotifyPropertyChanged(nameof(imgDepth));
            }
        }

        private bool isImported;

        public bool IsImported
        {
            get { return isImported; }
            set { 
                isImported = value;          
                Document.Instance.IsImported = value;
                NotifyPropertyChanged(nameof(IsImported));
            }
        }

        private readonly ObservableCollection<Vision_Result> _vision_Result;
        public ObservableCollection<Vision_Result> Vision_Result 
        { 
            get { return _vision_Result; }
        }


        public MainViewModel() 
        {
            _mainModel = new MainModel();
            _rosmgr = RosbridgeMgr.Instance;
            _rosmgr.SetMainModel(this);

            ImportClick = new RelayCommand<object>(ImportCommand, CanExcute_ImportButton); // AsyncRelayCommand로 하면 async 함수를 넣을 수 있다.
            ExportClick = new RelayCommand<object>(ExportCommand, CanExcute_ExportButton);
            CaptureClick = new RelayCommand<object>(CaptureCommand, CanExcute_CaptureButton);
            StartClick = new RelayCommand<object>(StartCommand, CanExcute_StartButton);

            _vision_Result = new ObservableCollection<Vision_Result>();

            IsImported = false;

            Document.Instance.updater.Add(this);
        }
        
        // color from red to blue
        private Color[] colors21 = new Color[21] { 
            Color.FromRgb(0xFF, 0x00, 0x00), 
            Color.FromRgb(0xFF, 0x33, 0x00), 
            Color.FromRgb(0xFF, 0x66, 0x00), 
            Color.FromRgb(0xFF, 0x99, 0x00),
            Color.FromRgb(0xFF, 0xCC, 0x00),
            Color.FromRgb(0xFF, 0xFF, 0x00), 
            Color.FromRgb(0xCC, 0xFF, 0x00), 
            Color.FromRgb(0x99, 0xFF, 0x00), 
            Color.FromRgb(0x66, 0xFF, 0x00), 
            Color.FromRgb(0x33, 0xFF, 0x00), 
            Color.FromRgb(0x00, 0xFF, 0x00), 
            Color.FromRgb(0x00, 0xFF, 0x33), 
            Color.FromRgb(0x00, 0xFF, 0x66), 
            Color.FromRgb(0x00, 0xFF, 0x99), 
            Color.FromRgb(0x00, 0xFF, 0xCC), 
            Color.FromRgb(0x00, 0xFF, 0xFF), 
            Color.FromRgb(0x00, 0xCC, 0xFF), 
            Color.FromRgb(0x00, 0x99, 0xFF), 
            Color.FromRgb(0x00, 0x66, 0xFF), 
            Color.FromRgb(0x00, 0x33, 0xFF),
            Color.FromRgb(0x00, 0x00, 0xFF),
        };

        public void ImportCommand(object msg)
        {
            try
            {
                Task task1 = Task.Run(() =>
                {
                    //Import File to Linux
                    //string cmdGetYaml = String.Format("scp {0}@{1}:~/catkin_ws/config/config_file/config_file.yaml .", Document.Instance.userinfo.username, Document.Instance.userinfo.ip_address);
                    //PowershellUtil.RunPowershell(cmdGetYaml);

                    Document.Instance.IsImported = SSHUtil.DownloadFile(Document.Instance.userinfo.ip_address,
                                                                        Document.Instance.userinfo.username,
                                                                        Document.Instance.userinfo.password,
                                                                        @"config_file.yaml",
                                                                        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\PLAIF\\AI Vision");

                });

                task1.Wait();

                string Filtpath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\PLAIF\\AI Vision\\config_file.yaml";
                Document.Instance.jsonUtil.Load(Filtpath, JsonUtil.FileType.Type_Yaml);
                Document.Instance.updater.Notify(Observer.Cmd.UpdateFromJson);

                IsImported = true;
            }
            catch
            {
                IsImported = false;
                MessageBox.Show("정상적으로 Import되지 않았습니다.");
            }
        }

        public bool CanExcute_ImportButton(object parameter)
        {
            return Document.Instance.IsConnected ? true : false;
        }

        public void ExportCommand(object parameter)
        {
            try
            {
                Document.Instance.updater.Notify(Observer.Cmd.UpdateToJson);
                string Filtpath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\PLAIF\\AI Vision\\config_file.yaml";
                Document.Instance.jsonUtil.Save(Filtpath, JsonUtil.FileType.Type_Yaml);

                Task task1 = Task.Run(() =>
                {
                    //Export File to Linux
                    //string cmdGetYaml = String.Format("scp config_file.yaml {0}@{1}:~/catkin_ws/config/config_file", Document.Instance.userinfo.username, Document.Instance.userinfo.ip_address);
                    //PowershellUtil.RunPowershell(cmdGetYaml);

                    Document.Instance.IsImported = SSHUtil.UploadFile(Document.Instance.userinfo.ip_address,
                                                                        Document.Instance.userinfo.username,
                                                                        Document.Instance.userinfo.password,
                                                                        @"config_file.yaml",
                                                                        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\PLAIF\\AI Vision\\config_file.yaml");

                });

                task1.Wait();
            }
            catch
            {
                MessageBox.Show("Config File을 찾을 수 없습니다.");
            }

            try
            {
                Task task2 = Task.Run(() =>
                {
                    string strZividSettingFilePath = Document.Instance.ZividSettingFile;

                    if(strZividSettingFilePath != null && strZividSettingFilePath != "")
                    {
                        //Export File to Linux
                        //string cmdGetYaml = String.Format("scp {0} {1}@{2}:~/catkin_ws/config/config_file", strZividSettingFilePath, Document.Instance.userinfo.username, Document.Instance.userinfo.ip_address);
                        //PowershellUtil.RunPowershell(cmdGetYaml);

                        Document.Instance.IsImported = SSHUtil.UploadFile(Document.Instance.userinfo.ip_address,
                                                                        Document.Instance.userinfo.username,
                                                                        Document.Instance.userinfo.password,
                                                                        @"config_file.yaml",
                                                                        strZividSettingFilePath);

                    }
                });

                task2.Wait();
            }
            catch
            {
                MessageBox.Show("Zivid Setting File을 찾을 수 없습니다.");
            }

        }

        public bool CanExcute_ExportButton(object parameter)
        {
            return Document.Instance.IsConnected ? true : false;
        }

        public void CaptureCommand(object parameter)
        {
            try
            {
                Task<bool> task = Task.Run(() =>
                {
                    _rosmgr.Capture();

                    return true;
                });
                task.Wait();
            }
            catch
            {
                MessageBox.Show("Capture가 정상적으로 진행되지 않았습니다.");
            }
        }

        public bool CanExcute_CaptureButton(object parameter)
        {
            return Document.Instance.IsConnected ? true : false;
        }

        public void StartCommand(object parameter)
        {
            try
            {
                Task<bool> task = Task.Run(() =>
                {
                    _rosmgr.Start_Action();

                    return true;
                });
                task.Wait();
            }
            catch
            {
                MessageBox.Show("Start가 정상적으로 진행되지 않았습니다.");
            }
        }
        public bool CanExcute_StartButton(object parameter)
        {
            return Document.Instance.IsConnected ? true : false;
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

        public void Create2DResultImage(string message)
        {
            byte[] bByte = Convert.FromBase64String(message);

            //2D Image
            Mat rgb = new Mat(1200, 1944, MatType.CV_8UC3, bByte);

            // 종우님 여기서 이런 느낌으로 배열 채워주시면 됩니다!
            Document.Instance.xy_2d_color_img.Clear();
            Document.Instance.xy_2d_color_img.Add(new(0, 0, Color.FromRgb(1, 2, 3)));

            //Mat rgb = new Mat();
            //Cv2.CvtColor(rgba, rgb, ColorConversionCodes.RGBA2BGR);

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

        // 이 함수를 비동기로 변경
        public void CreatePointCloud(string message)
        {
            byte[] bByte = Convert.FromBase64String(message);

            //PointCloud2
            int point_step = 16;
            int size = bByte.Length;
            size /= point_step;

            Document.Instance.xyz_pcd_list.Clear();

            for (int n = 0; n < size; n++)
            {
                int x_posi = n * point_step + 0;
                int y_posi = n * point_step + 4;
                int z_posi = n * point_step + 8;

                float x = BitConverter.ToSingle(bByte, x_posi);
                float y = BitConverter.ToSingle(bByte, y_posi);
                float z = BitConverter.ToSingle(bByte, z_posi);

                // filter NaN values
                if (float.IsNaN(x) || float.IsNaN(y) || float.IsNaN(z))
                    continue;

                Document.Instance.xyz_pcd_list.Add(new (x, y, z));
            }

            // window Form과 연결할 경우가 아니면, 문제없다.
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.DataBind, new Action(() => { UpdatePcdView(); }));
        }

        public void UpdatePcdView()
        {
            try
            {
                vgm.ClearVisual3Ds();

                // 나중에 사용자 높이 입력받아서 컷하는 기능 추가 필요
                float min_z = Document.Instance.xyz_pcd_list.Min(x => x.z);
                float max_z = Document.Instance.xyz_pcd_list.Max(x => x.z);

                const int POINTSIZE = 1; //display size for magnetometer points
                PointsVisual3D[] pv3ds = new PointsVisual3D[colors21.Length];
                Point3DCollection[] p3dcs = new Point3DCollection[colors21.Length];
                for (int i = 0; i < colors21.Length; i++)
                {
                    pv3ds[i] = new PointsVisual3D { Color = colors21[i], Size = POINTSIZE };
                    p3dcs[i] = new Point3DCollection(); // 이 시점에 pv3ds[i].Points에 넣으면 성능저하 발생하므로 나중에 한번에 넣는다.
                }

                for (int i = 0; i < Document.Instance.xyz_pcd_list.Count; i++)
                {
                    // min_z와 max_z 사이의 21등분 중 어느 구간에 속하는지 계산
                    int color_index = (int)((Document.Instance.xyz_pcd_list[i].z - min_z) / (max_z - min_z) * 20);
                    color_index = Math.Min(color_index, 20);
                    color_index = Math.Max(color_index, 0);

                    p3dcs[color_index].Add(new Point3D(Document.Instance.xyz_pcd_list[i].x, Document.Instance.xyz_pcd_list[i].y, Document.Instance.xyz_pcd_list[i].z));
                }

                foreach (var (pv3d, p3dc) in pv3ds.Zip(p3dcs, (pv3d, p3dc) => (pv3d, p3dc)))
                    pv3d.Points = p3dc;

                vgm.AddVisual3Ds(pv3ds.ToList<Visual3D>());
            }
            catch
            {

            }
        }

        private ViewportGeometryModel vgm;
        public ViewportGeometryModel Vgm
        {
            get { return vgm; }
            set { vgm = value; }
        }

        protected bool SetProperty<T>(ref T field, T newValue, [CallerMemberName] string propertyName = null!)
        {
            if (!Equals(field, newValue))
            {
                field = newValue;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
                return true;
            }

            return false;
        }
        private Model3D? geometryModel;
        public Model3D GeometryModel { get => geometryModel!; set => SetProperty(ref geometryModel, value); }

        public void ParsingVisionResult(JToken message)
        {
            //Model ID
            List<int> ModelID = new List<int>();
            foreach (JToken token in message["id"]!)
            {
                ModelID.Add(((int)token));
            }

            //Matching Score
            List<string> Matching_Score_Result = new List<string>();
            List<string> Matching_Score_Cond = new List<string>();
            string strMatching_Score_Cond = Document.Instance.jsonUtil.jsonVisionSetting["Vision"]!["vision_node1"]!["preprocessing"]!["min_matching_points"]!.ToString();
            for (int i = 0; i < ModelID.Count(); i++)
            {
                Matching_Score_Result.Add(100.ToString());
                Matching_Score_Cond.Add(strMatching_Score_Cond);
            }

            //Position
            List<Vector3> position = new List<Vector3>();
            List<Vector4> orientation = new List<Vector4>();
            foreach (JToken token in message["pos"]!["poses"]!)
            {
                Vector3 vec3 = new Vector3();
                vec3.X = ((float)token["position"]!["x"]!) * 1000f;
                vec3.Y = ((float)token["position"]!["y"]!) * 1000f;
                vec3.Z = ((float)token["position"]!["z"]!) * 1000f;
                position.Add(vec3);

                Vector4 vec4 = new Vector4();
                vec4.X = ((float)token["orientation"]!["x"]!) * 1000f;
                vec4.Y = ((float)token["orientation"]!["y"]!) * 1000f;
                vec4.Z = ((float)token["orientation"]!["z"]!) * 1000f;
                vec4.W = ((float)token["orientation"]!["w"]!) * 1000f;
                orientation.Add(vec4);            
            }

            Document.Instance.visionResult.Clear();
            for (int i = 0; i < ModelID.Count(); i++)
            {
                Vision_Result vs = new Vision_Result(i, ModelID[i], Matching_Score_Cond[i], Matching_Score_Result[i], position[i], orientation[i]);
                Document.Instance.visionResult.Add(vs);
            }

            // window Form과 연결할 경우가 아니면, 문제없다.
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.DataBind, new Action(() =>
            {
                try
                {
                    Document.Instance.updater.Notify(Observer.Cmd.UpdateFromJson);
                }
                catch
                {

                }
            }));   
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Update(Observer.Cmd cmd)
        {
            switch (cmd)
            {
                case Observer.Cmd.UpdateFromJson:
                    UpdateFromJson();
                    break;
                case Observer.Cmd.UpdateView:
                    UpdateView();
                    break;
            }
        }

        public void UpdateFromJson()
        {
            Vision_Result.Clear();
            foreach(var result in Document.Instance.visionResult)
            {
                Vision_Result.Add(result);
            }
        }
        public void UpdateView()
        {
            IsImported = Document.Instance.IsImported;
        }
    }
}
