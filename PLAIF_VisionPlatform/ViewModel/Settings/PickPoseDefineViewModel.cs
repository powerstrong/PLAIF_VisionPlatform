using PLAIF_VisionPlatform.Interface;
using PLAIF_VisionPlatform.Work;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PLAIF_VisionPlatform.ViewModel.Settings
{
    internal class PickPoseDefineViewModel : INotifyPropertyChanged
    {
        public PickPoseDefineViewModel()
        {
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

        private string _x = "1.0";
        private string _y = "2.0";
        private string _z = "3.0";
        private string _rx = "4.0";
        private string _ry = "5.0";
        private string _rz = "6.0";

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
