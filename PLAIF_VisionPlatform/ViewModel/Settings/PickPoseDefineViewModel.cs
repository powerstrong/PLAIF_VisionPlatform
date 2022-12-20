using PLAIF_VisionPlatform.Interface;
using PLAIF_VisionPlatform.Model;
using PLAIF_VisionPlatform.Utilities;
using PLAIF_VisionPlatform.Work;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PLAIF_VisionPlatform.ViewModel.Settings
{
    internal class PickPoseDefineViewModel : INotifyPropertyChanged
    {
        public RelayCommand<Window>? OKClick { get; private set; }
        public RelayCommand<Window>? CancelClick { get; private set; }

        private Pickpose? pose;

        public Pickpose? Pose
        {
            get { return pose; }
            set { pose = value; }
        }


        public PickPoseDefineViewModel()
        {
            OKClick = new RelayCommand<Window>(OKCommand);
            CancelClick = new RelayCommand<Window>(CancelCommand);
        }

        private void OKCommand(Window window)
        {
            if (pose is null)
            {
                pose = new Pickpose();
            }

            pose.X = Double.Parse(_x);
            pose.Y = Double.Parse(_y);
            pose.Z = Double.Parse(_z);
            pose.RX = Double.Parse(_rx);
            pose.RY = Double.Parse(_ry);
            pose.RZ = Double.Parse(_rz);

            if (Document.Instance.pickPoses.Contains(pose))
                Document.Instance.pickPoses[Document.Instance.pickPoses.IndexOf(pose)] = pose;
            else
                Document.Instance.pickPoses.Add(pose);

            Document.Instance.updater.Notify(Observer.Cmd.RedrawPickPoseView);

            if (window != null)
            {
                window.Close();
            }
        }

        private void CancelCommand(Window window)
        {
            if (window != null)
            {
                window.Close();
            }
        }

        public PickPoseDefineViewModel(string x, string y, string z, string rx, string ry, string rz)
        {
            _x = x;
            _y = y;
            _z = z;
            _rx = rx;
            _ry = ry;
            _rz = rz;
        }

        private string _x = "0.0";
        private string _y = "0.0";
        private string _z = "0.0";
        private string _rx = "0.0";
        private string _ry = "0.0";
        private string _rz = "0.0";

        public string X { get { return _x; } set { _x = value; NotifyPropertyChanged(nameof(_x)); }}
        public string Y { get { return _y; } set { _y = value; NotifyPropertyChanged(nameof(_y)); }}
        public string Z { get { return _z; } set { _z = value; NotifyPropertyChanged(nameof(_z)); }}
        public string Rx { get { return _rx; } set { _rx = value; NotifyPropertyChanged(nameof(_rx)); }}
        public string Ry { get { return _ry; } set { _ry = value; NotifyPropertyChanged(nameof(_ry)); }}
        public string Rz { get { return _rz; } set { _rz = value; NotifyPropertyChanged(nameof(_rz)); }}

        public event PropertyChangedEventHandler? PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
