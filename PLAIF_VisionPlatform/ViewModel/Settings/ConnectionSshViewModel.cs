using Microsoft.Toolkit.Mvvm.Input;
using PLAIF_VisionPlatform.ViewModel.Command;
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
            if (window != null)
            {
                window.Close();
            }
        }

        private string ipAddress;

        public string IpAddress
        {
            get { return ipAddress; }
            set { ipAddress = value; }
        }

        private string password;

        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        
        private string username;

        public string Username
        {
            get { return username; }
            set { username = value; }
        }


        public event PropertyChangedEventHandler? PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
