using Microsoft.Toolkit.Mvvm.Input;
using PLAIF_VisionPlatform.ViewModel.Command;
using PLAIF_VisionPlatform.Work;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace PLAIF_VisionPlatform.ViewModel.Settings
{
    class ConnectionSshViewModel : INotifyPropertyChanged
    {
        public RelayCommand<Window> OKWindowCommand { get; private set; }

        public ConnectionSshViewModel()
        {
            OKWindowCommand = new RelayCommand<Window>(OKWindow);
        }

        private void OKWindow(Window window)
        {
            var userinfo = Document.Instance.userinfo;
            userinfo.ip_address = _ipAddress;
            userinfo.username = _username;
            userinfo.password = _password;

            if (window != null)
            {
                window.Close();
            }
        }

        private string _ipAddress;

        public string IpAddress
        {
            get { return _ipAddress; }
            set { _ipAddress = value; }
        }

        private string _password;

        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }

        
        private string _username;

        public string Username
        {
            get { return _username; }
            set { _username = value; }
        }


        public event PropertyChangedEventHandler? PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
