using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Win32;
using PLAIF_VisionPlatform.Interface;
using PLAIF_VisionPlatform.Model;
using PLAIF_VisionPlatform.ViewModel.HelixView;
using PLAIF_VisionPlatform.Work;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Input;
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
            if (ofd.ShowDialog() == true)
            {
                FileStream fs = new FileStream(ofd.FileName, mode:FileMode.Open);
                const int max_chunk_size = 100000; // 정확히 무슨 뜻인지 모르겠다.
                Ply.Net.PlyParser.Dataset ds = Ply.Net.PlyParser.Parse(fs, max_chunk_size);
            }
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
