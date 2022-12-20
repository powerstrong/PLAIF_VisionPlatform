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
using System.Numerics;
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
            pose.QX = Double.Parse(_qx);
            pose.QY = Double.Parse(_qy);
            pose.QZ = Double.Parse(_qz);
            pose.QW = Double.Parse(_qw);

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

        public PickPoseDefineViewModel(string x, string y, string z, string qx, string qy, string qz, string qw)
        {
            _x = x;
            _y = y;
            _z = z;
            _qx = qx;
            _qy = qy;
            _qz = qz;
            _qw = qw;
        }

        private string _x = "0.0";
        private string _y = "0.0";
        private string _z = "0.0";
        private string _qx = "0.0";
        private string _qy = "0.0";
        private string _qz = "0.0";
        private string _qw = "0.0";

        public string X { get { return _x; } set { _x = value; NotifyPropertyChanged(nameof(_x)); }}
        public string Y { get { return _y; } set { _y = value; NotifyPropertyChanged(nameof(_y)); }}
        public string Z { get { return _z; } set { _z = value; NotifyPropertyChanged(nameof(_z)); }}
        public string Qx { get { return _qx; } set { _qx = value; NotifyPropertyChanged(nameof(_qx)); }}
        public string Qy { get { return _qy; } set { _qy = value; NotifyPropertyChanged(nameof(_qy)); }}
        public string Qz { get { return _qz; } set { _qz = value; NotifyPropertyChanged(nameof(_qz)); }}
        public string Qw { get { return _qw; } set { _qw = value; NotifyPropertyChanged(nameof(_qw)); }}

        public event PropertyChangedEventHandler? PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
