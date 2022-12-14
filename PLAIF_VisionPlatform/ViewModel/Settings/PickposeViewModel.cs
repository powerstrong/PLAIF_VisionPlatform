using HelixToolkit.Wpf;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Win32;
using PLAIF_VisionPlatform.Interface;
using PLAIF_VisionPlatform.Model;
using PLAIF_VisionPlatform.ViewModel.HelixView;
using PLAIF_VisionPlatform.Work;
using Ply.Net;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace PLAIF_VisionPlatform.ViewModel.Settings
{
    internal class PickposeViewModel : INotifyPropertyChanged, Observer
    {
        public ICommand? ImportPlyClick { get; set; }
        public ICommand? AddClick { get; set; }
        public ICommand? DelClick { get; set; }
        public ICommand? ModClick { get; set; }

        private ViewportGeometryModel? vgm;
        public ViewportGeometryModel Vgm
        {
            get { return vgm!; }
            set { vgm = value; }
        }

        public PickposeViewModel()
        {
            Document.Instance.updater.Add(this);

            ImportPlyClick = new RelayCommand(ImportPlyCommand);
            AddClick = new RelayCommand(AddCommand);
            DelClick = new RelayCommand(DelCommand);
            ModClick = new RelayCommand(ModCommand);
            _pickPoses = new ObservableCollection<Pickpose>();
            
            //this.UpdateToJson();
        }

        private Pickpose selectedPose = new();

        public Pickpose SelectedPose
        {
            get { return selectedPose; }
            set { 
                selectedPose = value;
                NotifyPropertyChanged(nameof(selectedPose));
            }
        }


        public void ImportPlyCommand()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Polygon Files(*.ply)|*.PLY|All files (*.*)|*.*";
            if (ofd.ShowDialog() == false) return;

            FileStream fs = new FileStream(ofd.FileName, mode: FileMode.Open);
            const int max_chunk_size = 1920*10800; // 정확히 무슨 뜻인지 모르겠다.
            PlyParser.Dataset ds = PlyParser.Parse(fs, max_chunk_size);
            
            var list = ds.Data.ToList();
            var vertex_list = list.Find(x => x.Element.Type == PlyParser.ElementType.Vertex);
            var face_list = list.Find(x => x.Element.Type == PlyParser.ElementType.Face);

            PlyParser.PropertyData xs = vertex_list!["x"]!;
            PlyParser.PropertyData ys = vertex_list!["y"]!;
            PlyParser.PropertyData zs = vertex_list!["z"]!;
            PlyParser.PropertyData cr = vertex_list!["red"]!;
            PlyParser.PropertyData cg = vertex_list!["green"]!;
            PlyParser.PropertyData cb = vertex_list!["blue"]!;
            PlyParser.PropertyData ca = vertex_list!["alpha"]!;

            const int POINTSIZE = 2; //display size for magnetometer points
            PointsVisual3D pv3d = new PointsVisual3D() { Color = Colors.Red, Size = POINTSIZE };
            Point3DCollection p3dc = new Point3DCollection();

            for (int i = 0; i < xs.Data.Length; i++)
            {
                var x = xs.Data.GetValue(i);
                var y = ys.Data.GetValue(i);
                var z = zs.Data.GetValue(i);
                p3dc.Add(new Point3D((float)x! * 100, (float)y! * 100, (float)z! * 100));
            }

            pv3d.Points = p3dc;
            vgm!.ReplaceVisual3Ds(new List<Visual3D> { pv3d });
            fs.Close();
        }

        PickPoseDefineViewService _pickPoseDefineViewService = new PickPoseDefineViewService();

        private ObservableCollection<Pickpose> _pickPoses;
        public ObservableCollection<Pickpose> PickPoses
        {
            get => _pickPoses;
            set
            {
                _pickPoses = value;
                NotifyPropertyChanged(nameof(_pickPoses));
            }
        }

        private void AddCommand()
        {
            _pickPoseDefineViewService.CreateWindow();
        }

        private void DelCommand()
        {
            Document.Instance.pickPoses.Remove(SelectedPose);
            Document.Instance.updater.NotifyToJson();
        }

        private void ModCommand()
        {
            if (_pickPoses.Contains(SelectedPose))
                _pickPoseDefineViewService.CreateWindow(SelectedPose);
        }

        public void UpdateFromJson()
        {
            //PickPoses.Clear();
            //foreach (var pp in Document.Instance.pickPoses)
            //{
            //    PickPoses.Add(pp);
            //}
        }

        public void UpdateToJson()
        {
            PickPoses.Clear();
            foreach (var pp in Document.Instance.pickPoses)
            {
                PickPoses.Add(pp);
            }
        }

        private void vp_raw_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //rawmodel.Rawview_MouseDown(sender, e);
        }

        protected bool SetProperty<T>(ref T field, T newValue, [CallerMemberName] string propertyName = "")
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

        public event PropertyChangedEventHandler? PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
